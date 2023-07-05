using System.Collections;
using UnityEngine;

/// <summary>
///     Global manager that initializes all singletons.
/// </summary>
public class GlobalManager : MonoBehaviour {
    /// <summary>
    ///     The immediate parent directory of all the singleton prefabs.
    /// </summary>
    private const string SingletonsDirName = "Singletons";

    /// <summary>
    ///     Initialize is run on game start.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize() {
        CreateSingletons();
#if UNITY_EDITOR
        GameManager.Instance.StartCoroutine(LoadMainMenu());
#endif
    }
    
    private static IEnumerator LoadMainMenu() {    
        yield return SceneSwitcher.UnloadOuterScene();
        var asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainMenu");
        while (!asyncOperation.isDone) yield return null;
        SaveDataManager.Instance.LoadGame();
    }

    /// <summary>
    ///     Create instances of all singletons.
    /// </summary>
    private static void CreateSingletons() {
        var audioManager = AudioManager.Instance;
        var gameManager = GameManager.Instance;
        var uiManager = UIManager.Instance;
        var saveDataManager = SaveDataManager.Instance;
        var sceneSwitcher = SceneSwitcher.Instance;
#if UNITY_EDITOR
        Instantiate(Resources.Load<GameObject>($"{SingletonsDirName}/UnityExplorer"));
#endif
    }
}