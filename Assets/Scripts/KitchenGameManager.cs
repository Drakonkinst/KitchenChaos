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

    private const float WAIT_TO_START_SECONDS_MAX = 1f;
    private const float COUNTDOWN_TO_START_SECONDS_MAX = 3f;
    private const float GAME_PLAYING_SECONDS_MAX = 64f;
    private const float RECIPE_SUCCESS_BONUS = 8f;
    private const float RECIPE_FAILURE_PENALTY = 8f;

    private enum GameState {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }

    private GameState gameState;
    private float waitingToStartTimer = WAIT_TO_START_SECONDS_MAX;
    private float countdownToStartTimer = COUNTDOWN_TO_START_SECONDS_MAX;
    private float gamePlayingTimer = GAME_PLAYING_SECONDS_MAX;
    private float timeSurvived = 0f;
    private bool isGamePaused = false;

    private void Awake() {
        Instance = this;
        SetGameState(GameState.WaitingToStart);
    }

    private void Start() {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        DeliveryCounter.OnAnyDeliverySuccess += DeliveryCounter_OnAnyDeliverySuccess;
        DeliveryCounter.OnAnyDeliveryFailure += DeliveryCounter_OnAnyDeliveryFailure;
    }

    private void DeliveryCounter_OnAnyDeliverySuccess(object sender, EventArgs e) {
        gamePlayingTimer = Mathf.Clamp(gamePlayingTimer + RECIPE_SUCCESS_BONUS, 0f, GAME_PLAYING_SECONDS_MAX);
    }

    private void DeliveryCounter_OnAnyDeliveryFailure(object sender, EventArgs e) {
        if (gamePlayingTimer < RECIPE_FAILURE_PENALTY) {
            // No penalty if the penalty would instantly end the game
            return;
        }
        gamePlayingTimer = Mathf.Clamp(gamePlayingTimer - RECIPE_FAILURE_PENALTY, 0f, GAME_PLAYING_SECONDS_MAX);
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
        timeSurvived += Time.deltaTime;
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
        return 1 - (gamePlayingTimer / GAME_PLAYING_SECONDS_MAX);
    }

    public bool IsGamePaused() {
        return isGamePaused;
    }

    public float GetTimeSurvived() {
        return timeSurvived;
    }
}
