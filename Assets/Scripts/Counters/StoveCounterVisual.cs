using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private GameObject stoveOnGameObject;
    [SerializeField] private GameObject particlesGameObject;

    private void Start() {
        stoveCounter.OnFryingStateChanged += StoveCounter_OnFryingStateChanged;
    }

    private void StoveCounter_OnFryingStateChanged(object sender, StoveCounter.OnFryingStateChangedEventArgs e) {
        bool showVisual = e.isFrying;
        stoveOnGameObject.SetActive(showVisual);
        particlesGameObject.SetActive(showVisual);
    }
}
