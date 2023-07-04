using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Glider : MonoBehaviour {
    [SerializeField] private float glidingGravityScale = 1;
    [SerializeField] private float fallingGravityScale = 5;
    
    public bool IsGliding { get; private set; }
    
    private Jumper jumper;
    private Rigidbody2D body;
    
    private void Awake() {
        body = GetComponent<Rigidbody2D>();
        jumper = GetComponent<Jumper>();
    }

    public void StartGliding() {
        if (jumper) jumper.enabled = false;
        IsGliding = true;
        body.gravityScale = glidingGravityScale;
    }

    public void StopGliding() {
        if (jumper) jumper.enabled = true;
        IsGliding = false;
        body.gravityScale = fallingGravityScale;
    }
}