using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
///     Controller for a player.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Grounder))]
[RequireComponent(typeof(Facer))]
[RequireComponent(typeof(Jumper))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Runner))]
public class Player : MonoBehaviour, ISpawnable {
    protected string RunParameter => "Run";
    protected string JumpParameter => "Jump";
    protected string FallParameter => "Fall";
    protected string LandParameter => "Land";
    protected string GlideParameter => "Glide";
    protected string CollapseParameter => "Collapse";
    protected string DeathParameter => "Death";
    
    /// <summary>
    ///     The input manager for this specific player instance.
    /// </summary>
    public PlayerInputHandler InputHandler { get; private set; }

    /// <inheritdoc />
    public void OnCreate() { }

    /// <inheritdoc />
    public void OnSpawn() { }

    /// <inheritdoc />
    public void OnDespawn() { }

    /// <inheritdoc />
    public void OnDelete() { }

    /// <summary>
    ///     Enable all player inputs.
    /// </summary>
    public virtual void EnableAllInputs() {
        EnableBaseInputs();
    }

    /// <summary>
    ///     Enable only the player's base inputs.
    /// </summary>
    private void EnableBaseInputs() {
        InputHandler.Jump.InputAction.performed += OnJumpStart;
        // InputHandler.Move.performed += OnMoveStart;
        // InputHandler.Move.canceled += OnMoveStop;
    }

    /// <summary>
    ///     Disable all player inputs.
    /// </summary>
    public virtual void DisableAllInputs() {
        DisableBaseInputs();
    }

    /// <summary>
    ///     Disable only the player's base inputs.
    /// </summary>
    private void DisableBaseInputs() {
        InputHandler.Jump.InputAction.performed -= OnJumpStart;
        //InputHandler.Move.performed -= OnMoveStart;
        //InputHandler.Move.canceled -= OnMoveStop;
    }

    /// <summary>
    ///     Get all components on the player.
    /// </summary>
    private void GetComponents() {
        Animator = GetComponent<Animator>();
        Body = GetComponent<Rigidbody2D>();
        Facer = GetComponent<Facer>();
        Grounder = GetComponent<Grounder>();
        Jumper = GetComponent<Jumper>();
        Runner = GetComponent<Runner>();
        InputHandler = GetComponent<PlayerInputHandler>();
    }

    /// <summary>
    ///     Initialize the values that are tracked throughout the script's execution.
    /// </summary>
    private void InitializeTrackedValues() {
        coyoteTimer = coyoteTime + 1;
    }

    /// <summary>
    ///     Perform a jump.
    /// </summary>
    public virtual void Jump() {
        if (Grounder.IsGrounded() || coyoteTimer <= coyoteTime) {
            Animator.SetBool(JumpParameter, true);
            Animator.SetBool(FallParameter, false);
            Animator.SetBool(LandParameter, false);
            Animator.SetBool(GlideParameter, false);
            coyoteTimer = coyoteTime + 1;
            Jumper.Jump();
        }
    }

    /// <summary>
    ///     Callback for when the player lands.
    /// </summary>
    private void OnLand() {
        Animator.SetBool(FallParameter, false);
        Animator.SetBool(GlideParameter, false);
        Animator.SetBool(LandParameter, true);
        if (InputHandler.IsEnabled && InputHandler.Jump.IsBuffered()) Jump();
    }

    /// <summary>
    ///     Stop all movement of the player.
    /// </summary>
    public void StopMovement() {
        Jumper.CancelJump();
        Runner.StopRun();
    }

    /// <summary>
    ///     Handle the player's coyote time.
    /// </summary>
    private void HandleCoyoteTime() {
        Jumper.StopGravity = coyoteTimer <= coyoteTime;
    }

    /// <summary>
    ///     Subscribe to events managed within the script.
    /// </summary>
    private void SubscribeEvents() {
        Jumper.Landed += OnLand;
        deathManager.Died += OnDeath;
    }

    private void OnDeath() {
        StartCoroutine(GameManager.Instance.LoadSaveSpot(SceneManager.GetActiveScene().name,
            SceneSwitcher.CurrentOuterScene));
        healthManager.FullHeal();
    }

    /// <summary>
    ///     Update values that are to be tracked.
    /// </summary>
    private void UpdateTrackedValues() {
        coyoteTimer = Mathf.Clamp(coyoteTimer + Time.deltaTime, 0, coyoteTime + 1);
    }

    private void UpdateAnimator() {
        if (Body.velocity.y <= 0 && !Animator.GetBool(FallParameter) && !Animator.GetBool(LandParameter) &&
            !Animator.GetBool(GlideParameter)) {
            if (Animator.GetBool(JumpParameter)) {
                Animator.SetBool(JumpParameter, false);   
            }
            Animator.SetBool(FallParameter, true);
        }
    }

    /// <summary>
    ///     Callback for when the player starts a jump.
    /// </summary>
    /// <param name="context">The input action callback context.</param>
    private void OnJumpStart(InputAction.CallbackContext context) {
        Jump();
    }

    /// <summary>
    ///     Callback for when the player starts moving.
    /// </summary>
    /// <param name="context">The input action callback context.</param>
    private void OnMoveStart(InputAction.CallbackContext context) {
        InputVector = context.ReadValue<Vector2>();
        Runner.Run(InputVector.x);
        Facer.CheckFlip();
    }

    /// <summary>
    ///     Callback for when the player stops moving.
    /// </summary>
    /// <param name="context">The input action callback context.</param>
    private void OnMoveStop(InputAction.CallbackContext context) {
        Runner.StopRun();
    }

    /// <summary>
    ///     Assign the player as the camera controller's current target.
    /// </summary>
    private void AssignPlayer() {
        FindObjectOfType<CameraController>(true).Target = transform;
        DontDestroyOnLoad(this);
    }

    /// <summary>
    ///     Check whether the player is grounded, accounting for coyote time.
    /// </summary>
    private void CheckGrounded() {
        if (Body.velocity.y <= 0 && Grounder.WasGrounded && !Grounder.IsGrounded()) {
            coyoteTimer = 0;
            Jumper.StopGravity = true;
        }

        if (Grounder.IsGrounded() && Mathf.Abs(Body.velocity.x) > 0) {
            if (!runParticles.isEmitting) runParticles.Play();
        }
        else {
            if (runParticles.isEmitting) runParticles.Stop();
        }
    }

    #region Exposed Values

    /// <summary>
    ///     The duration before the player actually falls and cannot perform a jump.
    /// </summary>
    [SerializeField] private float coyoteTime = 0.1f;

    [SerializeField] private ParticleSystem runParticles;
    [SerializeField] private DeathManager deathManager;
    [SerializeField] private HealthManager healthManager;

    #endregion

    #region Components

    protected Animator Animator;
    protected Rigidbody2D Body;
    protected Facer Facer;
    protected Grounder Grounder;
    protected Jumper Jumper;
    protected Runner Runner;

    #endregion

    #region Tracked Values

    private float coyoteTimer;
    protected Vector2 InputVector;

    #endregion

    #region Unity Functions

    protected virtual void Awake() {
        GetComponents();
        AssignPlayer();
        InitializeTrackedValues();
        SubscribeEvents();
    }

    protected virtual void Update() {
        HandleCoyoteTime();
        CheckGrounded();
        UpdateTrackedValues();
        UpdateAnimator();
    }

    #endregion
}