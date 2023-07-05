using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
///     Singleton that manages the game state.
/// </summary>
[RequireComponent(typeof(PlayerInputManager))]
public class GameManager : Singleton<GameManager> {
    public delegate void OnLevelStart();

    /// <summary>
    ///     The player prefab.
    /// </summary>
    [SerializeField] private InnerPlayer innerPlayerPrefab;

    [SerializeField] private OuterPlayer outerPlayerPrefab;

    public InnerPlayer InnerPlayer { get; private set; }

    public OuterPlayer OuterPlayer { get; private set; }

    public event OnLevelStart LevelStarted;

    protected override void OnAwake() {
        CreatePlayers();
    }

    private void CreatePlayers() {
        InnerPlayer = Instantiate(innerPlayerPrefab);
        OuterPlayer = Instantiate(outerPlayerPrefab);
        InnerPlayer.gameObject.SetActive(false);
        OuterPlayer.gameObject.SetActive(false);
    }

    /// <summary>
    ///     Change scenes with a fade transition.
    /// </summary>
    /// <param name="sceneName">The name of the scene to change to.</param>
    /// <param name="sceneTransitionType">The type of scene transition when changing scenes.</param>
    /// <param name="entryName">The name of the scene transition trigger to enter from after the scene changes.</param>
    public void LoadScene(string sceneName, SceneType sceneType,
        SceneTransitionType sceneTransitionType = SceneTransitionType.Level, string entryName = null) {
        StartCoroutine(LoadSceneRoutine(sceneName, sceneType, sceneTransitionType, entryName));
    }

    /// <summary>
    ///     The routine that carries out the scene change sequence.
    /// </summary>
    /// <param name="sceneName">The name of the scene to change to.</param>
    /// <param name="sceneTransitionType">The type of scene transition when changing scenes.</param>
    /// <param name="entryName">The name of the scene transition trigger to load from.</param>
    public IEnumerator LoadSceneRoutine(string sceneName, SceneType sceneType, SceneTransitionType sceneTransitionType,
        string entryName) {
        var fader = UIManager.Instance.GetUI<Fader>();
        yield return fader.FadeIn();
        SaveDataManager.Instance.SaveGame();
        var loadOperation = SceneManager.LoadSceneAsync(sceneName,
            sceneType == SceneType.Inner ? LoadSceneMode.Single : LoadSceneMode.Additive);
        while (!loadOperation.isDone) yield return null;
        SaveDataManager.Instance.LoadGame();
        switch (sceneType) {
            case SceneType.Inner when SceneData.IsGameplayScene(sceneName):
                SceneSwitcher.ActivateInnerScene();
                switch (sceneTransitionType) {
                    case SceneTransitionType.Level when entryName != null:
                        StartInnerLevel(entryName);
                        break;
                    case SceneTransitionType.MainMenu:
                        StartInnerLevel();
                        break;
                }

                break;
            case SceneType.Outer when SceneData.IsGameplayScene(sceneName):
                SceneSwitcher.ActivateOuterScene();
                switch (sceneTransitionType) {
                    case SceneTransitionType.Level when entryName != null:
                        StartOuterLevel(entryName);
                        break;
                    case SceneTransitionType.MainMenu:
                        StartOuterLevel();
                        break;
                }

                break;
        }
        
        if (SceneData.IsGameplayScene(sceneName)) SwitchPlayer(sceneType);
        
        LevelStarted?.Invoke();

        yield return fader.FadeOut();
    }

    public void SwitchPlayer(SceneType playerType) {
        var cameraController = FindObjectOfType<CameraController>();
        switch (playerType) {
            case SceneType.Inner:
                InnerPlayer.gameObject.SetActive(true);
                cameraController.transform.localPosition = Vector3.back * 20;
                cameraController.smoothing = 0.1f;
                cameraController.Target = InnerPlayer.transform;
                OuterPlayer.gameObject.SetActive(false);
                break;
            case SceneType.Outer:
                OuterPlayer.gameObject.SetActive(true);
                cameraController.transform.localPosition = Vector3.back * 10;
                cameraController.smoothing = 0.5f;
                cameraController.Target = OuterPlayer.transform;
                InnerPlayer.gameObject.SetActive(false);
                break;
        }
    }

    public IEnumerator LoadSaveSpot(string saveInnerScene, string saveOuterScene) {
        StartCoroutine(LoadSceneRoutine(saveInnerScene, SceneType.Inner, SceneTransitionType.MainMenu, null));
        yield return SceneSwitcher.LoadOuterScene(saveOuterScene, SceneTransitionType.MainMenu, null);
        SceneSwitcher.ActivateOuterScene();
    }

    /// <summary>
    ///     Toggle whether the game is paused.
    /// </summary>
    public static void TogglePause() {
        if (Time.timeScale <= 0)
            ResumeGame();
        else
            PauseGame();
    }

    /// <summary>
    ///     Pause the game.
    /// </summary>
    public static void PauseGame() {
        Time.timeScale = 0;
    }

    /// <summary>
    ///     Resume the game.
    /// </summary>
    public static void ResumeGame() {
        Time.timeScale = 1;
    }

    /// <summary>
    ///     Quit the game.
    /// </summary>
    public static void QuitGame() {
        Application.Quit();
    }

    /// <summary>
    ///     Start a gameplay level from a scene transition trigger.
    /// </summary>
    /// <param name="entryName">The name of the scene transition trigger to enter from.</param>
    private void StartInnerLevel(string entryName) {
        var sceneTransitionTrigger = FindObjectsOfType<SceneTransitionTrigger>().FirstOrDefault(trigger =>
            trigger.name == entryName && trigger.gameObject.scene.name == SceneManager.GetActiveScene().name);
        if (!sceneTransitionTrigger) {
            Debug.LogError(
                $"No scene transition trigger found in scene {SceneManager.GetActiveScene().name} with entry name {entryName}.");
            return;
        }

        sceneTransitionTrigger.SceneType = SceneType.Inner;
        var triggerCollider = sceneTransitionTrigger.GetComponent<Collider2D>();
        var triggerTransform = sceneTransitionTrigger.transform;
        var triggerScale = triggerTransform.localScale;
        var triggerPosition = triggerTransform.position;
        var triggerWidth = triggerCollider.bounds.size.x;
        var targetX = triggerPosition.x + triggerWidth * triggerScale.x;
        triggerCollider.enabled = false;
        InnerPlayer.transform.position = triggerPosition;
        var playerInputHandler = InnerPlayer.GetComponent<PlayerInputHandler>();
        playerInputHandler.Disable();
        var playerScale = InnerPlayer.transform.localScale;
        playerScale = new Vector3(Mathf.Sign(triggerScale.x) * playerScale.x, playerScale.y, playerScale.z);
        InnerPlayer.transform.localScale = playerScale;
        var playerRunner = InnerPlayer.GetComponent<Runner>();
        playerRunner.RunTo(targetX);

        void RunFinishedHandler(Runner runner) {
            runner.StopRun();
            playerInputHandler.Enable();
            triggerCollider.enabled = true;
            playerRunner.AutoRunFinished -= RunFinishedHandler;
        }

        playerRunner.AutoRunFinished += RunFinishedHandler;
    }

    private void StartOuterLevel(string entryName) {
        var sceneTransitionTrigger = FindObjectsOfType<SceneTransitionTrigger>().FirstOrDefault(trigger =>
            trigger.name == entryName && trigger.gameObject.scene.name == SceneSwitcher.CurrentOuterScene);
        if (!sceneTransitionTrigger) {
            Debug.LogError(
                $"No scene transition trigger found in scene {SceneSwitcher.CurrentOuterScene} with entry name {entryName}.");
            return;
        }

        sceneTransitionTrigger.SceneType = SceneType.Outer;
        var triggerCollider = sceneTransitionTrigger.GetComponent<Collider2D>();
        var triggerTransform = sceneTransitionTrigger.transform;
        var triggerScale = triggerTransform.localScale;
        var triggerPosition = triggerTransform.position;
        var triggerWidth = triggerCollider.bounds.size.x;
        var targetX = triggerPosition.x + triggerWidth * triggerScale.x;
        triggerCollider.enabled = false;
        OuterPlayer.transform.position = triggerPosition;
        var playerInputHandler = OuterPlayer.GetComponent<PlayerInputHandler>();
        playerInputHandler.Disable();
        var playerScale = OuterPlayer.transform.localScale;
        playerScale = new Vector3(Mathf.Sign(triggerScale.x) * playerScale.x, playerScale.y, playerScale.z);
        OuterPlayer.transform.localScale = playerScale;
        var playerRunner = OuterPlayer.GetComponent<Runner>();
        playerRunner.RunTo(targetX);

        void RunFinishedHandler(Runner runner) {
            runner.StopRun();
            playerInputHandler.Enable();
            triggerCollider.enabled = true;
            playerRunner.AutoRunFinished -= RunFinishedHandler;
        }

        playerRunner.AutoRunFinished += RunFinishedHandler;
    }

    /// <summary>
    ///     Start a gameplay level from a save spot.
    /// </summary>
    private void StartInnerLevel() {
        var saveSpot = FindObjectsOfType<SaveSpot>()
            .FirstOrDefault(spot => spot.gameObject.scene.name == SceneManager.GetActiveScene().name);
        if (!saveSpot) {
            Debug.LogError($"No save spot found in scene {SceneManager.GetActiveScene().name}.");
            return;
        }

        saveSpot.SceneType = SceneType.Inner;
        InnerPlayer.transform.position = saveSpot.transform.position;
    }

    private void StartOuterLevel() {
        var saveSpot = FindObjectsOfType<SaveSpot>()
            .FirstOrDefault(spot => spot.gameObject.scene.name == SceneSwitcher.CurrentOuterScene);
        if (!saveSpot) {
            Debug.LogError($"No save spot found in scene {SceneSwitcher.CurrentOuterScene}.");
            return;
        }

        saveSpot.SceneType = SceneType.Outer;
        OuterPlayer.transform.position = saveSpot.transform.position;
    }
}