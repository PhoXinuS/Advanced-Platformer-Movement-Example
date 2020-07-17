using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class CalculateLedge
{
    [SerializeField] SingleRaycast2DChecker leftLedgeCheckTop = new SingleRaycast2DChecker();
    [SerializeField] SingleRaycast2DChecker rightLedgeCheckTop = new SingleRaycast2DChecker();  
    [SerializeField] SingleRaycast2DChecker leftLedgeCheckBottom = new SingleRaycast2DChecker();
    [SerializeField] SingleRaycast2DChecker rightLedgeCheckBottom = new SingleRaycast2DChecker();
    [SerializeField] Vector2 ledgeDidNotClimbOffset = Vector2.zero;
    [SerializeField] Vector2 ledgeClimbedOffset = Vector2.zero;
    [SerializeField] string animClimbingLedge = "isClimbingLedge";

    private int animClimbingLedgeHashed;
    private bool climbingLedge = false;
    private Vector2 ledgeDidNotClimbPosition;
    private Vector2 ledgeClimbedPosition;
    
    private Rigidbody2D rb2D;
    private Animator animator;

    internal void SetUp(Rigidbody2D rb2D, Animator animator)
    {
        this.rb2D = rb2D;
        this.animator = animator;

        animClimbingLedgeHashed = Animator.StringToHash(animClimbingLedge);
        leftLedgeCheckTop.Setup(Vector2.left);
        rightLedgeCheckTop.Setup(Vector2.right);
    }

    internal void ApplyLedge(bool flipped)
    {
        bool canClimbLeftLedge, canClimbRightLedge = false;
        bool isTouchingLeftWall, isTouchingRightWall = false;
        if (!flipped)
        {
            leftLedgeCheckTop.Setup(Vector2.left); 
            rightLedgeCheckTop.Setup(Vector2.right); 
            canClimbLeftLedge = !leftLedgeCheckTop.IsInContactWithTarget();
            canClimbRightLedge = !rightLedgeCheckTop.IsInContactWithTarget();
            
            leftLedgeCheckBottom.Setup(Vector2.left);
            rightLedgeCheckBottom.Setup(Vector2.right);
            isTouchingLeftWall = leftLedgeCheckBottom.IsInContactWithTarget();
            isTouchingRightWall = rightLedgeCheckBottom.IsInContactWithTarget();
        }
        else
        {
            leftLedgeCheckTop.Setup(Vector2.right); 
            rightLedgeCheckTop.Setup(Vector2.left); 
            canClimbLeftLedge = !rightLedgeCheckTop.IsInContactWithTarget();
            canClimbRightLedge = !leftLedgeCheckTop.IsInContactWithTarget();
            
            leftLedgeCheckBottom.Setup(Vector2.right);
            rightLedgeCheckBottom.Setup(Vector2.left);  
            isTouchingLeftWall = rightLedgeCheckBottom.IsInContactWithTarget();
            isTouchingRightWall = leftLedgeCheckBottom.IsInContactWithTarget();
        }

        if ((canClimbLeftLedge && isTouchingLeftWall && !climbingLedge)
            || (canClimbRightLedge && isTouchingRightWall && !climbingLedge))
        {
            var ledgeBottomRaycastTransform = rightLedgeCheckBottom.raycastOrigin.position;
            if (flipped)
            {
                ledgeDidNotClimbPosition = new Vector2(
                    Mathf.Floor(ledgeBottomRaycastTransform.x + rightLedgeCheckBottom.checkDistance) - ledgeDidNotClimbOffset.x,
                    Mathf.Floor(ledgeBottomRaycastTransform.y) + ledgeDidNotClimbOffset.y);
                
                ledgeClimbedPosition = new Vector2(
                    Mathf.Floor(ledgeBottomRaycastTransform.x + rightLedgeCheckBottom.checkDistance) + ledgeClimbedOffset.x,
                    Mathf.Floor(ledgeBottomRaycastTransform.y) + ledgeClimbedOffset.y);
            }
            else
            {
                ledgeDidNotClimbPosition = new Vector2(
                    Mathf.Ceil(ledgeBottomRaycastTransform.x - rightLedgeCheckBottom.checkDistance) + ledgeDidNotClimbOffset.x,
                    Mathf.Floor(ledgeBottomRaycastTransform.y) + ledgeDidNotClimbOffset.y);
                
                ledgeClimbedPosition = new Vector2(
                    Mathf.Ceil(ledgeBottomRaycastTransform.x - rightLedgeCheckBottom.checkDistance) - ledgeClimbedOffset.x,
                    Mathf.Floor(ledgeBottomRaycastTransform.y) + ledgeClimbedOffset.y);
            }
            
            rb2D.MovePosition(ledgeDidNotClimbPosition);
            rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
            animator.SetBool(animClimbingLedgeHashed, true);
            climbingLedge = true;
        }
    }

    internal void Climbed()
    {
        Debug.Log("I don't mind");
        rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb2D.transform.position = ledgeClimbedPosition;
        //rb2D.MovePosition(ledgeClimbedPosition);
        animator.SetBool(animClimbingLedgeHashed, false);
        climbingLedge = false;
    }
}
