using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private const string PLAYER_PREFS_BINDINGS = "Bindings";

    public static GameInput Instance { get; private set; }

    public enum Binding {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Interact,
        InteractAlternate,
        Pause
    }
    
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;

    private PlayerInputActions playerInputActions;

    private void Awake() {
        Instance = this;
        SetupInputActions();
    }

    private void OnDestroy() {
        // Unsubscribe input since this is not automatically destroyed when leaving the scene
        playerInputActions.Dispose();
    }

    private void SetupInputActions() {
        playerInputActions = new PlayerInputActions();

        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS)) {
            playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }

        playerInputActions.Enable();

        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;
        playerInputActions.Player.Pause.performed += Pause_performed;
    }

    private void Pause_performed(InputAction.CallbackContext context) {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        // Shorter version of a null check on OnInteractAction to check if there are subscribers
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized() {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        return inputVector.normalized;
    }

    private (InputAction inputAction, int bindingIndex) BindingToInputAction(Binding binding) {
        switch (binding) {
            case Binding.MoveUp:
                return (playerInputActions.Player.Move, 1);            
            case Binding.MoveDown:
                return (playerInputActions.Player.Move, 2);
            case Binding.MoveLeft:
                return (playerInputActions.Player.Move, 3); 
            case Binding.MoveRight:
                return (playerInputActions.Player.Move, 4);
            case Binding.Interact:
                return (playerInputActions.Player.Interact, 0);
            case Binding.InteractAlternate:
                return (playerInputActions.Player.InteractAlternate, 0);
            case Binding.Pause:
                return (playerInputActions.Player.Pause, 0);
            default:
                return (null, -1);
        }
    }

    public string GetBindingText(Binding binding) {
        (InputAction inputAction, int bindingIndex) = BindingToInputAction(binding);
        if(inputAction == null) {
            return "???";
        }
        return inputAction.bindings[bindingIndex].ToDisplayString();
    }

    public void RebindBinding(Binding binding, Action onActionRebound) {
        playerInputActions.Player.Disable();
        (InputAction inputAction, int bindingIndex) = BindingToInputAction(binding);
        inputAction.PerformInteractiveRebinding(bindingIndex)
            .OnComplete((callback) => {
                callback.Dispose();
                playerInputActions.Player.Enable();
                onActionRebound();

                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, playerInputActions.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();
            })
            .Start();
    }

    public void ResetBindings() {
        playerInputActions.Dispose();
        playerInputActions = new PlayerInputActions();
        PlayerPrefs.DeleteKey(PLAYER_PREFS_BINDINGS);
        PlayerPrefs.Save();
        SetupInputActions();
    }
}
