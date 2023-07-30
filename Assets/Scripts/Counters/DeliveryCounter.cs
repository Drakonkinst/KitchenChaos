using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public override void Interact(Player player) {
        // Only accept plates
        KitchenObject playerKitchenObject = player.GetKitchenObject();
        if (playerKitchenObject != null && playerKitchenObject.TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
            playerKitchenObject.DestroySelf();
        }
    }
}
