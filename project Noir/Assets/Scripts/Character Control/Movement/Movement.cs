using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public MovementDataSO movementData;
    private Rigidbody2D rigidBody2D;
    private IMovementInput movementInput;
    
    [SerializeField] CalculateHorizontalVelocity horizontalVelocityCalculator = new CalculateHorizontalVelocity();
    [SerializeField] CalculateVerticalVelocity verticalVelocityCalculator = new CalculateVerticalVelocity();

    private void Start()
    {
        var thisGameObject = gameObject;
        rigidBody2D = thisGameObject.GetComponent<Rigidbody2D>();
        movementInput = new PlayerMovement(thisGameObject); 
        
        horizontalVelocityCalculator.Setup(movementData, thisGameObject, rigidBody2D, movementInput); 
        verticalVelocityCalculator.Setup(movementData, thisGameObject, rigidBody2D, movementInput); 
    }

    private void FixedUpdate()
    {
        var horizontalVelocity = horizontalVelocityCalculator.Calculate();
        var verticalVelocity = verticalVelocityCalculator.Calculate();
        rigidBody2D.velocity = new Vector2(horizontalVelocity, verticalVelocity);
    }
}

[System.Serializable]
internal class CalculateHorizontalVelocity
{
    internal void Setup( MovementDataSO movementData
        , GameObject gameObject
        , Rigidbody2D rigidBody2D
        , IMovementInput movementInput )
    {       
        this.movementData = movementData;
        this.gameObject = gameObject;
        this.rigidBody2D = rigidBody2D;
        this.movementInput = movementInput;
    }
    
    [SerializeField] bool calculateHorizontalVelocity = true;
    [SerializeField] LayerMask whatIsWall;
    [SerializeField] List<Transform> wallChecks = new List<Transform>();
    [SerializeField] float wallCheckLength = 1f;
    
    private MovementDataSO movementData;
    private GameObject gameObject;
    private Rigidbody2D rigidBody2D;
    private IMovementInput movementInput;
    private float xVelocity = 0f;

    internal float Calculate()
    {
        if (!calculateHorizontalVelocity)
            return rigidBody2D.velocity.x;
        
        
        var horizontalInput = movementInput.movementInputNormalized.x;

        float timeMultiplier = Time.fixedDeltaTime * 100f;
        float horizontalTargetVelocity = horizontalInput * movementData.horizontalSpeed * timeMultiplier;

        float horizontalVelocity;
        if (VelocityIsIncreasing(horizontalTargetVelocity))
        {
            horizontalVelocity = Mathf.SmoothDamp(rigidBody2D.velocity.x, horizontalTargetVelocity, ref xVelocity,
                movementData.accelerationTime);
        }
        else
        {
            horizontalVelocity = Mathf.SmoothDamp(rigidBody2D.velocity.x, horizontalTargetVelocity, ref xVelocity,
                movementData.decelerationTime);
        }

        if (IsRunningIntoWall(horizontalVelocity))
            return 0f;
        
        return horizontalVelocity;
    }
    
    private bool VelocityIsIncreasing(float horizontalTargetVelocity)
    {
        return Mathf.Abs(horizontalTargetVelocity) > Mathf.Abs(rigidBody2D.velocity.x);
    }
    
    private bool IsRunningIntoWall(float horizontalVelocity)
    {
        foreach (var wallCheck in wallChecks)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(wallCheck.position
                , Vector2.right * horizontalVelocity
                , wallCheckLength
                , whatIsWall);

            foreach (var hit in hits)
            {
                if (hit.collider.gameObject != gameObject)
                    return true;
            }
        }        
        return false;
    }
}

[System.Serializable]
internal class CalculateVerticalVelocity
{
    internal void Setup( MovementDataSO movementData
        , GameObject gameObject
        , Rigidbody2D rigidBody2D
        , IMovementInput movementInput )
    {
        this.movementData = movementData;
        this.gameObject = gameObject;
        this.rigidBody2D = rigidBody2D;
        this.movementInput = movementInput;

        int jumpsCount = movementData.jumpsNumber;
        for (int i = 0; i < jumpsCount; i++)
        {
            avaibleJumps.Add(false);
        }
    }

    [SerializeField] bool calculateVerticalVelocity = true;
    
    private MovementDataSO movementData;
    private Rigidbody2D rigidBody2D;
    private GameObject gameObject;
    private IMovementInput movementInput;
    
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float rememberJumpPressTime = 0.2f;
    [SerializeField] float delayedJumpPressTime = 0.2f;
    [SerializeField] float fallMultiplier = 2.5f;
    
    private float rememberJumpPressCounter;
    private float delayedJumpPressCounter;

    private List<bool> avaibleJumps = new List<bool>();
    private bool grounded;
    private bool alreadyPushedJumpButton;

    internal float Calculate()
    {
        //TODO: remove ground detection from this class to another marked with interface.
        //TODO: or just make another class and ScriptableObject with grounded variable

        float verticalVelocity = rigidBody2D.velocity.y;
        verticalVelocity += BetterFallingVelocity();
        verticalVelocity += AdjustJumpheight();
        
        if (!calculateVerticalVelocity)
            return verticalVelocity;
        

        int jumpsCount = movementData.jumpsNumber;

        bool wasGrounded = grounded;
        grounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.05f, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                grounded = true;
                for (int j = 0; j < jumpsCount; j++)
                {
                    avaibleJumps[j] = true;
                }

                break;
            }
        }

        if (wasGrounded && !grounded)
            delayedJumpPressCounter = delayedJumpPressTime;
        
        if ( HoldingInputUpButton()
            && !alreadyPushedJumpButton
            && !avaibleJumps.Contains(true) )
        {
            rememberJumpPressCounter = rememberJumpPressTime;
        }

        if (delayedJumpPressCounter > 0)
            delayedJumpPressCounter -= Time.fixedDeltaTime;
        if (rememberJumpPressCounter > 0)
            rememberJumpPressCounter -= Time.fixedDeltaTime;

        if ( (HoldingInputUpButton() || rememberJumpPressCounter > 0 )
            && !alreadyPushedJumpButton)
        {
            alreadyPushedJumpButton = true;
            
            if ((grounded || delayedJumpPressCounter > 0) 
                && avaibleJumps.Count > 0
                && avaibleJumps[0])
            {
                avaibleJumps[0] = false;
                return movementData.jumpHeight;
            }
            
            for (int i = 1; i < jumpsCount; i++)
            {
                if (avaibleJumps[i])
                {
                    avaibleJumps[i] = false;
                    return movementData.jumpHeight;
                }
            }
        }
        
        if(!HoldingInputUpButton())
        {
            alreadyPushedJumpButton = false;
        }
        

        return verticalVelocity;
    }

    
    private float BetterFallingVelocity()
    {
        if (rigidBody2D.velocity.y < 0)
            return Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        
        return 0f;
    }
    
    private float AdjustJumpheight()
    {
        if (!HoldingInputUpButton() 
            && rigidBody2D.velocity.y > 0
            && alreadyPushedJumpButton)
        {
            return -(rigidBody2D.velocity.y / 2);
        }
        return 0f;
    }

    private bool HoldingInputUpButton()
    {
        return movementInput.movementInputNormalized.y > 0;
    }
}
