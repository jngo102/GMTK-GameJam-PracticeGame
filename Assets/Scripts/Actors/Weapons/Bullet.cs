using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : Damager {
    [SerializeField] private float despawnTime = 3;
    public delegate void OnBulletHit(Bullet bullet);
    public event OnBulletHit BulletHit;

    private Rigidbody2D body;
    
    private float despawnTimer;
    private bool fired;

    private void Awake() {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        despawnTimer += Time.deltaTime;
        if (despawnTimer >= despawnTime) {
            BulletCollide();
        }
    }

    public void Fire(float direction, float speed) {
        despawnTimer = 0;
        fired = true;
        var selfTransform = transform;
        var scale = selfTransform.localScale;
        scale = new Vector3(direction * scale.x, scale.y, scale.z);
        selfTransform.localScale = scale;
        body.velocity = Vector2.right * (direction * speed);
    }
    
    private void OnCollisionEnter2D(Collision2D other) {
        BulletCollide();
    }

    private void BulletCollide() {
        fired = false;
        BulletHit?.Invoke(this);
    }
}
