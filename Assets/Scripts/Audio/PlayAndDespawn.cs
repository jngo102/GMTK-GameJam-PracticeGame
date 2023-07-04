using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayAndDespawn : MonoBehaviour, ISpawnable {
    private AudioSource audioSource;

    private AudioClip clip;
    public AudioClip Clip {
        get => clip;
        set {
            clip = value;
            StartCoroutine(Play());
        }
    }

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    private IEnumerator Play() {
        audioSource.PlayOneShot(Clip);
        yield return new WaitWhile(() => audioSource.isPlaying);
        AudioManager.Instance.DespawnPlayer(this);
    }

    public void OnCreate() {
    }

    public void OnSpawn() {
    }

    public void OnDespawn() {
        audioSource.clip = null;
    }

    public void OnDelete() {
    }
}
