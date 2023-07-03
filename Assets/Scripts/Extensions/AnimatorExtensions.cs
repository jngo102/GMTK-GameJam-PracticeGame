using UnityEngine;

/// <summary>
///     Extensions for the Unity Animator component.
/// </summary>
public static class AnimatorExtensions {
    /// <summary>
    ///     Whether the animator has finished playing.
    /// </summary>
    /// <param name="animator">The animator to check.</param>
    /// <returns>Whether the animator has finished playing.</returns>
    public static bool IsFinished(this Animator animator) {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1;
    }
}