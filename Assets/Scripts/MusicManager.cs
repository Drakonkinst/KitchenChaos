using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundManager;

public class MusicManager : MonoBehaviour
{
    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";

    private const int MAX_VOLUME_STEP = 10;
    private const float VOLUME_STEP_TO_VOLUME = 0.1f;

    public static MusicManager Instance { get; private set; }

    private AudioSource audioSource;
    private int currentVolumeStep;
    private int defaultInitialVolumeStep;

    private void Awake() {
        Instance = this;
        audioSource = GetComponent<AudioSource>();

        defaultInitialVolumeStep = (int) Mathf.Round(audioSource.volume * MAX_VOLUME_STEP);
        SetVolumeStep(PlayerPrefs.GetInt(PLAYER_PREFS_MUSIC_VOLUME, defaultInitialVolumeStep));
    }

    public void ChangeVolume() {
        SetVolumeStep((currentVolumeStep + 1) % MAX_VOLUME_STEP);
    }

    public void ResetVolume() {
        SetVolumeStep(defaultInitialVolumeStep);
    }

    private void SetVolumeStep(int volumeStep) {
        currentVolumeStep = volumeStep;
        audioSource.volume = currentVolumeStep * VOLUME_STEP_TO_VOLUME;
        PlayerPrefs.SetInt(PLAYER_PREFS_MUSIC_VOLUME, currentVolumeStep);
        PlayerPrefs.Save();
    }

    public int GetVolumeStep() {
        return currentVolumeStep;
    }
}
