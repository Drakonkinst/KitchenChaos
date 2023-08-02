using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter, IHasProgressBar
{
    private const float WARNING_PROGRESS_THRESHOLD = 0.5f;

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
        bool hasKitchenObject = HasKitchenObject();
        bool playerHasKitchenObject = player.HasKitchenObject();

        if (!hasKitchenObject && playerHasKitchenObject) {
            // Player puts kitchen object on counter
            KitchenObject kitchenObject = player.GetKitchenObject();

            // Only place if valid fryable object
            if (HasRecipeForInput(kitchenObject.GetKitchenObjectSO())) {
                kitchenObject.SetKitchenObjectParent(this);
                currentFryingRecipeSO = GetFryingRecipeSOForInput(GetKitchenObject().GetKitchenObjectSO());
                SetFryingTimer(0f);
                UpdateIsFrying();
            }
        } else if (hasKitchenObject && !playerHasKitchenObject) {
            // Player takes kitchen object from counter
            GetKitchenObject().SetKitchenObjectParent(player);

            // Reset current state
            currentFryingRecipeSO = null;
            SetFryingTimer(0f);
            UpdateIsFrying();
        } else if (hasKitchenObject && playerHasKitchenObject && player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
            // Player is holding a plate
            KitchenObject kitchenObject = GetKitchenObject();
            bool successfullyAdded = plateKitchenObject.TryAddIngredient(kitchenObject.GetKitchenObjectSO());
            if (successfullyAdded) {
                kitchenObject.DestroySelf();

                // Reset current state
                currentFryingRecipeSO = null;
                SetFryingTimer(0f);
                UpdateIsFrying();
            }
        }
    }

    private void IncrementFryingTimer() {
        SetFryingTimer(fryingTimer + Time.deltaTime);
        if (fryingTimer > currentFryingRecipeSO.fryingTimeSeconds) {
            // Fried
            SetFryingTimer(0f);
            GetKitchenObject().DestroySelf();
            KitchenObject.SpawnKitchenObject(currentFryingRecipeSO.output, this, true);
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
        bool isFrying = IsFrying();
        OnFryingStateChanged?.Invoke(this, new OnFryingStateChangedEventArgs {
            isFrying = isFrying
        });
    }

    private void SetFryingTimer(float fryingTimer) {
        this.fryingTimer = fryingTimer;

        OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventArgs {
            progressNormalized = GetProgressNormalized()
        });
    }

    // Progress is 0 if there is no frying recipe, otherwise base it off the max seconds
    private float GetProgressNormalized() {
        if (currentFryingRecipeSO == null) {
            return 0f;
        }
        return Mathf.Clamp01(fryingTimer / currentFryingRecipeSO.fryingTimeSeconds);
    }

    private bool IsFrying() {
        return currentFryingRecipeSO != null;
    }

    public bool IsBurning() {
        return IsFrying() && currentFryingRecipeSO.burning && GetProgressNormalized() >= WARNING_PROGRESS_THRESHOLD;
    }
}
