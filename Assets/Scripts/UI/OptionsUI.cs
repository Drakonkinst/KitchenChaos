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
    private const string GAMEPAD_MOVEMENT_STRING = "Left Joystick";

    [SerializeField] private Button effectsVolumeButton;
    [SerializeField] private Button musicVolumeButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI effectsVolumeText;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private Transform pressToRebindKeyTransform;
    [SerializeField] private Button resetAllButton;

    [Header("Controls Buttons")]
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button interactAltButton;
    [SerializeField] private Button pauseButton;

    [Header("Controls Text")]
    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI interactAltText;
    [SerializeField] private TextMeshProUGUI pauseText;

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

        moveUpButton.onClick.AddListener(() => {
            RebindBinding(GameInput.Binding.MoveUp, GameInput.Binding.None);
        });
        moveDownButton.onClick.AddListener(() => {
            RebindBinding(GameInput.Binding.MoveDown, GameInput.Binding.None);
        });
        moveLeftButton.onClick.AddListener(() => {
            RebindBinding(GameInput.Binding.MoveLeft, GameInput.Binding.None);
        });
        moveRightButton.onClick.AddListener(() => {
            RebindBinding(GameInput.Binding.MoveRight, GameInput.Binding.None);
        });
        interactButton.onClick.AddListener(() => {
            RebindBinding(GameInput.Binding.Interact, GameInput.Binding.Gamepad_Interact);
        });
        interactAltButton.onClick.AddListener(() => {
            RebindBinding(GameInput.Binding.InteractAlternate, GameInput.Binding.Gamepad_InteractAlternate);
        });
        pauseButton.onClick.AddListener(() => {
            RebindBinding(GameInput.Binding.Pause, GameInput.Binding.Gamepad_Pause);
        });

        resetAllButton.onClick.AddListener(() => {
            ResetAllOptions();
        });
    }

    private void Start() {
        KitchenGameManager.Instance.OnGameUnpaused += KitchenGameManager_OnGameUnpaused;
        GameInput.Instance.OnControlSchemeChanged += GameInput_OnControlSchemeChanged;

        UpdateVisuals();
        Hide();
    }

    private void GameInput_OnControlSchemeChanged(object sender, EventArgs e) {
        UpdateVisuals();
    }

    private void KitchenGameManager_OnGameUnpaused(object sender, EventArgs e) {
        Hide();
    }

    private void UpdateVisuals() {
        effectsVolumeText.text = "Effects Volume: " + SoundManager.Instance.GetVolumeStep() + " / " + MAX_VOLUME;
        musicVolumeText.text = "Music Volume: " + MusicManager.Instance.GetVolumeStep() + " / " + MAX_VOLUME;

        if(GameInput.Instance.IsUsingKeyboard()) {
            moveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveUp);
            moveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveDown);
            moveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveLeft);
            moveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveRight);
            interactText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
            interactAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlternate);
            pauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);

            moveUpButton.enabled = true;
            moveDownButton.enabled = true;
            moveLeftButton.enabled = true;
            moveRightButton.enabled = true;
        } else if(GameInput.Instance.IsUsingGamepad()) {
            moveUpText.text = GAMEPAD_MOVEMENT_STRING;
            moveDownText.text = GAMEPAD_MOVEMENT_STRING;
            moveLeftText.text = GAMEPAD_MOVEMENT_STRING;
            moveRightText.text = GAMEPAD_MOVEMENT_STRING;
            interactText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Interact);
            interactAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_InteractAlternate);
            pauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Pause);

            moveUpButton.enabled = false;
            moveDownButton.enabled = false;
            moveLeftButton.enabled = false;
            moveRightButton.enabled = false;
        }
    }

    public void Show() {
        gameObject.SetActive(true);
        closeButton.Select();
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    private void ShowPressToRebindKey() {
        pressToRebindKeyTransform.gameObject.SetActive(true);
    }

    private void HidePressToRebindKey() {
        pressToRebindKeyTransform.gameObject.SetActive(false);
    }

    private void RebindBinding(GameInput.Binding bindingIfKeyboard, GameInput.Binding bindingIfGamepad) {
        GameInput.Binding binding = GameInput.Instance.IsUsingGamepad() ? bindingIfGamepad : bindingIfKeyboard;
        bool wasBindable = GameInput.Instance.RebindBinding(binding, () => {
            HidePressToRebindKey();
            UpdateVisuals();
        });

        if (wasBindable) {
            ShowPressToRebindKey();
        }
    }

    private void ResetAllOptions() {
        GameInput.Instance.ResetBindings();
        SoundManager.Instance.ResetVolume();
        MusicManager.Instance.ResetVolume();
        UpdateVisuals();
    }
}
