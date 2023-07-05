using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Glider))]
public class OuterPlayer : Player {
    [SerializeField] private float runSpeed = 5;
    [SerializeField] private float sprintSpeed = 8;
    [SerializeField] private float deathPause = 5;
    
    private Glider glider;

    private bool doubleJumpRefreshed;
    
    private bool canDoubleJump = true;
    public bool CanDoubleJump {
        get => canDoubleJump;
        set {
            canDoubleJump = value;
            if (!canDoubleJump) CheckDeath();
        }
    }

    private bool canGlide = true;

    public bool CanGlide {
        get => canGlide;
        set {
            canGlide = value;
            if (!canGlide) CheckDeath();
        } 
    }
    
    private bool canSprint = true;
    public bool CanSprint {
        get => canSprint;
        set {
            canSprint = value;
            if (canSprint) {
                InputHandler.Sprint.performed += OnSprintStart;
                InputHandler.Sprint.canceled += OnSprintStop;
            }
            else {
                InputHandler.Sprint.performed -= OnSprintStart;
                InputHandler.Sprint.canceled -= OnSprintStop;
                Runner.RunSpeed = runSpeed;
                CheckDeath();
            }
        }
    }

    protected override void Awake() {
        base.Awake();

        glider = GetComponent<Glider>();
        
        Jumper.Landed += OnLand;
    }

    protected override void Update() {
        base.Update();

        InputVector = InputHandler.Move.ReadValue<Vector2>();
        if (Mathf.Abs(InputVector.x) > 0.1f) {
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

        if (CanGlide && Body.velocity.y <= 0 && InputHandler.Jump.InputAction.IsPressed() && 
            !InputHandler.Jump.IsBuffered() && !glider.IsGliding) { 
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

    private void CheckDeath() {
        if (!CanDoubleJump && !CanGlide && !CanSprint) {
            StartCoroutine(CustomDeath());
        }
    }

    private IEnumerator CustomDeath() {
        Animator.SetBool(CollapseParameter, true);
        Animator.SetBool(DeathParameter, true);
        InputHandler.Disable();
        
        float pauseTimer = 0;
        while (pauseTimer < deathPause) {
            pauseTimer += Time.deltaTime;
            yield return null;
        }
        
        yield return SceneSwitcher.UnloadOuterScene();
        InputHandler.Enable();
        GameManager.Instance.LoadScene("MainMenu", SceneType.Inner);
        CanDoubleJump = CanGlide = CanSprint = true;
        gameObject.SetActive(false);
    }

    public override void DisableAllInputs() {
        base.DisableAllInputs();

        InputHandler.Jump.InputAction.canceled -= OnJumpStop;
        InputHandler.Sprint.performed -= OnSprintStart;
        InputHandler.Sprint.canceled -= OnSprintStop;
    }

    private void OnLand() {
        doubleJumpRefreshed = true;
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

    public override void Jump() {
        base.Jump();

        if (!Grounder.IsGrounded() && canDoubleJump) {
            DoubleJump();
        }
    }
    
    public void DoubleJump() {
        if (!CanDoubleJump || !doubleJumpRefreshed) return;
        Animator.SetBool(JumpParameter, true);
        Animator.SetBool(FallParameter, false);
        Animator.SetBool(GlideParameter, false);
        doubleJumpRefreshed = false;
        Jumper.Jump();
    }
}