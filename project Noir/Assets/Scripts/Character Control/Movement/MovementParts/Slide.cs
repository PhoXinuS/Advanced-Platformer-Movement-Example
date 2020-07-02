using UnityEngine;

[System.Serializable]
public class Slide
{
    [SerializeField] bool calculateSlide = true;

    internal bool isSliding = false;

    private bool wasCrouching;
    private float slidingTime;
    private MovementDataSO movementData;
    private Rigidbody2D rigidBody2D;

    internal void Setup(MovementDataSO movementData, Rigidbody2D rigidBody2D)
    {
        this.movementData = movementData;
        this.rigidBody2D = rigidBody2D;
    }

    internal void Tick(bool isCrouching)
    {
        if (!calculateSlide)
        {
            isSliding = false;
            return;
        }

        if (isCrouching && !wasCrouching)
        {
            isSliding = true;
            slidingTime = movementData.slideDuration;
        }
        else if (!isCrouching)
        {
            isSliding = false;
        }

        if (isSliding)
        {
            slidingTime -= Time.fixedDeltaTime;

            if ( slidingTime <= 0f 
                || Mathf.Abs(rigidBody2D.velocity.x) < 0.05f )
            {
                isSliding = false;
            }
        }

        wasCrouching = isCrouching;
    }
    
}