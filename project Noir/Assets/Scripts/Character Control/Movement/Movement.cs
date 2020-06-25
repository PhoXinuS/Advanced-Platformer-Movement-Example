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
        {
            return 0f;
        }
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
                {
                    return true;
                }
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

    private List<bool> avaibleJumps = new List<bool>();
    private bool grounded;
    private bool alreadyPushedJumpButton;
    
    internal float Calculate()
    {
        //TODO: remove ground detection from this class to another marked with interface. Leave only input check and maybe Grounded / DoubleJump

        if (!calculateVerticalVelocity)
            return rigidBody2D.velocity.y;

        int jumpsCount = movementData.jumpsNumber;

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

        if (HoldingInputUpButton() && !alreadyPushedJumpButton)
        {
            alreadyPushedJumpButton = true;
            
            if (grounded && avaibleJumps.Count > 0)
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
        else if(!HoldingInputUpButton())
        {
            alreadyPushedJumpButton = false;
        }

        return rigidBody2D.velocity.y;
    }

    private bool HoldingInputUpButton()
    {
        return movementInput.movementInputNormalized.y > 0;
    }
}
