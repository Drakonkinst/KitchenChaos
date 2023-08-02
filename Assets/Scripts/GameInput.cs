using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private const string PLAYER_PREFS_BINDINGS = "Bindings";
    private const string CONTROL_SCHEME_KEYBOARD = "Keyboard";
    private const string CONTROL_SCHEME_GAMEPAD = "Gamepad";

    public static GameInput Instance { get; private set; }

    public enum Binding {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Interact,
        InteractAlternate,
        Pause,
        Gamepad_Interact,
        Gamepad_InteractAlternate,
        Gamepad_Pause,
        None
    }
    
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;
    public event EventHandler OnControlSchemeChanged;
    public event EventHandler OnBindingRebind;

    private PlayerInput playerInput;    // This is used solely to determine active control scheme, no events attached
    private PlayerInputActions playerInputActions;

    private void Awake() {
        Instance = this;
        playerInput = GetComponent<PlayerInput>();
        playerInput.onControlsChanged += PlayerInput_onControlsChanged;
        SetupInputActions();
    }

    private void PlayerInput_onControlsChanged(PlayerInput obj) {
        OnControlSchemeChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy() {
        // Unsubscribe input since this is not automatically destroyed when leaving the scene
        playerInputActions.Dispose();
    }

    private void Update() {
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
            case Binding.Gamepad_Interact:
                return (playerInputActions.Player.Interact, 1);
            case Binding.Gamepad_InteractAlternate:
                return (playerInputActions.Player.InteractAlternate, 1);
            case Binding.Gamepad_Pause:
                return (playerInputActions.Player.Pause, 1);
            default:
                return (null, -1);
        }
    }

    public string GetBindingText(Binding binding) {
        (InputAction inputAction, int bindingIndex) = BindingToInputAction(binding);
        if(inputAction == null) {
            return "-";
        }
        return inputAction.bindings[bindingIndex].ToDisplayString();
    }

    public bool RebindBinding(Binding binding, Action onActionRebound) {
        if(binding == Binding.None) {
            return false;
        }

        playerInputActions.Player.Disable();
        (InputAction inputAction, int bindingIndex) = BindingToInputAction(binding);
        string bindingGroupName = IsUsingGamepad() ? CONTROL_SCHEME_GAMEPAD : CONTROL_SCHEME_KEYBOARD;
        inputAction.PerformInteractiveRebinding(bindingIndex)
            .WithBindingGroup(bindingGroupName)
            .OnComplete((callback) => {
                callback.Dispose();
                playerInputActions.Player.Enable();
                onActionRebound();

                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, playerInputActions.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();

                OnBindingRebind?.Invoke(this, EventArgs.Empty);
            })
            .Start();
        return true;
    }

    public void ResetBindings() {
        playerInputActions.Dispose();
        playerInputActions = new PlayerInputActions();
        PlayerPrefs.DeleteKey(PLAYER_PREFS_BINDINGS);
        PlayerPrefs.Save();
        SetupInputActions();
    }

    public bool IsUsingGamepad() {
        return playerInput.currentControlScheme == CONTROL_SCHEME_GAMEPAD;
    }

    public bool IsUsingKeyboard() {
        return playerInput.currentControlScheme == CONTROL_SCHEME_KEYBOARD;
    }
}
