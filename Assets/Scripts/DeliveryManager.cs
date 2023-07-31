using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;

    public static DeliveryManager Instance { get; private set; }

    private const int WAITING_RECIPES_MAX = 4;
    private const float SPAWN_RECIPE_TIMER_MAX = 8f;
    private const float SPAWN_RECIPE_START_DELAY = 3f;

    [SerializeField] RecipeListSO recipeListSO;

    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer;
    private int successfulRecipesAmount = 0;

    private void Awake() {
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
        spawnRecipeTimer = SPAWN_RECIPE_START_DELAY;
    }

    private void Update() {
        if (!KitchenGameManager.Instance.IsGamePlaying()) {
            return;
        }

        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f) {
            spawnRecipeTimer = SPAWN_RECIPE_TIMER_MAX;

            if (waitingRecipeSOList.Count < WAITING_RECIPES_MAX) {
                int randomIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[randomIndex];
                waitingRecipeSOList.Add(waitingRecipeSO);

                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    // Return true if successful
    public bool DeliverRecipe(PlateKitchenObject plateKitchenObject) {
        for (int i = 0; i < waitingRecipeSOList.Count; ++i) {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            // Check if different number of ingredients
            if(waitingRecipeSO.kitchenObjectSOList.Count != plateKitchenObject.GetKitchenObjectSOList().Count) {
                continue;
            }

            // Cycle through all ingredients in recipe
            bool plateContentsMatchesRecipe = true;
            foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList) {
                // Cycle through all ingredients in plate
                bool ingredientFound = false;
                foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
                    if (plateKitchenObjectSO == recipeKitchenObjectSO) {
                        // Match found
                        ingredientFound = true;
                        break;
                    }
                }
                if(!ingredientFound) {
                    // Recipe is missing this ingredient
                    plateContentsMatchesRecipe = false;
                    break;
                }
            }

            if(plateContentsMatchesRecipe) {
                // Yay! Player delivered correct recipe

                // Remove it from the list
                // No need for backwards iteration here since there is only one removal at most
                waitingRecipeSOList.RemoveAt(i);
                successfulRecipesAmount++;

                OnRecipeCompleted?.Invoke(this, EventArgs.Empty);

                // Stop at the first one, so does not count for multiple of the same order
                return true;
            }
        }

        // Player did not deliver correct recipe
        return false;
    }

    public List<RecipeSO> GetWaitingRecipeSOList() {
        return waitingRecipeSOList;
    }
    
    public int GetSuccessfulRecipesAmount() {
        return successfulRecipesAmount;
    }
}
