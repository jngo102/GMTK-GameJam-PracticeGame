using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(EnemyPool))]
public class Organ : MonoBehaviour {
    [SerializeField] private float spawnRadius = 8;
    [SerializeField] private int spawnMin = 3;
    [SerializeField] private int spawnMax = 7;

    private Collider2D collider;
    private EnemyPool enemyPool;

    private readonly List<Enemy> aliveEnemies = new();

    private void Awake() {
        enemyPool = GetComponent<EnemyPool>();
        collider = GetComponent<Collider2D>();
    }

    public void Infect() {
        collider.enabled = false;
        aliveEnemies.Clear();
        for (var spawnNum = 0; spawnNum <= Random.Range(spawnMin, spawnMax + 1); spawnNum++) {
            var enemy = enemyPool.Spawn();
            enemy.transform.position = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
            aliveEnemies.Add(enemy);
            enemy.GetComponent<DeathManager>().Died -= OnEnemyDeath;
            enemy.GetComponent<DeathManager>().Died += OnEnemyDeath;
        }
    }

    private void OnEnemyDeath(DeathManager deathManager) {
        aliveEnemies.Remove(deathManager.GetComponent<Enemy>());
        if (aliveEnemies.Count <= 0) {
            Uninfect();
        }
    }

    public void Uninfect() {
        collider.enabled = true;
    }
}
