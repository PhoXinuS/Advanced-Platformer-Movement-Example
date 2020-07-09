using UnityEngine;

public class Flipper
{
    private bool facingRight = true;
    private Rigidbody2D rigidBody2D;
    private Transform transform;
    
    internal void Setup(Rigidbody2D rigidBody2D, Transform transform)
    {
        this.rigidBody2D = rigidBody2D;
        this.transform = transform;
    }
    
    internal void ApplyFlip()
    {
        if (MovingInDifferentDirectionToTheFacing())
        {
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
            facingRight = !facingRight;
        }
    }

    private bool MovingInDifferentDirectionToTheFacing()
    {
        var horizontalVelocity = rigidBody2D.velocity.x;
        return facingRight && horizontalVelocity < 0 || !facingRight && horizontalVelocity > 0;
    }
}