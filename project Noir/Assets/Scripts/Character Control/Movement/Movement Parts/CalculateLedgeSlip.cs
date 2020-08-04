using UnityEngine;

[System.Serializable]
public class CalculateLedgeSlip
{
    public LedgeSlip ledgeSlip = new LedgeSlip();
    
    [SerializeField] Vector2 ledgeBottomOffset = Vector2.zero;
    [SerializeField] Vector2 ledgeTopOffset = Vector2.zero;
    private Vector2 ledgeBottomPosition;
    private Vector2 ledgeTopPosition;
    
    [SerializeField] SingleRaycast2DChecker leftLedgeSlipCheck = new SingleRaycast2DChecker();
    [SerializeField] SingleRaycast2DChecker midLedgeSlipCheck = new SingleRaycast2DChecker();  
    [SerializeField] SingleRaycast2DChecker rightLedgeSlipCheck = new SingleRaycast2DChecker();
    private Transform leftLedgeSlipCheckTransform;
    private Transform midLedgeSlipCheckTransform;  
    private Transform rightLedgeSlipCheckTransform;

    internal void Setup(Rigidbody2D rb2D, Animator animator, IMovementInput movementInput)
    {
        leftLedgeSlipCheck.Setup(Vector2.down);
        midLedgeSlipCheck.Setup(Vector2.down);
        rightLedgeSlipCheck.Setup(Vector2.down);

        leftLedgeSlipCheckTransform = leftLedgeSlipCheck.raycastOrigin;
        rightLedgeSlipCheckTransform = midLedgeSlipCheck.raycastOrigin;    
        midLedgeSlipCheckTransform = rightLedgeSlipCheck.raycastOrigin;

        ledgeSlip.Setup(rb2D, animator, movementInput);
    }

    internal void ApplyLedgeSlip(bool flipped, bool isClimbing, bool isCrouching)
    {
        if (!flipped)
        {
            leftLedgeSlipCheck.raycastOrigin = leftLedgeSlipCheckTransform;
            rightLedgeSlipCheck.raycastOrigin = midLedgeSlipCheckTransform;

        }
        else
        {
            leftLedgeSlipCheck.raycastOrigin = rightLedgeSlipCheckTransform;
            rightLedgeSlipCheck.raycastOrigin = leftLedgeSlipCheckTransform;
        }
        
        bool shouldSlipLeftLedge = !midLedgeSlipCheck.IsInContactWithTarget() && rightLedgeSlipCheck.IsInContactWithTarget();
        bool shouldSlipRightLedge = !midLedgeSlipCheck.IsInContactWithTarget() && leftLedgeSlipCheck.IsInContactWithTarget();
        
        if (shouldSlipRightLedge)
        {
            var ledgeGroundRaycastTransform = leftLedgeSlipCheck.raycastOrigin.position;
            var ledgeTilePosition = new Vector2(
                Mathf.Ceil(ledgeGroundRaycastTransform.x), 
                Mathf.Ceil(ledgeGroundRaycastTransform.y - leftLedgeSlipCheck.checkDistance));
            
            ledgeBottomPosition = new Vector2(
                ledgeTilePosition.x + ledgeBottomOffset.x,
                ledgeTilePosition.y + ledgeBottomOffset.y);

            ledgeTopPosition = new Vector2(
                ledgeTilePosition.x + ledgeTopOffset.x,
                ledgeTilePosition.y + ledgeTopOffset.y);
        }
        else if (shouldSlipLeftLedge)
        {
            var ledgeGroundRaycastTransform = rightLedgeSlipCheck.raycastOrigin.position;
            var ledgeTilePosition = new Vector2(
                Mathf.Floor(ledgeGroundRaycastTransform.x), 
                Mathf.Ceil(ledgeGroundRaycastTransform.y - rightLedgeSlipCheck.checkDistance));
            
            ledgeBottomPosition = new Vector2(
                ledgeTilePosition.x - ledgeBottomOffset.x,
                ledgeTilePosition.y + ledgeBottomOffset.y);

            ledgeTopPosition = new Vector2(
                ledgeTilePosition.x - ledgeTopOffset.x,
                ledgeTilePosition.y + ledgeTopOffset.y);
        }
        
        ledgeSlip.ApplySlip(isClimbing, isCrouching, shouldSlipLeftLedge, shouldSlipRightLedge, ledgeBottomPosition, ledgeTopPosition);
    }
}