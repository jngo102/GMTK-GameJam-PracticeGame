using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DetectArea : MonoBehaviour {
    public delegate void OnDetect(Transform detectedTransform);

    public event OnDetect Detected;
    
    private void OnTriggerStay2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            Detected?.Invoke(other.transform);            
        }
    }
}
