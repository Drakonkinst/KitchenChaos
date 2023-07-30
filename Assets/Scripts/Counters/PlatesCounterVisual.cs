using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{
    private const float PLATE_OFFSET_Y = 0.1f;

    [SerializeField] private PlatesCounter platesCounter;
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private Transform plateVisualPrefab;

    private List<GameObject> plateVisualGameObjectList;

    private void Awake() {
        plateVisualGameObjectList = new List<GameObject>();
        platesCounter.OnPlateSpawned += PlatesCounter_OnPlateSpawned;
        platesCounter.OnPlateRemoved += PlatesCounter_OnPlateRemoved;
    }

    private void PlatesCounter_OnPlateRemoved(object sender, EventArgs e) {
        // Destroy the last plate
        GameObject plateGameObjectToDestroy = plateVisualGameObjectList[plateVisualGameObjectList.Count - 1];
        plateVisualGameObjectList.Remove(plateGameObjectToDestroy);
        Destroy(plateGameObjectToDestroy);
    }

    private void PlatesCounter_OnPlateSpawned(object sender, EventArgs e) {
        Transform plateVisualTransform = Instantiate(plateVisualPrefab, counterTopPoint);
        plateVisualTransform.localPosition = new Vector3(0f, PLATE_OFFSET_Y * plateVisualGameObjectList.Count, 0f);
        plateVisualGameObjectList.Add(plateVisualTransform.gameObject);
    }
}
