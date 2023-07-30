using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player) {
        if(!player.HasKitchenObject()) {
            GiveKitchenObjectToPlayer(player);
        }
        if(player.HasKitchenObject() && player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
            bool successfullyAdded = plateKitchenObject.TryAddIngredient(kitchenObjectSO);
            if(successfullyAdded) {
                OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private void GiveKitchenObjectToPlayer(Player player) {
        KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
        OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
    }
}
