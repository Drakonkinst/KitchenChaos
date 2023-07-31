using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenGameManager : MonoBehaviour
{
    public static KitchenGameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;

    private const float WAIT_TO_START_SECONDS = 1f;
    private const float COUNTDOWN_TO_START_SECONDS = 3f;
    private const float GAME_PLAYING_SECONDS = 10f;

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

    private void Awake() {
        Instance = this;
        SetGameState(GameState.WaitingToStart);
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

    private void SetGameState(GameState gameState) {
        this.gameState = gameState;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
        Debug.Log(gameState);
    }

    public bool IsGamePlaying() {
        return gameState == GameState.GamePlaying;
    }

    public bool IsCountdownToStartActive() {
        return gameState == GameState.CountdownToStart;
    }

    public float GetCountdownToStartTimer() {
        return countdownToStartTimer;
    }
}
