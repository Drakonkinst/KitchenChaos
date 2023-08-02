using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        defaultInitialVolumeStep = Mathf.RoundToInt(audioSource.volume * MAX_VOLUME_STEP);
        SetVolumeStep(PlayerPrefs.GetInt(PLAYER_PREFS_MUSIC_VOLUME, defaultInitialVolumeStep));
    }

    private void Start() {
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e) {
        if (KitchenGameManager.Instance.IsGamePlaying()) {
            PlayMusic();
        } else if (KitchenGameManager.Instance.IsGameOver()) {
            PauseMusic();
        }
    }

    public void ChangeVolume() {
        SetVolumeStep((currentVolumeStep + 1) % (MAX_VOLUME_STEP + 1));
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

    public void PlayMusic() {
        audioSource.Play();
    }

    public void PauseMusic() {
        audioSource.Pause();
    }
}
