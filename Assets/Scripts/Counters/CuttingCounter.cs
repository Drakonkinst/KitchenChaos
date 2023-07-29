using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgressBar
{
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
                cuttingProgress = 0;
                
                KitchenObjectSO inputKitchenObjectSO = GetKitchenObject().GetKitchenObjectSO();
                CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOForInput(inputKitchenObjectSO);
                UpdateProgress(cuttingRecipeSO.cuttingProgressMax);
            }
        } else if (hasKitchenObject && !playerHasKitchenObject) {
            // Player takes kitchen object from counter
            GetKitchenObject().SetKitchenObjectParent(player);
        } else if (hasKitchenObject && playerHasKitchenObject && player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
            // Player is holding a plate
            KitchenObject kitchenObject = GetKitchenObject();
            bool successfullyAdded = plateKitchenObject.TryAddIngredient(kitchenObject.GetKitchenObjectSO());
            if (successfullyAdded) {
                kitchenObject.DestroySelf();
            }
        }
    }

    public override void InteractAlternate(Player player) {
        Debug.Log("Interact Alternate");
        if (HasKitchenObject()) {
            KitchenObjectSO inputKitchenObjectSO = GetKitchenObject().GetKitchenObjectSO();
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOForInput(inputKitchenObjectSO);

            // Only cut if valid recipe found
            if(cuttingRecipeSO != null) {
                // Progress cutting
                cuttingProgress++;

                UpdateProgress(cuttingRecipeSO.cuttingProgressMax);
                OnCut?.Invoke(this, EventArgs.Empty);

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

    private void UpdateProgress(float cuttingProgressMax) {
        OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventArgs {
            progressNormalized = (cuttingProgress * 1.0f) / cuttingProgressMax
        });
    }
}

