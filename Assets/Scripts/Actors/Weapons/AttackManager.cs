using UnityEngine;

public abstract class AttackManager : MonoBehaviour {
    [SerializeField] private float cooldown = 0.25f;

    private float attackTimer;

    public bool CanAttack => attackTimer >= cooldown;

    private void Update() {
        attackTimer += Time.deltaTime;
    }

    public virtual void Attack() {
        attackTimer = 0;
    }
}