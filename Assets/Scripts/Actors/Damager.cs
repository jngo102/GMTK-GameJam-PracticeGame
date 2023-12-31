using UnityEngine;

/// <summary>
///     Deals damage to an actor.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Damager : MonoBehaviour, ISpawnable {
    /// <summary>
    ///     The amount of damage to deal.
    /// </summary>
    [SerializeField] private float damageAmount = 1;

    private void OnTriggerStay2D(Collider2D other) {
        if (other.TryGetComponent<HealthManager>(out var healthManager)) healthManager.Hurt(damageAmount, this);
    }

    public void OnCreate() { }

    public void OnSpawn() { }

    public void OnDespawn() { }

    public void OnDelete() { }
}