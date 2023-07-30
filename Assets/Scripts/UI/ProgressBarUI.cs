using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private Image barImage;
    [SerializeField] private GameObject hasProgressBarGameObject;

    private IHasProgressBar hasProgressBar;

    private void Start() {
        hasProgressBar = hasProgressBarGameObject.GetComponent<IHasProgressBar>();
        if(hasProgressBar == null) {
            Debug.LogError("Game object " + hasProgressBarGameObject + " does not have component that implements IHasProgress!");
        }

        hasProgressBar.OnProgressChanged += HasProgressBar_OnProgressChanged;
        barImage.fillAmount = 0f;
        Hide();
    }

    private void HasProgressBar_OnProgressChanged(object sender, IHasProgressBar.OnProgressChangedEventArgs e) {
        barImage.fillAmount = e.progressNormalized;
        if (barImage.fillAmount <= 0.0f || barImage.fillAmount >= 1.0f) {
            Hide();
        } else {
            Show();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
