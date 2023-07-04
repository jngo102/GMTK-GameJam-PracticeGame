using System;
using UnityEngine;

public abstract class AttackManager : MonoBehaviour {
    [SerializeField] private float cooldown = 0.25f;
    
    public bool CanAttack => attackTimer >= cooldown;

    private float attackTimer;

    private void Update() {
        attackTimer += Time.deltaTime;
    }

    public virtual void Attack() {
        attackTimer = 0;
    }
}
