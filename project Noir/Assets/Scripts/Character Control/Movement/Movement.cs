using UnityEngine;

public class Movement : MonoBehaviour
{
    public MovementDataSO movementData;
    private Rigidbody2D rigidBody2D;
    private IMovementInput movementInput;
    
    private void Start()
    {
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
        movementInput = new PlayerMovement(gameObject);
    }
    
    private void FixedUpdate()
    {
        var verticalVelocity = movementInput.movementInputNormalized.x * movementData.verticalSpeed;
        var timeMultiplier = Time.fixedDeltaTime * 100f;
        rigidBody2D.velocity = new Vector2(verticalVelocity * timeMultiplier, rigidBody2D.velocity.y);
    }
}
