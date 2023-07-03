using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
///     Singleton that manages the game state.
/// </summary>
[RequireComponent(typeof(PlayerInputManager))]
public class GameManager : Singleton<GameManager> {
    /// <summary>
    ///     The player prefab.
    /// </summary>
    [SerializeField] private Player playerPrefab;

    public delegate void OnLevelStart();

    public event OnLevelStart LevelStarted;

    private PlayerInputManager playerInputManager;

    private readonly Dictionary<InputDevice, Player> players = new();
    
    protected override void OnAwake() {
        playerInputManager = GetComponent<PlayerInputManager>();

        InputSystem.onDeviceChange += OnDeviceChange;
    }

    /// <summary>
    ///     Callback for when a controller event occurs.
    /// </summary>
    /// <param name="device">The device that triggered the event.</param>
    /// <param name="change">The change that occurred.</param>
    private void OnDeviceChange(InputDevice device, InputDeviceChange change) {
        switch (change) {
            case InputDeviceChange.Added:
            case InputDeviceChange.Reconnected:
                AddNewPlayer(device);
                break;
            case InputDeviceChange.Disconnected:
                RemoveExistingPlayer(device);
                break;
        }
    }

    /// <summary>
    ///     Change scenes with a fade transition.
    /// </summary>
    /// <param name="sceneName">The name of the scene to change to.</param>
    /// <param name="sceneTransitionType">The type of scene transition when changing scenes.</param>
    /// <param name="entryName">The name of the scene transition trigger to enter from after the scene changes.</param>
    public void ChangeScene(string sceneName, SceneTransitionType sceneTransitionType = SceneTransitionType.Level,
        string entryName = null) {
        StartCoroutine(ChangeSceneRoutine(sceneName, sceneTransitionType, entryName));
    }

    /// <summary>
    ///     The routine that carries out the scene change sequence.
    /// </summary>
    /// <param name="sceneName">The name of the scene to change to.</param>
    /// <param name="sceneTransitionType">The type of scene transition when changing scenes.</param>
    /// <param name="entryName">The name of the scene transition trigger to load from.</param>
    /// <returns></returns>
    public IEnumerator ChangeSceneRoutine(string sceneName, SceneTransitionType sceneTransitionType, string entryName) {
        var fader = UIManager.Instance.GetUI<Fader>();
        yield return fader.FadeIn();
        SaveDataManager.Instance.SaveGame();
        yield return SceneManager.LoadSceneAsync(sceneName);
        SaveDataManager.Instance.LoadGame();
        SetupPlayers();
        switch (sceneTransitionType) {
            case SceneTransitionType.Level when SceneData.IsGameplayScene(sceneName) && entryName != null:
                StartLevel(entryName);
                break;
            case SceneTransitionType.MainMenu when SceneData.IsGameplayScene(sceneName):
                StartLevel();
                break;
        }
        
        LevelStarted?.Invoke();
        
        yield return fader.FadeOut();
    }

    /// <summary>
    ///     Load the player at a save spot.
    /// </summary>
    /// <param name="saveScene">The saved scene to load.</param>
    public void LoadSaveSpot(string saveScene) {
        ChangeScene(saveScene, SceneTransitionType.MainMenu);
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
    private void StartLevel(string entryName) {
        var sceneTransitionTrigger = FindObjectsOfType<SceneTransitionTrigger>()
            .FirstOrDefault(trigger => trigger.name == entryName);
        if (!sceneTransitionTrigger) {
            Debug.LogError(
                $"No scene transition trigger found in scene {SceneManager.GetActiveScene().name} with entry name {entryName}.");
            return;
        }
        
        var triggerCollider = sceneTransitionTrigger.GetComponent<Collider2D>();
        var triggerTransform = sceneTransitionTrigger.transform;
        var triggerScale = triggerTransform.localScale;
        var triggerWidth = triggerCollider.bounds.size.x;
        var targetX = triggerTransform.position.x + triggerWidth * triggerScale.x;
        foreach (var (_, player) in players) {
            triggerCollider.enabled = false;
            player.transform.position = sceneTransitionTrigger.transform.position;
            var playerInputHandler = player.GetComponent<PlayerInputHandler>();
            playerInputHandler.Disable();
            var playerScale = player.transform.localScale;
            playerScale = new Vector3(Mathf.Sign(triggerScale.x) * playerScale.x, playerScale.y, playerScale.z);
            player.transform.localScale = playerScale;
            var playerRunner = player.GetComponent<Runner>();
            playerRunner.RunTo(targetX);

            void RunFinishedHandler(Runner runner) {
                runner.StopRun();
                playerInputHandler.Enable();
                triggerCollider.enabled = true;
                playerRunner.AutoRunFinished -= RunFinishedHandler;
            }
            
            playerRunner.AutoRunFinished += RunFinishedHandler;
        }
    }

    /// <summary>
    ///     Start a gameplay level from a save spot.
    /// </summary>
    private void StartLevel() {
        var saveSpot = FindObjectOfType<SaveSpot>();
        if (!saveSpot) {
            Debug.LogError($"No save spot found in scene {SceneManager.GetActiveScene().name}.");
            return;
        }

        foreach (var (_, player) in players) {
            player.transform.position = saveSpot.transform.position;
        }
    }

    /// <summary>
    ///     Add a new player state to the game manager.
    /// </summary>
    /// <param name="device">The device that will be used to control the new player.</param>
    private void AddNewPlayer(InputDevice device) {
        var playerInput = playerInputManager.JoinPlayer(device.deviceId, -1, null, device);
        var existingPlayer = players.Values.FirstOrDefault();
        if (existingPlayer) playerInput.transform.position = existingPlayer.transform.position;
        var player = playerInput.GetComponent<Player>();
        player.gameObject.SetActive(SceneData.IsGameplayScene(SceneManager.GetActiveScene().name));
        players.Add(device, player);
    }

    /// <summary>
    ///     Remove an existing player state from the game manager. 
    /// </summary>
    /// <param name="device">The input device key associated with the player to remove.</param>
    private void RemoveExistingPlayer(InputDevice device) {
        Destroy(players[device].gameObject);
        players.Remove(device);
    }

    /// <summary>
    ///     Set up all the players for the new scene.
    /// </summary>
    private void SetupPlayers() {
        foreach (var device in InputSystem.devices) {
            if (device is not (Gamepad or Keyboard)) continue;
            if (players.TryGetValue(device, out var player)) {
                player.gameObject.SetActive(SceneData.IsGameplayScene(SceneManager.GetActiveScene().name));
            } else {
                AddNewPlayer(device);
            }
        }
    }
}