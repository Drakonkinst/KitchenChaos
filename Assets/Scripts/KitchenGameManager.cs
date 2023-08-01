using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenGameManager : MonoBehaviour
{
    public static KitchenGameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;

    private const float WAIT_TO_START_SECONDS = 0.5f;
    private const float COUNTDOWN_TO_START_SECONDS = 3f;
    private const float GAME_PLAYING_SECONDS = 2 * 60f;

    private enum GameState {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }

    private GameState gameState;
    private float waitingToStartTimer = WAIT_TO_START_SECONDS;
    private float countdownToStartTimer = COUNTDOWN_TO_START_SECONDS;
    private float gamePlayingTimer = GAME_PLAYING_SECONDS;
    private bool isGamePaused = false;

    private void Awake() {
        Instance = this;
        SetGameState(GameState.WaitingToStart);
    }

    private void Start() {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e) {
        if(isGamePaused) {
            UnpauseGame();
        } else {
            PauseGame();
        }
    }

    private void Update() {
        switch (gameState) {
            case GameState.WaitingToStart:
                HandleWaitingToStartState();
                break;
            case GameState.CountdownToStart:
                HandleCountdownToStartState();
                break;
            case GameState.GamePlaying:
                HandleGamePlayingState();
                break;
            case GameState.GameOver:
                HandleGameOverState();
                break;
        }
    }

    private void HandleWaitingToStartState() {
        waitingToStartTimer -= Time.deltaTime;
        if (waitingToStartTimer < 0f) {
            SetGameState(GameState.CountdownToStart);
        }
    }

    private void HandleCountdownToStartState() {
        countdownToStartTimer -= Time.deltaTime;
        if (countdownToStartTimer < 0f) {
            SetGameState(GameState.GamePlaying);
        }
    }

    private void HandleGamePlayingState() {
        gamePlayingTimer -= Time.deltaTime;
        if (gamePlayingTimer < 0f) {
            SetGameState(GameState.GameOver);
        }
    }

    private void HandleGameOverState() {
        // Do nothing
    }

    public void PauseGame() {
        OnGamePaused?.Invoke(this, EventArgs.Empty);
        Time.timeScale = 0f;
        isGamePaused = true;
    }

    public void UnpauseGame() {
        OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        Time.timeScale = 1f;
        isGamePaused = false;
    }

    private void SetGameState(GameState gameState) {
        this.gameState = gameState;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
        Debug.Log(gameState);
    }

    public bool IsGamePlaying() {
        return gameState == GameState.GamePlaying && !IsGamePaused();
    }

    public bool IsCountdownToStartActive() {
        return gameState == GameState.CountdownToStart;
    }

    public bool IsGameOver() {
        return gameState == GameState.GameOver;
    }

    public float GetCountdownToStartTimer() {
        return countdownToStartTimer;
    }

    // Return progress of the game playing timer, with 0.0 being started and 1.0 being finished
    public float GetGamePlayingTimerNormalized() {
        return 1 - (gamePlayingTimer / GAME_PLAYING_SECONDS);
    }

    public bool IsGamePaused() {
        return isGamePaused;
    }
}
