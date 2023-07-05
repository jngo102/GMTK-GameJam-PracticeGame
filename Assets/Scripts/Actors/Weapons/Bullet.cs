using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : Damager {
    public delegate void OnBulletHit(Bullet bullet);

    [SerializeField] private float despawnTime = 3;

    private Rigidbody2D body;

    private float despawnTimer;

    private void Awake() {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        despawnTimer += Time.deltaTime;
        if (despawnTimer >= despawnTime) BulletCollide();
    }

    private void OnCollisionEnter2D(Collision2D other) {
        BulletCollide();
    }

    public event OnBulletHit BulletHit;

    public void Fire(float direction, float speed) {
        despawnTimer = 0;
        var selfTransform = transform;
        var scale = selfTransform.localScale;
        scale = new Vector3(direction * scale.x, scale.y, scale.z);
        selfTransform.localScale = scale;
        body.velocity = Vector2.right * (direction * speed);
    }

    private void BulletCollide() {
        BulletHit?.Invoke(this);
    }
}