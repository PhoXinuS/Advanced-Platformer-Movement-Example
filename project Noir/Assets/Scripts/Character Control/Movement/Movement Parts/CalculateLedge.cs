using UnityEngine;

[System.Serializable]
public class CalculateLedge
{
    public LedgeDangle ledgeDangle = new LedgeDangle();
    public LedgeClimb ledgeClimb = new LedgeClimb();
    
    [SerializeField] Vector2 ledgeDangleOffset = Vector2.zero;
    [SerializeField] Vector2 ledgeClimbedOffset = Vector2.zero;
    private Vector2 ledgeDanglePosition;
    private Vector2 ledgeClimbedPosition;
    
    [SerializeField] SingleRaycast2DChecker leftLedgeCheckTop = new SingleRaycast2DChecker();
    [SerializeField] SingleRaycast2DChecker rightLedgeCheckTop = new SingleRaycast2DChecker();  
    [SerializeField] SingleRaycast2DChecker leftLedgeCheckBottom = new SingleRaycast2DChecker();
    [SerializeField] SingleRaycast2DChecker rightLedgeCheckBottom = new SingleRaycast2DChecker();
    private Transform leftLedgeCheckTopTransform;
    private Transform leftLedgeCheckBottomTransform;  
    private Transform rightLedgeCheckTopTransform;
    private Transform rightLedgeCheckBottomTransform;

    internal void Setup(Rigidbody2D rb2D, Animator animator, IMovementInput movementInput)
    {
        leftLedgeCheckTop.Setup(Vector2.left);
        rightLedgeCheckTop.Setup(Vector2.right);
        leftLedgeCheckBottom.Setup(Vector2.left);
        rightLedgeCheckBottom.Setup(Vector2.right);

        leftLedgeCheckTopTransform = leftLedgeCheckTop.raycastOrigin;
        rightLedgeCheckTopTransform = rightLedgeCheckTop.raycastOrigin;    
        leftLedgeCheckBottomTransform = leftLedgeCheckBottom.raycastOrigin;
        rightLedgeCheckBottomTransform = rightLedgeCheckBottom.raycastOrigin;
        
        ledgeDangle.Setup(rb2D, animator, movementInput);
        ledgeClimb.Setup(rb2D, animator, movementInput);
    }

    internal void ApplyLedge(bool flipped)
    {
        if (!flipped)
        {
            leftLedgeCheckTop.raycastOrigin = leftLedgeCheckTopTransform;
            leftLedgeCheckBottom.raycastOrigin = leftLedgeCheckBottomTransform;
            
            rightLedgeCheckTop.raycastOrigin = rightLedgeCheckTopTransform;
            rightLedgeCheckBottom.raycastOrigin = rightLedgeCheckBottomTransform;
        }
        else
        {
            leftLedgeCheckTop.raycastOrigin = rightLedgeCheckTopTransform;
            leftLedgeCheckBottom.raycastOrigin = rightLedgeCheckBottomTransform;
            
            rightLedgeCheckTop.raycastOrigin = leftLedgeCheckTopTransform;
            rightLedgeCheckBottom.raycastOrigin = leftLedgeCheckBottomTransform;
        }
        
        bool shouldClimbLeftLedge = !leftLedgeCheckTop.IsInContactWithTarget() && leftLedgeCheckBottom.IsInContactWithTarget();
        bool shouldClimbRightLedge = !rightLedgeCheckTop.IsInContactWithTarget() && rightLedgeCheckBottom.IsInContactWithTarget();
        if (shouldClimbRightLedge)
        {
            var ledgeBottomRaycastTransform = rightLedgeCheckBottom.raycastOrigin.position;
            ledgeDanglePosition = new Vector2(
                Mathf.Floor(ledgeBottomRaycastTransform.x + rightLedgeCheckBottom.checkDistance) + ledgeDangleOffset.x,
                Mathf.Floor(ledgeBottomRaycastTransform.y) + ledgeDangleOffset.y);

            ledgeClimbedPosition = new Vector2(
                Mathf.Floor(ledgeBottomRaycastTransform.x + rightLedgeCheckBottom.checkDistance) + ledgeClimbedOffset.x,
                Mathf.Floor(ledgeBottomRaycastTransform.y) + ledgeClimbedOffset.y);
        }
        else if (shouldClimbLeftLedge)
        {
            var ledgeBottomRaycastTransform = leftLedgeCheckBottom.raycastOrigin.position;
            ledgeDanglePosition = new Vector2(
                Mathf.Ceil(ledgeBottomRaycastTransform.x - leftLedgeCheckBottom.checkDistance) - ledgeDangleOffset.x,
                Mathf.Floor(ledgeBottomRaycastTransform.y) + ledgeDangleOffset.y);

            ledgeClimbedPosition = new Vector2(
                Mathf.Ceil(ledgeBottomRaycastTransform.x - leftLedgeCheckBottom.checkDistance) - ledgeClimbedOffset.x,
                Mathf.Floor(ledgeBottomRaycastTransform.y) + ledgeClimbedOffset.y);
        }
        
        ledgeDangle.ApplyDangle(ledgeClimb.isClimbingLedge, shouldClimbLeftLedge, shouldClimbRightLedge, ledgeDanglePosition);
        ledgeClimb.ApplyClimb(ledgeDangle.isDangling, shouldClimbLeftLedge, shouldClimbRightLedge, ledgeClimbedPosition);
    }
}