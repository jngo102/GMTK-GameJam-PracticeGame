using UnityEngine;

[RequireComponent(typeof(Facer))]
[RequireComponent(typeof(Rigidbody2D))]
public class Flyer : MonoBehaviour {
    [SerializeField] private float flyToSpeed = 5;
    [SerializeField] private float idleFlySpeed = 2;
    [SerializeField] private float idleFlyRandomness = 1;

    private Rigidbody2D body;

    private float currentFlySpeed;
    private Facer facer;
    private Vector3 flyTarget;
    private float idleFlyTimer;

    private void Awake() {
        body = GetComponent<Rigidbody2D>();
        facer = GetComponent<Facer>();
    }

    private void Start() {
        IdleFly();
    }

    private void Update() {
        var selfTransform = transform;
        var selfPosition = selfTransform.position;
        var distance = (flyTarget - selfPosition).normalized;
        facer.FaceDirection(distance.x);
        body.velocity = distance * currentFlySpeed;
        if ((selfPosition - flyTarget).magnitude < 0.1f) IdleFly();
    }

    public void IdleFly() {
        currentFlySpeed = idleFlySpeed;
        var selfTransform = transform;
        var selfPosition = selfTransform.position;
        flyTarget = new Vector3(
            selfPosition.x + Random.Range(-idleFlyRandomness, idleFlyRandomness),
            selfPosition.y + Random.Range(-idleFlyRandomness, idleFlyRandomness),
            selfPosition.z
        );
    }

    public void FlyTo(Vector3 position) {
        currentFlySpeed = flyToSpeed;
        flyTarget = position;
    }
}