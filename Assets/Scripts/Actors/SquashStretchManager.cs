using UnityEngine;

/// <summary>
///     Handle squashing and stretching of the game object.
/// </summary>
[RequireComponent(typeof(Animator))]
public class SquashStretchManager : MonoBehaviour {
    [SerializeField] private Animator squashStretchAnimator;
    private readonly int squashParameter = Animator.StringToHash("Squashing");
    private readonly int stretchParameter = Animator.StringToHash("Stretching");

    /// <summary>
    /// Whether to squash the game object.
    /// </summary>
    public bool Squash {
        set {
            squashStretchAnimator.SetBool(squashParameter, value);
            if (value) {
                squashStretchAnimator.Play("Squash");
            }
        }
    }
    
    /// <summary>
    ///     Whether to stretch the game object.
    /// </summary>
    public bool Stretch {
        set {
            squashStretchAnimator.SetBool(stretchParameter, value);
            if (value) {
                squashStretchAnimator.Play("Stretch");
            }
        }
    }

    /// <summary>
    ///     Reset the animator parameter that controls squashing.
    /// </summary>
    public void ResetSquashParameter() {
        squashStretchAnimator.SetBool(squashParameter, false);
    }
    
    /// <summary>
    ///     Reset the animator parameter that controls stretching.
    /// </summary>
    public void ResetStretchParameter() {
        squashStretchAnimator.SetBool(stretchParameter, false);
    }
}
