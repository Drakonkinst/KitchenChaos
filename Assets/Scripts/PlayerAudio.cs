using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    private const float FOOTSTEP_TIMER_MAX = 0.1f;
    private const float FOOTSTEP_VOLUME = 0.5f;

    private Player player;
    private float footstepTimer;
    
    private void Awake() {
        player = GetComponent<Player>();
    }

    private void Update() {
        footstepTimer -= Time.deltaTime;
        if(footstepTimer < 0f) {
            footstepTimer = FOOTSTEP_TIMER_MAX;
            
            if(player.IsWalking()) {
                SoundManager.Instance.PlayFootstepSound(player.transform.position, FOOTSTEP_VOLUME);
            }
        }
    }
}
