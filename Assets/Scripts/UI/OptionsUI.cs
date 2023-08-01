using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }

    private const int MAX_VOLUME = 10;

    [SerializeField] private Button effectsVolumeButton;
    [SerializeField] private Button musicVolumeButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI effectsVolumeText;
    [SerializeField] private TextMeshProUGUI musicVolumeText;

    private void Awake() {
        Instance = this;
        effectsVolumeButton.onClick.AddListener(() => {
            SoundManager.Instance.ChangeVolume();
            UpdateVisuals();
        });
        musicVolumeButton.onClick.AddListener(() => {
            MusicManager.Instance.ChangeVolume();
            UpdateVisuals();
        });
        closeButton.onClick.AddListener(() => {
            Hide();
            GamePauseUI.Instance.Show();
        });
    }

    private void Start() {
        KitchenGameManager.Instance.OnGameUnpaused += KitchenGameManager_OnGameUnpaused;

        UpdateVisuals();
        Hide();
    }

    private void KitchenGameManager_OnGameUnpaused(object sender, EventArgs e) {
        Hide();
    }

    private void UpdateVisuals() {
        effectsVolumeText.text = "Effects Volume: " + SoundManager.Instance.GetVolumeStep() + " / " + MAX_VOLUME;
        musicVolumeText.text = "Music Volume: " + MusicManager.Instance.GetVolumeStep() + " / " + MAX_VOLUME;
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
