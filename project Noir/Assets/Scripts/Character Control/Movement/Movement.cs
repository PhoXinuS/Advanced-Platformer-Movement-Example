using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public MovementDataSO movementData;
    private Rigidbody2D rigidBody2D;
    private IMovementInput movementInput;
    
    [SerializeField] CalculateHorizontalVelocity horizontalVelocityCalculator = new CalculateHorizontalVelocity();
    [SerializeField] CalculateVerticalVelocity verticalVelocityCalculator = new CalculateVerticalVelocity();

    private void Start()
    {
        var thisGameObject = gameObject;
        rigidBody2D = thisGameObject.GetComponent<Rigidbody2D>();
        movementInput = new PlayerMovement(thisGameObject); 
        
        horizontalVelocityCalculator.Setup(movementData, thisGameObject, rigidBody2D, movementInput); 
        verticalVelocityCalculator.Setup(movementData, thisGameObject, rigidBody2D, movementInput); 
    }

    private void FixedUpdate()
    {
        var horizontalVelocity = horizontalVelocityCalculator.Calculate();
        var verticalVelocity = verticalVelocityCalculator.Calculate();
        rigidBody2D.velocity = new Vector2(horizontalVelocity, verticalVelocity);
    }
}