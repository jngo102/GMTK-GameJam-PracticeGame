using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     A camera that follows a target.
/// </summary>
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Shaker))]
public class CameraController : MonoBehaviour {
    [SerializeField] private float smoothing = 0.5f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float minZoom = 40;
    [SerializeField] private float maxZoom = 10;
    [SerializeField] private float zoomLimit = 50;

    private Transform target;
    public Transform Target {
        get => target;
        set {
            target = value;
            ResetPosition();
        }
    }
    
    private new Camera camera;
    private Shaker shaker;
    private Vector3 velocity;
    
    private void Awake() {
        camera = GetComponent<Camera>();
        shaker = GetComponent<Shaker>();
        GameManager.Instance.LevelStarted += ResetPosition;
    }

    private void LateUpdate() {
        FollowPosition();
    }

    /// <summary>
    ///     Reset the camera to the targets' positions.
    /// </summary>
    private void ResetPosition() {
        FollowPosition(false);
    }
    
    /// <summary>
    ///     Follow targets, modifying the camera's position.
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