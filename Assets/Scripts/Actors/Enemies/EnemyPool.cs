using UnityEngine;

public class EnemyPool : AbstractedObjectPool<Enemy> {
    [SerializeField] private Enemy enemyPrefab;
    
    private void Awake() {
        InitPool(enemyPrefab);
    }
}
