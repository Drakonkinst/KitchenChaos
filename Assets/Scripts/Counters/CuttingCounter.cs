using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgressBar
{
    private const float DUMMY_MAX_PROGRESS = 1f;

    public static event EventHandler OnAnyCounterCut;

    public event EventHandler<IHasProgressBar.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;

    [SerializeField] private CuttingRecipeSO[] cuttingRecipesSOArray;

    private int cuttingProgress = 0;

    public override void Interact(Player player) {
        bool hasKitchenObject = HasKitchenObject();
        bool playerHasKitchenObject = player.HasKitchenObject();

        if (!hasKitchenObject && playerHasKitchenObject) {
            // Player puts kitchen object on counter
            KitchenObject kitchenObject = player.GetKitchenObject();

            // Only place if valid cuttable object
            if(HasRecipeForInput(kitchenObject.GetKitchenObjectSO())) {
                kitchenObject.SetKitchenObjectParent(this);
                ClearProgress();
            }
        } else if (hasKitchenObject && !playerHasKitchenObject) {
            // Player takes kitchen object from counter
            GetKitchenObject().SetKitchenObjectParent(player);
            ClearProgress();
        } else if (hasKitchenObject && playerHasKitchenObject && player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
            // Player is holding a plate
            KitchenObject kitchenObject = GetKitchenObject();
            bool successfullyAdded = plateKitchenObject.TryAddIngredient(kitchenObject.GetKitchenObjectSO());
            if (successfullyAdded) {
                kitchenObject.DestroySelf();
                ClearProgress();
            }
        }
    }

    public override void InteractAlternate(Player player) {
        if (HasKitchenObject()) {
            KitchenObjectSO inputKitchenObjectSO = GetKitchenObject().GetKitchenObjectSO();
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOForInput(inputKitchenObjectSO);

            // Only cut if valid recipe found
            if(cuttingRecipeSO != null) {
                // Progress cutting
                cuttingProgress++;

                UpdateProgress(cuttingRecipeSO.cuttingProgressMax);
                OnCut?.Invoke(this, EventArgs.Empty);
                OnAnyCounterCut?.Invoke(this, EventArgs.Empty);

                // Replace object if cutting is finished
                if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax) {
                    KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(inputKitchenObjectSO);
                    GetKitchenObject().DestroySelf();
                    KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
                }
            }
        }
    }

    private bool HasRecipeForInput(KitchenObjectSO inputKitchenObjectSO) {
        return GetCuttingRecipeSOForInput(inputKitchenObjectSO) != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOForInput(inputKitchenObjectSO);
        if(cuttingRecipeSO != null) {
            return cuttingRecipeSO.output;
        }
        return null;
    }

    private CuttingRecipeSO GetCuttingRecipeSOForInput(KitchenObjectSO inputKitchenObjectSO) {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipesSOArray) {
            if (inputKitchenObjectSO == cuttingRecipeSO.input) {
                return cuttingRecipeSO;
            }
        }
        return null;
    }

    private void ClearProgress() {
        cuttingProgress = 0;
        // Max progress doesn't matter when cutting progress is 0
        // If we want to know the max progress at all times (like a segmented bar), would be better to cache to current cutting recipe
        UpdateProgress(DUMMY_MAX_PROGRESS);
    }

    private void UpdateProgress(float cuttingProgressMax) {
        OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventArgs {
            progressNormalized = (cuttingProgress * 1.0f) / cuttingProgressMax
        });
    }
}

