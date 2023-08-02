using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour
{
    private const string DELIVERY_RESULT_TRIGGER = "DeliveryResult";

    [SerializeField] private DeliveryCounter deliveryCounter;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Color successColor;
    [SerializeField] private Color failureColor;
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failureSprite;

    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        DeliveryCounter.OnAnyDeliverySuccess += DeliveryCounter_OnAnyDeliverySuccess;
        DeliveryCounter.OnAnyDeliveryFailure += DeliveryCounter_OnAnyDeliveryFailure;
        gameObject.SetActive(false);
    }

    private void DeliveryCounter_OnAnyDeliverySuccess(object sender, System.EventArgs e) {
        if (sender as DeliveryCounter != deliveryCounter) {
            return;
        }

        backgroundImage.color = successColor;
        iconImage.sprite = successSprite;
        messageText.text = "DELIVERY\nSUCCESS";
        gameObject.SetActive(true);
        animator.SetTrigger(DELIVERY_RESULT_TRIGGER);
    }

    private void DeliveryCounter_OnAnyDeliveryFailure(object sender, System.EventArgs e) {
        if (sender as DeliveryCounter != deliveryCounter) {
            return;
        }

        backgroundImage.color = failureColor;
        iconImage.sprite = failureSprite;
        messageText.text = "DELIVERY\nFAILED";
        gameObject.SetActive(true);
        animator.SetTrigger(DELIVERY_RESULT_TRIGGER);
    }

}
