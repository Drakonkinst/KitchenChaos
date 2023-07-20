using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private GameInput gameInput;

    private bool isWalking = false;

    private void Update() {
        // Get input vector
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        // Move character
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        isWalking = moveDir != Vector3.zero;
        transform.position += moveDir * Time.deltaTime * moveSpeed;

        // Rotate character to direction of movement
        // Use Slerp for rotation, just Lerp for movement
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * turnSpeed);
    }

    public bool IsWalking() {
        return isWalking;
    }
}
