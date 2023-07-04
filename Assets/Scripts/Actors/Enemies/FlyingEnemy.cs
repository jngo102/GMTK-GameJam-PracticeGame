using UnityEngine;

[RequireComponent(typeof(Flyer))]
public class FlyingEnemy : Enemy {
    private Flyer flyer;

    private void Awake() {
        flyer = GetComponent<Flyer>();
    }

    protected override void OnDetect(Transform detectedTransform) {
        flyer.FlyTo(detectedTransform.position);
    }
}