using UnityEngine;

[RequireComponent(typeof(AudioPlayerPool))]
[RequireComponent(typeof(AudioSource))]
public class AudioManager : Singleton<AudioManager> {
    private AudioPlayerPool audioPlayerPool;
    private AudioSource musicSource;

    protected override void OnAwake() {
        audioPlayerPool = GetComponent<AudioPlayerPool>();
        musicSource = GetComponent<AudioSource>();
    }
    
    public void PlayMusic(AudioClip clip) {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void SpawnAndPlay(AudioClip clip, Vector3 spawnPosition) {
        var audioPlayer = audioPlayerPool.Spawn();
        audioPlayer.transform.position = spawnPosition;
        audioPlayer.Clip = clip;
    }

    public void DespawnPlayer(PlayAndDespawn audioPlayer) {
        audioPlayerPool.Despawn(audioPlayer);
    }
}
