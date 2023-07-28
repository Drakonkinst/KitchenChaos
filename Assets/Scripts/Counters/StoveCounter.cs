using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter
{
    [SerializeField] private FryingRecipeSO[] fryingRecipesSOArray;

    private float fryingTimer = 0f;
    private FryingRecipeSO currentFryingRecipeSO;

    private void Update() {
        if (HasKitchenObject()) {
            fryingTimer += Time.deltaTime;
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
                fryingTimer = 0f;
            }
        } else if (HasKitchenObject() && !player.HasKitchenObject()) {
            // Player takes kitchen object from counter
            GetKitchenObject().SetKitchenObjectParent(player);
            currentFryingRecipeSO = null;
            fryingTimer = 0f;
        }
    }

    private void IncrementFryingTimer() {
        if (fryingTimer > currentFryingRecipeSO.fryingTimeSeconds) {
            // Fried
            fryingTimer = 0f;
            GetKitchenObject().DestroySelf();
            KitchenObject.SpawnKitchenObject(currentFryingRecipeSO.output, this);
            currentFryingRecipeSO = GetFryingRecipeSOForInput(GetKitchenObject().GetKitchenObjectSO());
        }
        Debug.Log(fryingTimer);
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
}
