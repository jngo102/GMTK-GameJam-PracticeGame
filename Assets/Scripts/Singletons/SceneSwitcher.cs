using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneSwitcher : Singleton<SceneSwitcher> {
    private static SceneType _activeSceneType;
    public static string CurrentOuterScene { get; private set; }

    private void OnEnable() {
        if (UIManager.Instance == null) return;
        UIManager.Instance.Actions.Switch.performed += OnSwitch;
    }

    private void OnDisable() {
        if (UIManager.Instance == null) return;
        UIManager.Instance.Actions.Switch.performed -= OnSwitch;
    }

    private void OnSwitch(InputAction.CallbackContext context) {
        if (context.ReadValueAsButton()) {
            if (_activeSceneType == SceneType.Outer)
                ActivateInnerScene();
            else
                ActivateOuterScene();
        }
    }

    public static IEnumerator LoadOuterScene(string sceneName, SceneTransitionType sceneTransitionType,
        string entryName) {
        if (SceneUtility.GetBuildIndexByScenePath(sceneName) < 0) yield break;
        if (!string.IsNullOrEmpty(CurrentOuterScene) && CurrentOuterScene != sceneName) yield return UnloadOuterScene();

        CurrentOuterScene = sceneName;
        yield return GameManager.Instance.LoadSceneRoutine(sceneName, SceneType.Outer, sceneTransitionType, entryName);
    }

    public static IEnumerator UnloadOuterScene() {
        if (SceneUtility.GetBuildIndexByScenePath(CurrentOuterScene) < 0 ||
            !SceneManager.GetSceneByName(CurrentOuterScene).isLoaded) yield break;
        yield return SceneManager.UnloadSceneAsync(CurrentOuterScene);
    }

    public static void ActivateInnerScene() {
        if (SceneUtility.GetBuildIndexByScenePath(SceneManager.GetActiveScene().name) < 0) return;
        _activeSceneType = SceneType.Inner;
        GameManager.Instance.SwitchPlayer(SceneType.Inner);
        if (SceneUtility.GetBuildIndexByScenePath(CurrentOuterScene) >= 0)
            foreach (var gameObject in SceneManager.GetSceneByName(CurrentOuterScene).GetRootGameObjects())
                gameObject.SetActive(false);

        foreach (var gameObject in SceneManager.GetActiveScene().GetRootGameObjects()) gameObject.SetActive(true);

        UIManager.Instance.CloseUI<Vignette>();
    }

    public static void ActivateOuterScene() {
        if (string.IsNullOrEmpty(CurrentOuterScene) ||
            SceneUtility.GetBuildIndexByScenePath(CurrentOuterScene) < 0) return;
        _activeSceneType = SceneType.Outer;
        GameManager.Instance.SwitchPlayer(SceneType.Outer);
        foreach (var gameObject in SceneManager.GetActiveScene().GetRootGameObjects()) gameObject.SetActive(false);

        foreach (var gameObject in SceneManager.GetSceneByName(CurrentOuterScene).GetRootGameObjects())
            gameObject.SetActive(true);

        // UIManager.Instance.OpenUI<Vignette>();
    }
}