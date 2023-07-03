using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
///     Controller for a player.
/// </summary>
[RequireComponent(typeof(Grounder))]
[RequireComponent(typeof(Facer))]
[RequireComponent(typeof(Jumper))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Runner))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(HealthManager))]
[RequireComponent(typeof(SquashStretchManager))]
public class Player : MonoBehaviour, ISpawnable {
    #region Exposed Values

    /// <summary>
    ///     The duration before the player actually falls and cannot perform a jump.
    /// </summary>
    [SerializeField] private float coyoteTime = 0.1f;

    [SerializeField] private ParticleSystem runParticles;
    
    #endregion

    /// <summary>
    ///     The input manager for this specific player instance.
    /// </summary>
    public PlayerInputHandler InputHandler { get; private set; }
    
    #region Components

    private Rigidbody2D body;
    private Facer facer;
    private Grounder grounder;
    private Jumper jumper;
    private Runner runner;
    private HealthManager healthManager;
    private SquashStretchManager squashStretchManager;

    #endregion

    #region Tracked Values

    private float coyoteTimer;
    private Vector2 inputVector;

    #endregion
    
    #region Unity Functions

    private void Awake() {
        GetComponents();
        AssignPlayer();
        InitializeTrackedValues();
        SubscribeEvents();
    }

    private void Update() {
        HandleCoyoteTime();
        facer.CheckFlip();
        CheckGrounded();
        UpdateTrackedValues();
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
    public void EnableAllInputs() {
        EnableBaseInputs();
    }

    /// <summary>
    ///     Enable only the player's base inputs.
    /// </summary>
    private void EnableBaseInputs() {
        InputHandler.Jump.InputAction.performed += OnJumpStart;
        InputHandler.Jump.InputAction.canceled += OnJumpStop;
        InputHandler.Move.performed += OnMoveStart;
        InputHandler.Move.canceled += OnMoveStop;
    }

    /// <summary>
    ///     Disable all player inputs.
    /// </summary>
    public void DisableAllInputs() {
        DisableBaseInputs();
    }

    /// <summary>
    ///     Disable only the player's base inputs.
    /// </summary>
    private void DisableBaseInputs() {
        InputHandler.Jump.InputAction.performed -= OnJumpStart;
        InputHandler.Jump.InputAction.canceled -= OnJumpStop;
        InputHandler.Move.performed -= OnMoveStart;
        InputHandler.Move.canceled -= OnMoveStop;
    }

    /// <summary>
    ///     Get all components on the player.
    /// </summary>
    private void GetComponents() {
        body = GetComponent<Rigidbody2D>();
        facer = GetComponent<Facer>();
        grounder = GetComponent<Grounder>();
        jumper = GetComponent<Jumper>();
        runner = GetComponent<Runner>();
        InputHandler = GetComponent<PlayerInputHandler>();
        healthManager = GetComponent<HealthManager>();
        squashStretchManager = GetComponent<SquashStretchManager>();
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
    public void Jump() {
        if (grounder.IsGrounded() || coyoteTimer <= coyoteTime) {
            coyoteTimer = coyoteTime + 1;
            jumper.Jump();
            squashStretchManager.Stretch = true;
        }
    }

    /// <summary>
    ///     Callback for when the player lands.
    /// </summary>
    private void OnLand() {
        if (InputHandler.IsEnabled && InputHandler.Jump.IsBuffered()) Jump();
        squashStretchManager.Squash = true;
        grounder.ForceGround();
    }

    /// <summary>
    ///     Stop all movement of the player.
    /// </summary>
    public void StopMovement() {
        jumper.CancelJump();
        runner.StopRun();
    }

    /// <summary>
    ///     Handle the player's coyote time.
    /// </summary>
    private void HandleCoyoteTime() {
        jumper.StopGravity = coyoteTimer <= coyoteTime;
    }

    /// <summary>
    ///     Subscribe to events managed within the script.
    /// </summary>
    private void SubscribeEvents() {
        jumper.Landed += OnLand;
    }

    /// <summary>
    ///     Update values that are to be tracked.
    /// </summary>
    private void UpdateTrackedValues() {
        coyoteTimer = Mathf.Clamp(coyoteTimer + Time.deltaTime, 0, coyoteTime + 1);
    }

    /// <inheritdoc />
    public void OnCreate() { }

    /// <inheritdoc />
    public void OnSpawn() {
        
    }

    /// <inheritdoc />
    public void OnDespawn() { }

    /// <inheritdoc />
    public void OnDelete() { }

    /// <summary>
    ///     Callback for when the player starts a jump.
    /// </summary>
    /// <param name="context">The input action callback context.</param>
    private void OnJumpStart(InputAction.CallbackContext context) {
        Jump();
    }

    /// <summary>
    ///     Callback for when the player ends a jump.
    /// </summary>
    /// <param name="context">The input action callback context.</param>
    private void OnJumpStop(InputAction.CallbackContext context) {
        jumper.CancelJump();
    }

    /// <summary>
    ///     Callback for when the player starts moving.
    /// </summary>
    /// <param name="context">The input action callback context.</param>
    private void OnMoveStart(InputAction.CallbackContext context) {
        inputVector = context.ReadValue<Vector2>();
        runner.Run(inputVector.x);
    }

    /// <summary>
    ///     Callback for when the player stops moving.
    /// </summary>
    /// <param name="context">The input action callback context.</param>
    private void OnMoveStop(InputAction.CallbackContext context) {
        runner.StopRun();
    }

    /// <summary>
    ///     Assign the player as the camera controller's current target.
    /// </summary>
    private void AssignPlayer() {
        FindObjectOfType<CameraController>(true).AddTarget(transform);
        DontDestroyOnLoad(this);
    }

    /// <summary>
    ///     Check whether the player is grounded, accounting for coyote time.
    /// </summary>
    private void CheckGrounded() {
        if (body.velocity.y <= 0 && grounder.WasGrounded && !grounder.IsGrounded()) {
            coyoteTimer = 0;
            jumper.StopGravity = true;
        }

        if (InputHandler.IsEnabled && !InputHandler.Jump.InputAction.IsPressed()) jumper.CancelJump();

        if (grounder.IsGrounded() && Mathf.Abs(body.velocity.x) > 0) {
            if (!runParticles.isEmitting) runParticles.Play();
        } else {
            if (runParticles.isEmitting) runParticles.Stop();
        }
    }
}