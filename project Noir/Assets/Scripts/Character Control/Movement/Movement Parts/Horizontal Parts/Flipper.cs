using UnityEngine;

[System.Serializable]
public class Flipper
{
    internal bool flipped = false;

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
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            flipped = !flipped;
        }
    }

    private bool MovingInDifferentDirectionToTheFacing()
    {
        var horizontalVelocity = rigidBody2D.velocity.x;
        return !flipped && horizontalVelocity < 0 || flipped && horizontalVelocity > 0;
    }
}