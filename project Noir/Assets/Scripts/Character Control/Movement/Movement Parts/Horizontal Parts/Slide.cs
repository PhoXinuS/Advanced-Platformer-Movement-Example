using UnityEngine;

[System.Serializable]
public class Slide
{
    internal bool isSliding = false;

    [SerializeField] string animSlide = "isSliding";

    private int animSlideHashed;
    private bool wasCrouching;
    private float slidingTime;
    private MovementDataSO movementData;
    private Rigidbody2D rigidBody2D;
    private Animator animator;

    internal void Setup(MovementDataSO movementData, Rigidbody2D rigidBody2D, Animator animator)
    {
        this.movementData = movementData;
        this.rigidBody2D = rigidBody2D;
        this.animator = animator;
        
        animSlideHashed = Animator.StringToHash(animSlide);
    }

    internal void Tick(bool isCrouching)
    {
        if (!movementData.calculateSlide)
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
        
        animator.SetBool(animSlideHashed, isSliding);

        wasCrouching = isCrouching;
    }
    
}