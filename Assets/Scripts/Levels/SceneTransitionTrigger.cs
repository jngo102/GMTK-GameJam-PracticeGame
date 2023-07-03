using UnityEngine;

/// <summary>
///     Behaviour that starts  a scene transition when triggered.  
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class SceneTransitionTrigger : MonoBehaviour {
    /// <summary>
    ///     The name of the scene that this transitions to.
    /// </summary>
    [SerializeField] private string toScene;

    /// <summary>
    ///     The name of the scene transition trigger that the player will enter from after transitioning to the next scene.
    /// </summary>
    [SerializeField] private string entryName;

    /// <summary>
    ///     The name of the scene transition trigger that the player will enter from after transitioning to the next scene.
    /// </summary>
    public string EntryName => entryName;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) GameManager.Instance.ChangeScene(toScene, SceneTransitionType.Level, entryName);
    }
}