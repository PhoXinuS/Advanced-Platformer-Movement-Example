using UnityEngine;

[System.Serializable]
public class Slide
{
    internal bool isSliding = false;

    [SerializeField] string animGroundSlide = "isSlidingGround";

    private int animGroundSlideHashed;
    private bool wasCrouching;
    private float slidingTime;
    private MovementDataSO movementData;
    private Rigidbody2D rb2D;
    private Animator animator;

    internal void Setup(MovementDataSO movementData, Rigidbody2D rb2D, Animator animator)
    {
        this.movementData = movementData;
        this.rb2D = rb2D;
        this.animator = animator;
        
        animGroundSlideHashed = Animator.StringToHash(animGroundSlide);
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
                || Mathf.Abs(rb2D.velocity.x) < 0.05f )
            {
                isSliding = false;
            }
        }
        
        animator.SetBool(animGroundSlideHashed, isSliding);

        wasCrouching = isCrouching;
    }
    
}