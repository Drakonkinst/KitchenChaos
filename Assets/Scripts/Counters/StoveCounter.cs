using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter, IHasProgressBar
{
    public event EventHandler<OnFryingStateChangedEventArgs> OnFryingStateChanged;
    public event EventHandler<IHasProgressBar.OnProgressChangedEventArgs> OnProgressChanged;
    
    public class OnFryingStateChangedEventArgs : EventArgs {
        public bool isFrying;
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipesSOArray;

    private float fryingTimer = 0f;
    private FryingRecipeSO currentFryingRecipeSO;

    private void Update() {
        if (HasKitchenObject()) {
            if(currentFryingRecipeSO != null ) {
                IncrementFryingTimer();
            }
        }
    }

    public override void Interact(Player player) {
        if (!HasKitchenObject() && player.HasKitchenObject()) {
            // Player puts kitchen object on counter
            KitchenObject kitchenObject = player.GetKitchenObject();

            // Only place if valid fryable object
            if (HasRecipeForInput(kitchenObject.GetKitchenObjectSO())) {
                kitchenObject.SetKitchenObjectParent(this);
                currentFryingRecipeSO = GetFryingRecipeSOForInput(GetKitchenObject().GetKitchenObjectSO());
                SetFryingTimer(0f);
                UpdateIsFrying();
            }
        } else if (HasKitchenObject() && !player.HasKitchenObject()) {
            // Player takes kitchen object from counter
            GetKitchenObject().SetKitchenObjectParent(player);
            currentFryingRecipeSO = null;
            SetFryingTimer(0f);
            UpdateIsFrying();
        }
    }

    private void IncrementFryingTimer() {
        SetFryingTimer(fryingTimer + Time.deltaTime);
        if (fryingTimer > currentFryingRecipeSO.fryingTimeSeconds) {
            // Fried
            SetFryingTimer(0f);
            GetKitchenObject().DestroySelf();
            KitchenObject.SpawnKitchenObject(currentFryingRecipeSO.output, this);
            currentFryingRecipeSO = GetFryingRecipeSOForInput(GetKitchenObject().GetKitchenObjectSO());
            UpdateIsFrying();
        }
    }

    private bool HasRecipeForInput(KitchenObjectSO inputKitchenObjectSO) {
        return GetFryingRecipeSOForInput(inputKitchenObjectSO) != null;
    }

    private FryingRecipeSO GetFryingRecipeSOForInput(KitchenObjectSO inputKitchenObjectSO) {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipesSOArray) {
            if (inputKitchenObjectSO == fryingRecipeSO.input) {
                return fryingRecipeSO;
            }
        }
        return null;
    }

    private void UpdateIsFrying() {
        OnFryingStateChanged?.Invoke(this, new OnFryingStateChangedEventArgs {
            isFrying = IsFrying()
        });
    }

    private void SetFryingTimer(float fryingTimer) {
        this.fryingTimer = fryingTimer;

        // Progress is 0 if there is no frying recipe, otherwise base it off the max seconds
        float progressNormalized = 0f;
        if (currentFryingRecipeSO != null) {
            progressNormalized = Mathf.Clamp01(fryingTimer / currentFryingRecipeSO.fryingTimeSeconds);
        }

        OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventArgs {
            progressNormalized = progressNormalized
        });
    }

    private bool IsFrying() {
        return currentFryingRecipeSO != null;
    }
}
