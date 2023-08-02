using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI keyboardMoveText;
    [SerializeField] private TextMeshProUGUI keyboardInteractText;
    [SerializeField] private TextMeshProUGUI keyboardInteractAltText;
    [SerializeField] private TextMeshProUGUI keyboardPauseText;
    [SerializeField] private TextMeshProUGUI gamepadMoveText;
    [SerializeField] private TextMeshProUGUI gamepadInteractText;
    [SerializeField] private TextMeshProUGUI gamepadInteractAltText;
    [SerializeField] private TextMeshProUGUI gamepadPauseText;

    private void Start() {
        UpdateVisuals();
        GameInput.Instance.OnBindingRebind += GameInput_OnBindingRebind;
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
        Show();
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e) {
        if (KitchenGameManager.Instance.IsCountdownToStartActive()) {
            Hide();
        }
    }

    private void GameInput_OnBindingRebind(object sender, System.EventArgs e) {
        UpdateVisuals();
    }

    private void UpdateVisuals() {
        string moveUpText = GameInput.Instance.GetBindingText(GameInput.Binding.MoveUp);
        string moveDownText = GameInput.Instance.GetBindingText(GameInput.Binding.MoveDown);
        string moveLeftText = GameInput.Instance.GetBindingText(GameInput.Binding.MoveLeft);
        string moveRightText = GameInput.Instance.GetBindingText(GameInput.Binding.MoveRight);

        keyboardMoveText.text = moveUpText + ", " + moveLeftText + ", " + moveDownText + ", " + moveRightText;
        keyboardInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        keyboardInteractAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlternate);
        keyboardPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);

        // Gamepad movement never changes
        gamepadInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Interact);
        gamepadInteractAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_InteractAlternate);
        gamepadPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Pause);
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
