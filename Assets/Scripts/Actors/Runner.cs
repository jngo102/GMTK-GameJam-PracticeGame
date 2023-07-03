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
    [SerializeField] private float runSpeed = 5;

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

    /// <summary>
    ///     Perform horizontal movement.
    /// </summary>
    /// <param name="direction">The direction that the actor runs in; negative means to the left, positive to the right.</param>
    public void Run(float direction) {
        var velocityX = direction * runSpeed;
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