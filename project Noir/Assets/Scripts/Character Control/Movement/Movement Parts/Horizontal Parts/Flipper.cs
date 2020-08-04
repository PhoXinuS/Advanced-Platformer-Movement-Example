using UnityEngine;

public class Flipper
{
    internal bool flipped = false;

    private Rigidbody2D rb2D;
    private Transform transform;

    internal void Setup(Rigidbody2D rb2D, Transform transform)
    {
        this.rb2D = rb2D;
        this.transform = transform;
    }
    
    internal void ApplyFlip(bool isGrounded, bool isOnLedge, bool isTouchingLeftWall, bool isTouchingRightWall)
    {
        if (!flipped
            && isTouchingLeftWall && !isGrounded)
        {
            Flip();
        }
        else if (flipped
                 && isTouchingRightWall && !isGrounded)
        {
            Flip();
        }
        else if (MovingInDifferentDirectionToTheFacing()
                 && (!isTouchingLeftWall && !isTouchingRightWall || isGrounded) && !isOnLedge )
        {
            Flip();
        }
    }
    
    private bool MovingInDifferentDirectionToTheFacing()
    {
        var horizontalVelocity = rb2D.velocity.x;
        return !flipped && horizontalVelocity < -0.1f || flipped && horizontalVelocity > 0.1f;
    }
    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        flipped = !flipped;
    }
}