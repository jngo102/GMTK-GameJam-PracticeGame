using System.Collections;
using UnityEngine;

/// <summary>
///     Manages horizontal movement for the actor.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Facer))]
public class Runner : MonoBehaviour {
    public delegate void OnAutoRunFinish(Runner runner);

    /// <summary>
    ///     The speed at which the actor runs.
    /// </summary>
    public float RunSpeed = 5;

    [SerializeField] float acceleration;
    [SerializeField] float deceleration;
    [SerializeField] float velPower;

    private Rigidbody2D body;
    private Facer facer;

    private void Awake() {
        body = GetComponent<Rigidbody2D>();
        facer = GetComponent<Facer>();
    }

    /// <summary>
    ///     Raised when the actor has finished running to a horizontal position.
    /// </summary>
    public event OnAutoRunFinish AutoRunFinished;

    public void SmoothRun(float direction)
    {
        float targetSpeed = direction * RunSpeed;
        float speedDiff = targetSpeed - body.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velPower) * Mathf.Sign(speedDiff);

        body.AddForce(movement * Vector2.right);
    }

    /// <summary>
    ///     Perform horizontal movement.
    /// </summary>
    /// <param name="direction">The direction that the actor runs in; negative means to the left, positive to the right.</param>
    public void Run(float direction) {
        var velocityX = direction * RunSpeed;
        body.velocity = new Vector2(velocityX, body.velocity.y);
        var scaleX = transform.localScale.x;
        if ((scaleX < 0 && velocityX > 0) || (scaleX > 0 && velocityX < 0)) facer.Flip();
    }

    /// <summary>
    ///     Stop running.
    /// </summary>
    public void StopRun() {
        body.velocity = new Vector2(0, body.velocity.y);
    }

    /// <summary>
    ///     Run to a target x position.
    /// </summary>
    /// <param name="targetX">The x position to run to.</param>
    public void RunTo(float targetX) {
        IEnumerator DoRunTo() {
            var distance = targetX - transform.position.x;
            var direction = Mathf.Sign(distance);
            Run(direction);

            if (distance < 0)
                yield return new WaitUntil(() => transform.position.x <= targetX);
            else if (distance > 0) yield return new WaitUntil(() => transform.position.x >= targetX);

            AutoRunFinished?.Invoke(this);
        }

        StartCoroutine(DoRunTo());
    }
}