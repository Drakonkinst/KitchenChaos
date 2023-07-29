using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter {
    public override void Interact(Player player) {
        bool hasKitchenObject = HasKitchenObject();
        bool playerHasKitchenObject = player.HasKitchenObject();

        if (!hasKitchenObject && playerHasKitchenObject) {
            // Player puts kitchen object on counter
            player.GetKitchenObject().SetKitchenObjectParent(this);
        } else if (hasKitchenObject && !playerHasKitchenObject) {
            // Player takes kitchen object from counter
            GetKitchenObject().SetKitchenObjectParent(player);
        } else if (hasKitchenObject && playerHasKitchenObject) {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                // Player is holding a plate
                KitchenObject kitchenObject = GetKitchenObject();
                bool successfullyAdded = plateKitchenObject.TryAddIngredient(kitchenObject.GetKitchenObjectSO());
                if (successfullyAdded) {
                    kitchenObject.DestroySelf();
                }
            } else if (GetKitchenObject().TryGetPlate(out plateKitchenObject)) {
                // Plate is on counter
                KitchenObject kitchenObject = player.GetKitchenObject();
                bool successfullyAdded = plateKitchenObject.TryAddIngredient(kitchenObject.GetKitchenObjectSO());
                if (successfullyAdded) {
                    kitchenObject.DestroySelf();
                }
            }
        }
    }
}
