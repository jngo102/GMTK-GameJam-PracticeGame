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
    }

    /// <summary>
    ///     Create instances of all singletons.
    /// </summary>
    private static void CreateSingletons() {
        var gameManager = GameManager.Instance;
        var uiManager = UIManager.Instance;
        var saveDataManager = SaveDataManager.Instance;
#if UNITY_EDITOR
        Instantiate(Resources.Load<GameObject>($"{SingletonsDirName}/UnityExplorer"));
#endif
    }
}