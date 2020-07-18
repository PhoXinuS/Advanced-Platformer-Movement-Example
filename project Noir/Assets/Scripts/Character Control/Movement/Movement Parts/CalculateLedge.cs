using System.Collections.Generic;
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

    private Transform leftLedgeCheckTransform;
    private Transform leftWallCheckTransform;  
    private Transform rightLedgeCheckTransform;
    private Transform rightWallCheckTransform;
    
    private int animClimbingLedgeHashed;
    private bool climbingLedge = false;
    private Vector2 ledgeDidNotClimbPosition;
    private Vector2 ledgeClimbedPosition;
    private RigidbodyConstraints2D normalConstraints;
    
    private Rigidbody2D rb2D;
    private Animator animator;

    internal void SetUp(Rigidbody2D rb2D, Animator animator)
    {
        this.rb2D = rb2D;
        this.animator = animator;

        animClimbingLedgeHashed = Animator.StringToHash(animClimbingLedge);
        
        leftLedgeCheckTop.Setup(Vector2.left);
        rightLedgeCheckTop.Setup(Vector2.right);
        leftLedgeCheckBottom.Setup(Vector2.left);
        rightLedgeCheckBottom.Setup(Vector2.right);

        leftLedgeCheckTransform = leftLedgeCheckTop.raycastOrigin;
        rightLedgeCheckTransform = rightLedgeCheckTop.raycastOrigin;    
        leftWallCheckTransform = leftLedgeCheckBottom.raycastOrigin;
        rightWallCheckTransform = rightLedgeCheckBottom.raycastOrigin;
    }

    internal void ApplyLedge(bool flipped)
    {
        if (!flipped)
        {
            leftLedgeCheckTop.raycastOrigin = leftLedgeCheckTransform;
            leftLedgeCheckBottom.raycastOrigin = leftWallCheckTransform;
            
            rightLedgeCheckTop.raycastOrigin = rightLedgeCheckTransform;
            rightLedgeCheckBottom.raycastOrigin = rightWallCheckTransform;
        }
        else
        {
            leftLedgeCheckTop.raycastOrigin = rightLedgeCheckTransform;
            leftLedgeCheckBottom.raycastOrigin = rightWallCheckTransform;
            
            rightLedgeCheckTop.raycastOrigin = leftLedgeCheckTransform;
            rightLedgeCheckBottom.raycastOrigin = leftWallCheckTransform;
        }
        
        bool shouldClimbLeftLedge = !leftLedgeCheckTop.IsInContactWithTarget() && leftLedgeCheckBottom.IsInContactWithTarget();
        bool shouldClimbRightLedge = !rightLedgeCheckTop.IsInContactWithTarget() && rightLedgeCheckBottom.IsInContactWithTarget();
        if (!climbingLedge && (shouldClimbLeftLedge || shouldClimbRightLedge))
        {
            normalConstraints = rb2D.constraints;
            if (shouldClimbRightLedge)
            {
                var ledgeBottomRaycastTransform = rightLedgeCheckBottom.raycastOrigin.position;
                ledgeDidNotClimbPosition = new Vector2(
                    Mathf.Floor(ledgeBottomRaycastTransform.x + rightLedgeCheckBottom.checkDistance) +
                    ledgeDidNotClimbOffset.x,
                    Mathf.Floor(ledgeBottomRaycastTransform.y) + ledgeDidNotClimbOffset.y);

                ledgeClimbedPosition = new Vector2(
                    Mathf.Floor(ledgeBottomRaycastTransform.x + rightLedgeCheckBottom.checkDistance) +
                    ledgeClimbedOffset.x,
                    Mathf.Floor(ledgeBottomRaycastTransform.y) + ledgeClimbedOffset.y);
            }
            else if (shouldClimbLeftLedge)
            {
                var ledgeBottomRaycastTransform = leftLedgeCheckBottom.raycastOrigin.position;
                ledgeDidNotClimbPosition = new Vector2(
                    Mathf.Ceil(ledgeBottomRaycastTransform.x - leftLedgeCheckBottom.checkDistance) -
                    ledgeDidNotClimbOffset.x,
                    Mathf.Floor(ledgeBottomRaycastTransform.y) + ledgeDidNotClimbOffset.y);

                ledgeClimbedPosition = new Vector2(
                    Mathf.Ceil(ledgeBottomRaycastTransform.x - leftLedgeCheckBottom.checkDistance) -
                    ledgeClimbedOffset.x,
                    Mathf.Floor(ledgeBottomRaycastTransform.y) + ledgeClimbedOffset.y);
            }

            StartLedgeClimbing();
        }
    }

    private void StartLedgeClimbing()
    {
        rb2D.transform.position = ledgeDidNotClimbPosition;
        rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
        animator.SetBool(animClimbingLedgeHashed, true);
        climbingLedge = true;
    }

    internal void Climbed()
    {
        rb2D.constraints = normalConstraints;
        rb2D.transform.position = ledgeClimbedPosition;
        animator.SetBool(animClimbingLedgeHashed, false);
        climbingLedge = false;
    }
}


