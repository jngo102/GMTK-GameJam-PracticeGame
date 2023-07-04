using UnityEngine;

public class AudioPlayerPool : AbstractedObjectPool<PlayAndDespawn> {
    [SerializeField] private PlayAndDespawn audioPlayerPrefab;
    
    private void Start() {
        InitPool(audioPlayerPrefab);
    }
}
