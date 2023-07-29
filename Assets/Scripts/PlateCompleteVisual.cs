using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    [Serializable]
    public struct KitchenObjectSO_GameObject {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject gameObject;
    }

    [SerializeField] PlateKitchenObject plateKitchenObject;
    [SerializeField] KitchenObjectSO_GameObject[] kitchenObjectSOGameObjectArray;

    // Start is called before the first frame update
    private void Start()
    {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;

        foreach (KitchenObjectSO_GameObject kitchenObjectSOGameObject in kitchenObjectSOGameObjectArray) {
            kitchenObjectSOGameObject.gameObject.SetActive(false);
        }
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e) {
        GameObject matchingGameObject = GetGameObjectForKitchenObjectSO(e.kitchenObjectSO);
        if(matchingGameObject == null) {
            Debug.LogError("Cannot find matching game object for " + e.kitchenObjectSO.name);
            return;
        }
        matchingGameObject.SetActive(true);
    }

    private GameObject GetGameObjectForKitchenObjectSO(KitchenObjectSO kitchenObjectSO) {
        foreach (KitchenObjectSO_GameObject kitchenObjectSOGameObject in kitchenObjectSOGameObjectArray) {
            if (kitchenObjectSOGameObject.kitchenObjectSO == kitchenObjectSO) {
                return kitchenObjectSOGameObject.gameObject;
            }
        }
        return null;
    }
}
