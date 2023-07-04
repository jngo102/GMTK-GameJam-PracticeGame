using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Glider : MonoBehaviour {
    [SerializeField] private float glidingGravityScale = 1;
    [SerializeField] private float fallingGravityScale = 5;
    private Rigidbody2D body;

    private Jumper jumper;

    public bool IsGliding { get; private set; }

    private void Awake() {
        body = GetComponent<Rigidbody2D>();
        jumper = GetComponent<Jumper>();
    }

    public void StartGliding() {
        if (jumper) jumper.ControllingGravity = false;
        IsGliding = true;
        body.gravityScale = glidingGravityScale;
    }

    public void StopGliding() {
        if (jumper) jumper.ControllingGravity = true;
        IsGliding = false;
        body.gravityScale = fallingGravityScale;
    }
}