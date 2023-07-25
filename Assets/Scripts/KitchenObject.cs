using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private Transform myTransform;
    private ClearCounter clearCounter;

    private void Awake() {
        myTransform = transform;
    }

    public KitchenObjectSO GetKitchenObjectSO() {
        return kitchenObjectSO;
    }

    public void SetClearCounter(ClearCounter clearCounter) {
        // Unset from previous parent if it exists
        if(this.clearCounter != null) {
            this.clearCounter.ClearKitchenObject();
        }

        this.clearCounter = clearCounter;

        if(clearCounter.HasKitchenObject()) {
            Debug.LogError("Counter already has a KitchenObject!");
        }
        clearCounter.SetKitchenObject(this);

        // Update position and hierarchy
        myTransform.parent = clearCounter.GetKitchenObjectFollowTransform();
        myTransform.localPosition = Vector3.zero;
    }

    public ClearCounter GetClearCounter() {
        return clearCounter;
    }
}
