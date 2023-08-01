using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterAudio : MonoBehaviour
{
    private const float MIN_VOLUME = 0f;
    private const float MAX_VOLUME = 1f;

    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private float audioFadePerSecond = 2f;

    private AudioSource audioSource;
    private bool audioShouldPlay = false;
    private float volumeMultiplier = 1f;
    private float volume;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        volume = MIN_VOLUME;
        stoveCounter.OnFryingStateChanged += StoveCounter_OnFryingStateChanged;
    }

    private void Start() {
        SoundManager.Instance.OnVolumeChanged += SoundManager_OnVolumeChanged;
    }

    private void Update() {
        if(audioShouldPlay) {
            volume += audioFadePerSecond * Time.deltaTime;
        } else {
            volume -= audioFadePerSecond * Time.deltaTime;
        }

        if(volume <= MIN_VOLUME && audioSource.isPlaying) {
            audioSource.Pause();
        } else if(volume > MIN_VOLUME && !audioSource.isPlaying) {
            audioSource.Play();
        }

        volume = Mathf.Clamp(volume, MIN_VOLUME, MAX_VOLUME);
        audioSource.volume = volume * volumeMultiplier; 
    }

    private void SoundManager_OnVolumeChanged(object sender, SoundManager.OnVolumeChangedEventArgs e) {
        volumeMultiplier = e.normalizedVolume;
    }

    private void StoveCounter_OnFryingStateChanged(object sender, StoveCounter.OnFryingStateChangedEventArgs e) {
        if (e.isFrying) {
            //audioSource.Play();
            audioShouldPlay = true;
        } else {
            //audioSource.Pause();
            audioShouldPlay = false;
        }
    }
}
