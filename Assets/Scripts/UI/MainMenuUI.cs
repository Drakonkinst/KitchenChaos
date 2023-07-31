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
        playButton.onClick.AddListener(OnPlayClick);
        quitButton.onClick.AddListener(OnQuitClick);
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