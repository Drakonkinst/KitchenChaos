using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayingClockUI : MonoBehaviour
{
    [SerializeField] private Image timerImage;

    private void Update() {
        float fillAmount = 1 - KitchenGameManager.Instance.GetGamePlayingTimerNormalized();
        timerImage.fillAmount = fillAmount;
    }
}
