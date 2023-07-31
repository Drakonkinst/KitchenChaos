using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public static event EventHandler OnAnyDeliverySuccess;
    public static event EventHandler OnAnyDeliveryFailure;

    new public static void ResetStaticData() {
        OnAnyDeliverySuccess = null;
        OnAnyDeliveryFailure = null;
    }

    public override void Interact(Player player) {
        // Only accept plates
        KitchenObject playerKitchenObject = player.GetKitchenObject();
        if (playerKitchenObject != null && playerKitchenObject.TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
            bool wasDeliverySuccess = DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);
            if (wasDeliverySuccess) {
                OnAnyDeliverySuccess?.Invoke(this, EventArgs.Empty);
            } else {
                OnAnyDeliveryFailure?.Invoke(this, EventArgs.Empty);
            }
            playerKitchenObject.DestroySelf();
        }
    }
}
