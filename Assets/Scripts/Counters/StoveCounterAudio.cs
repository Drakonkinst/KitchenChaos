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

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = MIN_VOLUME;
        stoveCounter.OnFryingStateChanged += StoveCounter_OnFryingStateChanged;
    }

    private void Update() {
        float newVolume = audioSource.volume;
        if(audioShouldPlay) {
            newVolume += audioFadePerSecond * Time.deltaTime;
        } else {
            newVolume -= audioFadePerSecond * Time.deltaTime;
        }

        if(newVolume <= MIN_VOLUME && audioSource.isPlaying) {
            audioSource.Pause();
        } else if(newVolume > MIN_VOLUME && !audioSource.isPlaying) {
            audioSource.Play();
        }

        audioSource.volume = Mathf.Clamp(newVolume, MIN_VOLUME, MAX_VOLUME);
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
