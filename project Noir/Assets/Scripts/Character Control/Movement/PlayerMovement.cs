using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private void OnMovement(InputValue input)
    {
        var inputVector = input.Get<Vector2>();
        Debug.Log($"x: {inputVector.x}, y: {inputVector.y}");
    }
}
