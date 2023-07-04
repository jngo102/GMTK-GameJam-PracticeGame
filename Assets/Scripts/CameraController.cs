using UnityEngine;

/// <summary>
///     A camera that follows a target.
/// </summary>
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Shaker))]
public class CameraController : MonoBehaviour {
    [SerializeField] private float smoothing = 0.5f;

    private new Camera camera;
    private Shaker shaker;

    private Transform target;
    private Vector3 velocity;

    public Transform Target {
        get => target;
        set {
            target = value;
            ResetPosition();
        }
    }

    private void Awake() {
        camera = GetComponent<Camera>();
        shaker = GetComponent<Shaker>();
        GameManager.Instance.LevelStarted += ResetPosition;
    }

    private void LateUpdate() {
        FollowPosition();
    }

    /// <summary>
    ///     Reset the camera to the target's position.
    /// </summary>
    private void ResetPosition() {
        FollowPosition(false);
    }

    /// <summary>
    ///     Follow target, modifying the camera's position.
    /// </summary>
    /// <param name="smooth">Whether to smooth the position change.</param>
    private void FollowPosition(bool smooth = true) {
        if (!Target) return;

        var selfTransform = transform;
        var newPosition = Target.transform.position;
        newPosition.z = selfTransform.position.z;
        selfTransform.position =
            smooth ? Vector3.SmoothDamp(selfTransform.position, newPosition, ref velocity, smoothing) : newPosition;
    }
}