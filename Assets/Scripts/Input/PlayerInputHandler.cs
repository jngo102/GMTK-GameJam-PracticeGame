using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
///     Singleton that manages global inputs.
/// </summary>
public class PlayerInputHandler : MonoBehaviour {
    /// <summary>
    ///     The duration that an input may be buffered for.
    /// </summary>
    [SerializeField] private float inputBufferTime = 0.15f;

    public BufferedInputAction AttackLeft;
    public BufferedInputAction AttackRight;
    
    /// <summary>
    ///     The buffered input action for jumping.
    /// </summary>
    public BufferedInputAction Jump;
    
    /// <summary>
    ///     The input action for moving.
    /// </summary>
    [NonSerialized] public InputAction Move;
    
    [NonSerialized] public InputAction Sprint;

    private PlayerInputActions inputActions;

    /// <summary>
    ///     Whether the input manager is enabled.
    /// </summary>
    public bool IsEnabled => inputActions.asset.enabled;

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
        inputActions = new PlayerInputActions();
        var overridesJson = UIManager.Instance.ReferencePlayerActions.asset.SaveBindingOverridesAsJson();
        inputActions.LoadBindingOverridesFromJson(overridesJson);

        AttackLeft = new BufferedInputAction(inputActions.Player.AttackLeft, inputBufferTime);
        AttackRight = new BufferedInputAction(inputActions.Player.AttackRight, inputBufferTime);
        
        Jump = new BufferedInputAction(inputActions.Player.Jump, inputBufferTime);

        Move = inputActions.Player.Move;
        Sprint = inputActions.Player.Sprint;
    }

    /// <summary>
    ///     Disable all input.
    /// </summary>
    public void Disable() {
        inputActions.asset.Disable();
    }

    /// <summary>
    ///     Enable all input.
    /// </summary>
    public void Enable() {
        inputActions.asset.Enable();
    }
}