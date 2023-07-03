using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
///     An interactable actor.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public abstract class Interactable : MonoBehaviour {
    /// <summary>
    ///     Automatically interact when the player activates the trigger.
    /// </summary>
    [SerializeField] protected bool autoInteract;

    /// <summary>
    ///     Whether the player can interact with this actor.
    /// </summary>
    public bool CanInteract { get; private set; }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<PlayerInputHandler>().Move.performed += OnInteract;
            if (autoInteract)
                Interact();
            else
                CanInteract = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<PlayerInputHandler>().Move.performed -= OnInteract;
            CanInteract = false;
        }
    }

    /// <summary>
    ///     Callback for when the player manually interacts with this NPC.
    /// </summary>
    /// <param name="context">The input action callback context.</param>
    private void OnInteract(InputAction.CallbackContext context) {
        if (CanInteract && !autoInteract && Mathf.Abs(context.ReadValue<Vector2>().y) > 0) Interact();
    }

    /// <summary>
    ///     Interact with this actor.
    /// </summary>
    public abstract void Interact();
}