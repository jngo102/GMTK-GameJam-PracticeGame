using UnityEngine;

[RequireComponent(typeof(HealthManager))]
[RequireComponent(typeof(DeathManager))]
public abstract class Enemy : MonoBehaviour, ISpawnable {
    [SerializeField] private DetectArea detectArea;

    private HealthManager healthManager;
    private DeathManager deathManager;
    
    protected virtual void Awake() {
        detectArea.Detected += OnDetect;

        healthManager = GetComponent<HealthManager>();
        deathManager = GetComponent<DeathManager>();
        deathManager.Died += OnDeath;
    }

    private void OnDeath(DeathManager _) {
        Destroy(gameObject);
    }

    protected virtual void OnDetect(Transform detectedTransform) {
        
    }

    public void OnCreate() {
        
    }

    public void OnSpawn() {
       healthManager.FullHeal(); 
    }

    public void OnDespawn() {
        
    }

    public void OnDelete() {
        
    }
}
