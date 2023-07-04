using UnityEngine;
using UnityEngine.InputSystem;

public class InnerPlayer : Player {
    private AttackManager weapon;
    
    #region Unity Functions

    protected override void Awake() {
        base.Awake();
        
        weapon = GetComponentInChildren<AttackManager>();
    }

    protected override void Update() {
        base.Update();
        
        if (InputHandler.AttackLeft.IsBuffered()) {
            Facer.FaceDirection(-1);
            weapon.Attack();
        } else if (InputHandler.AttackRight.IsBuffered()) {
            Facer.FaceDirection(1);
            weapon.Attack();
        }
    }
    
    private void OnEnable() {
        EnableAllInputs();
    }

    private void OnDisable() {
        DisableAllInputs();
    }

    #endregion
    
    /// <summary>
    ///     Enable all player inputs.
    /// </summary>
    public override void EnableAllInputs() {
        base.EnableAllInputs();
        
        InputHandler.AttackLeft.InputAction.performed += OnAttackLeftStart;
        InputHandler.AttackRight.InputAction.performed += OnAttackRightStart;
        InputHandler.Jump.InputAction.canceled += OnJumpStop;
    }
    
    /// <summary>
    ///     Disable all player inputs.
    /// </summary>
    public override void DisableAllInputs() {
        base.DisableAllInputs();
        
        InputHandler.AttackLeft.InputAction.performed -= OnAttackLeftStart;
        InputHandler.AttackRight.InputAction.performed -= OnAttackRightStart;
        InputHandler.Jump.InputAction.canceled -= OnJumpStop;
    }

    private void OnAttackLeftStart(InputAction.CallbackContext context) {
        Facer.FaceDirection(-1);
        weapon.Attack();
    }
    
    private void OnAttackRightStart(InputAction.CallbackContext context) {
        Facer.FaceDirection(1);
        weapon.Attack();
    }
    
    /// <summary>
    ///     Callback for when the player ends a jump.
    /// </summary>
    /// <param name="context">The input action callback context.</param>
    private void OnJumpStop(InputAction.CallbackContext context) {
        Jumper.CancelJump();
    }
}