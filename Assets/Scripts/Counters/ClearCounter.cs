using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player) {
        if(!HasKitchenObject() && player.HasKitchenObject()) {
            // Player puts kitchen object on counter
            player.GetKitchenObject().SetKitchenObjectParent(this);
        } else if(HasKitchenObject() && !player.HasKitchenObject()) {
            // Player takes kitchen object from counter
            GetKitchenObject().SetKitchenObjectParent(player);
        }
    }
}
