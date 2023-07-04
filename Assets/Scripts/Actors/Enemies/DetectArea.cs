using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DetectArea : MonoBehaviour {
    public delegate void OnDetect(Transform detectedTransform);

    private void OnTriggerStay2D(Collider2D other) {
        if (other.CompareTag("Player")) Detected?.Invoke(other.transform);
    }

    public event OnDetect Detected;
}