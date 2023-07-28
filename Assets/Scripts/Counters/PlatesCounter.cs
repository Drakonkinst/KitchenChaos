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
        if(!player.HasKitchenObject() && platesSpawnedAmount > 0) {
            // Can give player a plate
            platesSpawnedAmount--;
            KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);

            // Update visuals
            OnPlateRemoved?.Invoke(this, EventArgs.Empty);
        }
    }
}
