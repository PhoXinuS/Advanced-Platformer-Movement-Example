using UnityEngine;

[System.Serializable]
public class LedgeSlip
{
    internal bool isSlippingLedge = false;
    
    [SerializeField] string animSlippingLedge = "isSlippingLedge";
    
    private int aimSlippingLedgeHashed;
    private Vector2 ledgeBottomPosition;
    private Vector2 ledgeTopPosition;
    private RigidbodyConstraints2D normalConstraints;

    private Rigidbody2D rb2D;
    private Animator animator;
    private IMovementInput movementInput;

    internal void Setup(Rigidbody2D rb2D, Animator animator, IMovementInput movementInput)
    {
        this.rb2D = rb2D;
        this.animator = animator;
        this.movementInput = movementInput;

        aimSlippingLedgeHashed = Animator.StringToHash(animSlippingLedge);
    }

    internal void ApplySlip(bool isClimbing, bool isCrouching, bool shouldSlipLeftLedge, bool shouldSlipRightLedge, Vector2 ledgeBottomPosition, Vector2 ledgeTopPosition)
    {
        this.ledgeBottomPosition = ledgeBottomPosition;
        this.ledgeTopPosition = ledgeTopPosition;

        bool shouldSlipLedge = shouldSlipLeftLedge || shouldSlipRightLedge;
        if (ShouldSlip(isClimbing, isCrouching, shouldSlipLedge))
        {
            StartSlipping();
        }

        if (isSlippingLedge)
        {
            rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    private bool ShouldSlip(bool isClimbing, bool isCrouching, bool shouldSlipLedge)
    {
        return !isClimbing && shouldSlipLedge && !isSlippingLedge && (movementInput.verticalInput < 0 || isCrouching);
    }

    private void StartSlipping()
    {
        rb2D.transform.position = ledgeTopPosition;
        normalConstraints = rb2D.constraints;
        animator.SetBool(aimSlippingLedgeHashed, true);
        isSlippingLedge = true;
    }

    internal void Slipped()
    {
        rb2D.transform.position = ledgeBottomPosition;
        rb2D.constraints = normalConstraints;
        animator.SetBool(aimSlippingLedgeHashed, false);
        isSlippingLedge = false;
    }
}