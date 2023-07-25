using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    public class OnSelectedCounterChangedEventArgs : EventArgs {
        public ClearCounter selectedCounter;
    }

    private const float PLAYER_RADIUS = 0.65f;
    private const float PLAYER_HEIGHT = 2.0f;
    private const float INTERACT_DISTANCE = 2.0f;

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;

    private Transform myTranform;
    private bool isWalking = false;
    private Vector3 lastInteractDir;
    private ClearCounter selectedCounter;

    private void Awake() {
        if(Instance != null) {
            Debug.LogError("More than 1 Player instance");
        }
        Instance = this;
        myTranform = transform;
    }

    private void Start() {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e) {
        if(selectedCounter != null) {
            selectedCounter.Interact();
        }
    }

    private void Update() {
        HandleMovement();
        HandleInteractions();
    }

    private void HandleInteractions() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if(moveDir != Vector3.zero) {
            lastInteractDir = moveDir;
        }
        bool wasHit = Physics.Raycast(myTranform.position, lastInteractDir, out RaycastHit raycastHit, INTERACT_DISTANCE, countersLayerMask);
        if(wasHit) {
            // Shorter alternative to a null check on GetComponent<>()
            if(raycastHit.transform.TryGetComponent(out ClearCounter clearCounter)) {
                // Has ClearCounter
                SetSelectedCounter(clearCounter);
            } else {
                SetSelectedCounter(null);
            }
        } else {
            SetSelectedCounter(null);
        }
    }

    private void HandleMovement() {
        // Get input vector
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        // If we want the character to use a walking animation even when stopped by a wall
        //isWalking = inputVector != Vector2.zero;

        // Move character
        float moveDistance = moveSpeed * Time.deltaTime;
        Vector3 moveDir = CalcMoveDir(inputVector, moveDistance, out bool canMove);
        if (canMove) {
            myTranform.position += moveDir * moveDistance;
        }
        isWalking = canMove && moveDir != Vector3.zero;

        // Rotate character to direction of movement
        // Use Slerp for rotation, just Lerp for movement
        myTranform.forward = Vector3.Slerp(myTranform.forward, moveDir, Time.deltaTime * turnSpeed);
    }

    // Get direction of movement after wall hugging. Returns null vector if cannot move.
    private Vector3 CalcMoveDir(Vector2 inputVector, float moveDistance, out bool canMove) {

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        canMove = true;

        if(CanMoveInDir(moveDir, moveDistance)) {
            return moveDir;
        }

        // Cannot move towards moveDir, attempt only X movement
        Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f).normalized;
        if(CanMoveInDir(moveDirX, moveDistance)) {
            return moveDirX;
        }

        // Cannot move towards X, attempt only Z movement
        Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z).normalized;
        if(CanMoveInDir(moveDirZ, moveDistance)) {
            return moveDirZ;
        }

        // Cannot move
        canMove = false;
        return moveDir;
    }

    private bool CanMoveInDir(Vector3 moveDir, float moveDistance) {
        return !Physics.CapsuleCast(myTranform.position, myTranform.position + Vector3.up * PLAYER_HEIGHT, PLAYER_RADIUS, moveDir, moveDistance);
    }

    private void SetSelectedCounter(ClearCounter selectedCounter) {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
            selectedCounter = selectedCounter
        });
    }

    public bool IsWalking() {
        return isWalking;
    }
}
