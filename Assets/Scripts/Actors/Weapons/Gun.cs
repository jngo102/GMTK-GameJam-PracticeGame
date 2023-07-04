using UnityEngine;

[RequireComponent(typeof(BulletPool))]
public class Gun : AttackManager {
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 10;

    private BulletPool bulletPool;

    private void Awake() {
        bulletPool = GetComponent<BulletPool>();
    }

    public override void Attack() {
        if (!CanAttack) return;

        base.Attack();

        var bullet = bulletPool.Spawn();
        bullet.BulletHit -= OnBulletHit;
        bullet.BulletHit += OnBulletHit;
        bullet.transform.position = firePoint.transform.position;
        bullet.Fire(transform.lossyScale.x, bulletSpeed);
    }

    private void OnBulletHit(Bullet bullet) {
        bulletPool.Despawn(bullet);
    }
}