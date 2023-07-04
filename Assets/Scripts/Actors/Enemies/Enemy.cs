using UnityEngine;

public abstract class Enemy : MonoBehaviour {
    [SerializeField] private DetectArea detectArea;

    private void Awake() {
        detectArea.Detected += OnDetect;
    }

    protected virtual void OnDetect(Transform detectedTransform) {
        
    }
}
