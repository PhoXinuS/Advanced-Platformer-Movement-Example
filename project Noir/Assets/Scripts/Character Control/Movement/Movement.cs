using UnityEngine;

public class Movement : MonoBehaviour
{
    public MovementDataSO movementData;
    
    [SerializeField] CalculateHorizontalVelocity horizontalCalculator = new CalculateHorizontalVelocity();
    [SerializeField] CalculateVerticalVelocity verticalCalculator = new CalculateVerticalVelocity();
    [SerializeField] SetCrouch setCrouch = new SetCrouch();
    
    private Rigidbody2D rigidBody2D;
    private IMovementInput movementInput;
    private bool isCrouching;
    
    private void Start()
    {
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
        movementInput = new PlayerMovement(gameObject); 
        
        horizontalCalculator.Setup(movementData, rigidBody2D, movementInput); 
        verticalCalculator.Setup(movementData, gameObject, rigidBody2D, movementInput); 
        setCrouch.Setup(movementInput, gameObject);
    }

    private void FixedUpdate()
    {
        float verticalVelocity = verticalCalculator.Calculate();
        
        setCrouch.Set(ref isCrouching);
        float horizontalVelocity = horizontalCalculator.Calculate(isCrouching);
        
        rigidBody2D.velocity = new Vector2(horizontalVelocity, verticalVelocity);
    }
}