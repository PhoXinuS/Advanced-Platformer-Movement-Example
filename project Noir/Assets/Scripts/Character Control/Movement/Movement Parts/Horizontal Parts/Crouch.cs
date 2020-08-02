using UnityEngine;

[System.Serializable]
public class Crouch
{
    public Slide slide = new Slide();
    internal bool isCrouching;

    [SerializeField] string animCrouch = "isCrouching";
    [SerializeField] Collider2D[] normalColliders = new Collider2D[0];
    [SerializeField] Collider2D[] crouchColliders = new Collider2D[0];
    
    private int animCrouchHashed;
    private bool wasStanding = true;
    private IMovementInput movementInput;
    private MovementDataSO movementData;
    private Animator animator;

    internal void Setup(IMovementInput movementInput
        , MovementDataSO movementData
        , Rigidbody2D rb2D
        , Animator animator)
    {
        this.movementInput = movementInput;
        this.movementData = movementData;
        this.animator = animator;
        slide.Setup(movementData, rb2D, animator);
        
        animCrouchHashed = Animator.StringToHash(animCrouch);
    }

    internal void Tick(bool isGrounded, bool canStand)
    {
        if (!movementData.calculateCrouch)
        {
            isCrouching = false;
            return;
        }

        if (CrouchIsTriggered(canStand) && isGrounded)
        {
            isCrouching = true;
            
            foreach (var normalCollider in normalColliders)
                normalCollider.enabled = false;
            foreach (var crouchCollider in crouchColliders)
                crouchCollider.enabled = true;

            wasStanding = false;
            
            animator.SetBool(animCrouchHashed, true);
        }
        else if(!wasStanding && canStand)
        {
            isCrouching = false;
            
            foreach (var normalCollider in normalColliders)
                normalCollider.enabled = true;
            foreach (var crouchCollider in crouchColliders)
                crouchCollider.enabled = false;
            
            wasStanding = true;
            
            animator.SetBool(animCrouchHashed, false);
        }
        
        
        slide.Tick(isCrouching);
    }

    private bool CrouchIsTriggered(bool canStand)
    {
        return movementInput.verticalInput < 0f || !canStand;
    }
    
}  
