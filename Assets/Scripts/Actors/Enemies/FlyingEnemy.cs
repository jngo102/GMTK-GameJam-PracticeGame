using UnityEngine;

[RequireComponent(typeof(Flyer))]
public class FlyingEnemy : MonoBehaviour {
    [SerializeField] private DetectArea detectArea;

    private Flyer flyer;

    private void Awake() {
        flyer = GetComponent<Flyer>();

        detectArea.Detected += OnDetect;
    }

    private void OnDetect(Transform detectedTransform) {
        flyer.FlyTo(detectedTransform.position);
    }
}