using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    private const float SPAWN_PLATE_TIMER_MAX = 4f;
    private const int PLATES_SPAWNED_AMOUNT_MAX = 4;

    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;

    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

    private float spawnPlateTimer = 0f;
    private int platesSpawnedAmount = 0;

    private void Update() {
        spawnPlateTimer += Time.deltaTime;
        if(spawnPlateTimer > SPAWN_PLATE_TIMER_MAX) {
            spawnPlateTimer = 0f;

            if(platesSpawnedAmount < PLATES_SPAWNED_AMOUNT_MAX) {
                platesSpawnedAmount++;
                OnPlateSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public override void Interact(Player player) {
        if(platesSpawnedAmount <= 0) {
            return;
        }

        if(player.HasKitchenObject()) {
            // Attempt to put whatever player is holding on a plate and pick it up
            KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, this, true);
            KitchenObject kitchenObject = GetKitchenObject();

            bool successfullyAdded = kitchenObject.TryGetPlate(out PlateKitchenObject plateKitchenObject) && plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO());
            if (successfullyAdded) {
                // Valid time, switch player kitchen object with plate
                player.GetKitchenObject().DestroySelf();
                plateKitchenObject.SetKitchenObjectParent(player);
                platesSpawnedAmount--;
                OnPlateRemoved?.Invoke(this, EventArgs.Empty);
            } else {
                // Invalid item, clean up
                plateKitchenObject.DestroySelf();
            }
        } else {
            // Can give player a plate since they are holding nothing
            platesSpawnedAmount--;
            KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);

            // Update visuals
            OnPlateRemoved?.Invoke(this, EventArgs.Empty);
        }
    }
}
