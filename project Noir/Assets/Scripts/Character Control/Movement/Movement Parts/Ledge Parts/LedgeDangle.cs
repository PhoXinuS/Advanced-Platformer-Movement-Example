using Unity.Collections;
using UnityEngine;

[System.Serializable]
public class LedgeDangle
{
    [SerializeField] string animDanglingLedge = "isDanglingLedge";
    [SerializeField] string animLeaningLedge = "isLeaningLedge";

    private int animDanglingLedgeHashed;
    private int animLeaningLedgeHashed;
    private bool isDangling = false;
    private bool wasJumping = false;
    private RigidbodyConstraints2D normalConstraints;

    private Rigidbody2D rb2D;
    private Animator animator;
    private IMovementInput movementInput;

    internal void Setup(Rigidbody2D rb2D, Animator animator, IMovementInput movementInput)
    {
        this.rb2D = rb2D;
        this.animator = animator;
        this.movementInput = movementInput;

        animDanglingLedgeHashed = Animator.StringToHash(animDanglingLedge);
        animLeaningLedgeHashed = Animator.StringToHash(animLeaningLedge);
    }

    internal bool IsDangling()
    {
        return isDangling;
    }

    internal void ApplyDangle(bool shouldDangleLeftLedge, bool shouldDangleRightLedge, Vector2 ledgeDanglePosition)
    {
        bool shouldDangleLedge = shouldDangleLeftLedge || shouldDangleRightLedge;

        if (!isDangling && shouldDangleLedge)
        {
            normalConstraints = rb2D.constraints;
            StartDangling(ledgeDanglePosition);
        }

        if (ShouldLeanOut(shouldDangleLeftLedge, shouldDangleRightLedge))
        {
            LeanOut();
        }
        else
        {
            CancelLeanOut();
        }

        if (ShouldCancelDangle() && isDangling)
        {
            CancelDangle();
        }

        if (!shouldDangleLedge)
        {
            isDangling = false;
        }

        wasJumping = movementInput.jumpInput > 0;
    }

    private void StartDangling(Vector2 ledgeDanglePosition)
    {
        rb2D.transform.position = ledgeDanglePosition;
        rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
        animator.SetBool(animDanglingLedgeHashed, true);
        isDangling = true;
    }

    private bool ShouldLeanOut(bool shouldDangleLeftLedge, bool shouldDangleRightLedge)
    {
        return (shouldDangleLeftLedge && movementInput.horizontalInput > 0)
               || (shouldDangleRightLedge && movementInput.horizontalInput < 0);
    }
    
    private void LeanOut()
    {
        animator.SetBool(animLeaningLedgeHashed, true);
    }
    private void CancelLeanOut()
    {
        animator.SetBool(animLeaningLedgeHashed, false);
    }
    
    private bool ShouldCancelDangle()
    {
        return movementInput.verticalInput < 0 || !wasJumping && movementInput.jumpInput > 0;
    }

    internal void CancelDangle()
    {
        rb2D.constraints = normalConstraints;
        animator.SetBool(animDanglingLedgeHashed, false);
    }
}
