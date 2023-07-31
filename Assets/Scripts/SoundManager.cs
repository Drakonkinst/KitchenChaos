using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioClipRefsSO audioClipRefsSO;

    private void Awake() {
        SoundManager.Instance = this;
    }

    private void Start() {
        DeliveryCounter.OnAnyDeliverySuccess += DeliveryCounter_OnAnyDeliverySuccess;
        DeliveryCounter.OnAnyDeliveryFailure += DeliveryManager_OnRecipeFailure;
        CuttingCounter.OnAnyCounterCut += CuttingCounter_OnAnyCounterCut;
        Player.Instance.OnPickedUpObject += Player_OnPickedUpObject;
        BaseCounter.OnAnyObjectPlaced += BaseCounter_OnAnyObjectPlaced;
        TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
    }

    public void PlayFootstepSound(Vector3 position, float volume) {
        PlaySound(audioClipRefsSO.footstep, position, volume);
    }

    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f) {
        AudioClip randomSound = audioClipArray[UnityEngine.Random.Range(0, audioClipArray.Length)];
        PlaySound(randomSound, position, volume);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f) {
        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }

    private void TrashCounter_OnAnyObjectTrashed(object sender, EventArgs e) {
        TrashCounter trashCounter = sender as TrashCounter;
        PlaySound(audioClipRefsSO.trash, trashCounter.transform.position);
    }

    private void BaseCounter_OnAnyObjectPlaced(object sender, EventArgs e) {
        BaseCounter baseCounter = sender as BaseCounter;
        PlaySound(audioClipRefsSO.objectDrop, baseCounter.transform.position);
    }

    private void Player_OnPickedUpObject(object sender, EventArgs e) {
        PlaySound(audioClipRefsSO.objectPickup, Player.Instance.transform.position);
    }

    private void CuttingCounter_OnAnyCounterCut(object sender, EventArgs e) {
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        PlaySound(audioClipRefsSO.chop, cuttingCounter.transform.position);
    }

    private void DeliveryCounter_OnAnyDeliverySuccess(object sender, EventArgs e) {
        DeliveryCounter deliveryCounter = sender as DeliveryCounter;
        PlaySound(audioClipRefsSO.deliverySuccess, deliveryCounter.transform.position);
    }

    private void DeliveryManager_OnRecipeFailure(object sender, EventArgs e) {
        DeliveryCounter deliveryCounter = sender as DeliveryCounter;
        PlaySound(audioClipRefsSO.deliveryFailed, deliveryCounter.transform.position);
    }
}
