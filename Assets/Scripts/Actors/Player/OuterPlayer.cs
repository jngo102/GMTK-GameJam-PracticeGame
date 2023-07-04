using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Glider))]
public class OuterPlayer : Player {
    [SerializeField] private float runSpeed = 5;
    [SerializeField] private float sprintSpeed = 8;

    private Glider glider;

    protected override void Awake() {
        base.Awake();

        glider = GetComponent<Glider>();
    }

    protected override void Update() {
        base.Update();

        InputVector = InputHandler.Move.ReadValue<Vector2>();
        if (InputVector.magnitude > 0) {
            if (!Animator.GetBool(RunParameter)) {
                Animator.SetBool(RunParameter, true);
            }
            Runner.Run(InputVector.x);
            Facer.CheckFlip();
        }
        else {
            if (Animator.GetBool(RunParameter)) {
                Animator.SetBool(RunParameter, false);
            }
            Runner.StopRun();
        }

        if (Body.velocity.y <= 0 && InputHandler.Jump.InputAction.IsPressed() && !glider.IsGliding) {
            Animator.SetBool(GlideParameter, true);
            glider.StartGliding();   
        }
    }

    private void OnEnable() {
        EnableAllInputs();
    }

    private void OnDisable() {
        DisableAllInputs();
    }

    public override void EnableAllInputs() {
        base.EnableAllInputs();

        InputHandler.Jump.InputAction.canceled += OnJumpStop;
        InputHandler.Sprint.performed += OnSprintStart;
        InputHandler.Sprint.canceled += OnSprintStop;
    }

    public override void DisableAllInputs() {
        base.DisableAllInputs();

        InputHandler.Jump.InputAction.canceled -= OnJumpStop;
        InputHandler.Sprint.performed -= OnSprintStart;
        InputHandler.Sprint.canceled -= OnSprintStop;
    }

    private void OnJumpStop(InputAction.CallbackContext context) {
        if (!glider.IsGliding) return;
        Animator.SetBool(GlideParameter, false);
        glider.StopGliding();
    }

    private void OnSprintStart(InputAction.CallbackContext context) {
        Runner.RunSpeed = sprintSpeed;
        Runner.Run(InputVector.x);
    }

    private void OnSprintStop(InputAction.CallbackContext context) {
        Runner.RunSpeed = runSpeed;
        Runner.Run(InputVector.x);
    }
}