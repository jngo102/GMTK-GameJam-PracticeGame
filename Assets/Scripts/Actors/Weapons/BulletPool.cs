using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : AbstractedObjectPool<Bullet> {
    [SerializeField] private Bullet bulletPrefab;
    
    private void Awake() {
        InitPool(bulletPrefab);
    }
}
