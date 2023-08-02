using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterAudio : MonoBehaviour
{
    private const float MIN_VOLUME = 0f;
    private const float MAX_VOLUME = 1f;
    private const float WARNING_SOUND_TIMER_MAX = 0.2f;

    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private float audioFadePerSecond = 2f;

    private AudioSource audioSource;
    private bool audioShouldPlay = false;
    private float volumeMultiplier = 1f;
    private float volume;
    private float warningSoundTimer;
    private bool warningShouldPlay = false;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        volume = MIN_VOLUME;
    }

    private void Start() {
        stoveCounter.OnFryingStateChanged += StoveCounter_OnFryingStateChanged;
        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
        SoundManager.Instance.OnVolumeChanged += SoundManager_OnVolumeChanged;
    }

    private void StoveCounter_OnProgressChanged(object sender, IHasProgressBar.OnProgressChangedEventArgs e) {
        warningShouldPlay = stoveCounter.IsBurning();
    }

    private void Update() {
        HandlePanFryingSound();

        if (warningShouldPlay) {
            HandleBurnWarningSound();
        }
    }

    private void HandlePanFryingSound() {
        if (audioShouldPlay) {
            volume += audioFadePerSecond * Time.deltaTime;
        } else {
            volume -= audioFadePerSecond * Time.deltaTime;
        }

        if (volume <= MIN_VOLUME && audioSource.isPlaying) {
            audioSource.Pause();
        } else if (volume > MIN_VOLUME && !audioSource.isPlaying) {
            audioSource.Play();
        }

        volume = Mathf.Clamp(volume, MIN_VOLUME, MAX_VOLUME);
        audioSource.volume = volume * volumeMultiplier;
    }

    private void HandleBurnWarningSound() {
        warningSoundTimer -= Time.deltaTime;
        if (warningSoundTimer <= 0f)
        {
            SoundManager.Instance.PlayWarningSound(stoveCounter.transform.position);
            warningSoundTimer = WARNING_SOUND_TIMER_MAX;
        }
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
