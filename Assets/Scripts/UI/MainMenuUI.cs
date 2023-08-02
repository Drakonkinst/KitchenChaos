using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private void Awake() {
        // Reset timescale in case paused
        Time.timeScale = 1.0f;

        playButton.onClick.AddListener(() => {
            Debug.Log("PLAY");
            OnPlayClick();
        });
        quitButton.onClick.AddListener(OnQuitClick);
    }

    private void Start() {
        // Can also use "First Selected" parameter in Event System
        playButton.Select();
    }

    private void OnPlayClick() {
        Loader.Load(Loader.Scene.GameScene);
    }

    private void OnQuitClick() {
        Application.Quit();
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
