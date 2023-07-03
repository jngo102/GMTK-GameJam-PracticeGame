using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
///     Singleton that manages global inputs.
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : MonoBehaviour {
    /// <summary>
    ///     The duration that an input may be buffered for.
    /// </summary>
    [SerializeField] private float inputBufferTime = 0.15f;

    /// <summary>
    ///     The buffered input action for jumping.
    /// </summary>
    public BufferedInputAction Jump;

    /// <summary>
    ///     The input action for moving.
    /// </summary>
    [NonSerialized] public InputAction Move;

    public PlayerInput PlayerInput { get; private set; }

    /// <summary>
    ///     Whether the input manager is enabled.
    /// </summary>
    public bool IsEnabled => PlayerInput.enabled;

    private void Awake() {
        SetupInputActions();
    }

    private void OnEnable() {
        Enable();
    }

    private void OnDisable() {
        Disable();
    }

    /// <summary>
    ///     Initialize input actions.
    /// </summary>
    private void SetupInputActions() {
        PlayerInput = GetComponent<PlayerInput>();
        var overridesJson = UIManager.Instance.ReferencePlayerActions.asset.SaveBindingOverridesAsJson();
        PlayerInput.actions.LoadBindingOverridesFromJson(overridesJson);

        Jump = new BufferedInputAction(PlayerInput.actions["jump"], inputBufferTime);

        Move = PlayerInput.actions["move"];
    }

    /// <summary>
    ///     Disable all input.
    /// </summary>
    public void Disable() {
        PlayerInput.actions.Disable();
    }

    /// <summary>
    ///     Enable all input.
    /// </summary>
    public void Enable() {
        PlayerInput.actions.Enable();
    }
}