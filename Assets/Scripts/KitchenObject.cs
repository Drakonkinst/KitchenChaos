using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private Transform myTransform;
    private IKitchenObjectParent kitchenObjectParent;

    private void Awake() {
        myTransform = transform;
    }

    public KitchenObjectSO GetKitchenObjectSO() {
        return kitchenObjectSO;
    }

    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent) {
        // Unset from previous parent if it exists
        if(this.kitchenObjectParent != null) {
            this.kitchenObjectParent.ClearKitchenObject();
        }

        this.kitchenObjectParent = kitchenObjectParent;

        if(kitchenObjectParent.HasKitchenObject()) {
            Debug.LogError("Kitchen object parent already has a KitchenObject!");
        }
        kitchenObjectParent.SetKitchenObject(this);

        // Update position and hierarchy
        myTransform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform();
        myTransform.localPosition = Vector3.zero;
    }

    public IKitchenObjectParent GetKitchenObjectParent() {
        return kitchenObjectParent;
    }
}
