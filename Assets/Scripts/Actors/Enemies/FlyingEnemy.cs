using UnityEngine;

[RequireComponent(typeof(Flyer))]
public class FlyingEnemy : Enemy {
    private Flyer flyer;

    protected override void Awake() {
        base.Awake();
        
        flyer = GetComponent<Flyer>();
    }

    protected override void OnDetect(Transform detectedTransform) {
        flyer.FlyTo(detectedTransform.position);
    }
}