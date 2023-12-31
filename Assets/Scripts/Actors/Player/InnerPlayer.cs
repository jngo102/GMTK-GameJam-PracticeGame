using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(HealthManager))]
public class InnerPlayer : Player {
    private HealthManager healthManager;
    private AttackManager weapon;

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

    #region Unity Functions

    protected override void Awake() {
        base.Awake();

        healthManager = GetComponent<HealthManager>();
        weapon = GetComponentInChildren<AttackManager>();

        healthManager.Harmed += OnHurt;
    }

    protected override void Update() {
        base.Update();

        var inputVector = InputHandler.Move.ReadValue<Vector2>();
        Runner.SmoothRun(inputVector.x);
        if (inputVector.magnitude > 0 && !Animator.GetBool(RunParameter)) {
            Animator.SetBool(RunParameter, true);
        }
        else if (Animator.GetBool(RunParameter)) {
            Animator.SetBool(RunParameter, false);
        }
        //Facer.CheckFlip();

        if (InputHandler.AttackLeft.IsBuffered()) {
            Facer.FaceDirection(-1);
            weapon.Attack();
        }
        else if (InputHandler.AttackRight.IsBuffered()) {
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
    
    private void OnHurt(float damageAmount, Damager damageSource) {
        var organs = FindObjectsOfType<Organ>();
        var organ = organs[Random.Range(0, organs.Length)];
        organ.Infect();
        
        healthManager.FullHeal();
        var respawnPoints = FindObjectsOfType<RespawnPoint>();
        var respawnPoint = respawnPoints[Random.Range(0, respawnPoints.Length)];
        transform.position = respawnPoint.transform.position;
    }
}