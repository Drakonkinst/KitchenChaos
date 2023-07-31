using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent
{
    public static event EventHandler OnAnyObjectPlaced;

    [SerializeField] private Transform counterTopPoint;

    private KitchenObject kitchenObject;

    public virtual void Interact(Player player) {
        Debug.LogError("Base Counter interact not implemented");
    }

    public virtual void InteractAlternate(Player player) {
        // Empty
    }

    public Transform GetKitchenObjectFollowTransform() {
        return counterTopPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject, bool silent = false) {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null) {
            // If silent, does not fire this event
            if (!silent) {
                OnAnyObjectPlaced?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public KitchenObject GetKitchenObject() {
        return kitchenObject;
    }

    public void ClearKitchenObject() {
        SetKitchenObject(null);
    }

    public bool HasKitchenObject() {
        return kitchenObject != null;
    }
}
