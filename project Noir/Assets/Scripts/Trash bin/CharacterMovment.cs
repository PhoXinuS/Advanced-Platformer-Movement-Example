using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovment : MonoBehaviour
{
    public GameObject Player;
    public Rigidbody2D Player_RigidBody;
    public Collider2D PlayerGroundedCheck_Collider;

    public LayerMask ground;

    private bool sprintingingBeforeJump = false;

    [SerializeField] float jumpForce = 3f;

    private float moveDampingBasic;
    private float moveDampingWhenStopping;
    private float moveDampingWhenTurning;

    [Range(0, 1)] [SerializeField] float moveDampingBasic_Normal = 0.65f;
    [Range(0, 1)] [SerializeField] float moveDampingWhenStopping_Normal = 0.85f;
    [Range(0, 1)] [SerializeField] float moveDampingWhenTurning_Normal = 0.7f;

    [Range(0, 1)] [SerializeField] float moveDampingBasic_Sprint = 0.5f;
    [Range(0, 1)] [SerializeField] float moveDampingWhenStopping_Sprint = 0.7f;
    [Range(0, 1)] [SerializeField] float moveDampingWhenTurning_Sprint = 0.6f;

    [Range(0, 15)] [SerializeField] float maxMoveSpeedNormal = 15f;
    [Range(0, 15)] [SerializeField] float maxMoveSpeedSprint = 7f;

    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float lowJumpMultiplier = 2f;

    private float groundedRememberTime = 0f;
    [SerializeField] private float setGroundedRememberTime = 0.2f;
    private float jumpPressedRemember = 0f;
    [SerializeField] private float setJumpPressedRememberTime = 0.2f;


    private float controlOff_timeLeft;
    private float verticalControlOff_timeLeft;

    private void Start()
    {
        //Events.turnOffControl += turnOffControl;
        //Events.turnOffControlVertical += turnOffControlVertical;
    }

    private void FixedUpdate()
    {
        controlOff_timeLeft -= Time.fixedDeltaTime;
        verticalControlOff_timeLeft -= Time.fixedDeltaTime;

        if (Player != null && Player_RigidBody != null && PlayerGroundedCheck_Collider != null && controlOff_timeLeft < 0)
        {
            bool grounded = PlayerGroundedCheck_Collider.IsTouchingLayers(ground);


            #region Horizontal movment

            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveDampingBasic = moveDampingBasic_Sprint;
                moveDampingWhenStopping = moveDampingWhenStopping_Sprint;
                moveDampingWhenTurning = moveDampingWhenTurning_Sprint;
            }
            else
            {
                moveDampingBasic = moveDampingBasic_Normal;
                moveDampingWhenStopping = moveDampingWhenStopping_Normal;
                moveDampingWhenTurning = moveDampingWhenTurning_Normal;
            }

            float moveSpeed = Player_RigidBody.velocity.x;
            moveSpeed += Input.GetAxisRaw("Horizontal");

            if (!Input.GetButton("Horizontal") && Mathf.Abs(Input.GetAxis("Horizontal")) > 0)
            {
                moveSpeed *= Mathf.Pow(1f - moveDampingWhenStopping, Time.fixedDeltaTime * 10f);
            }
            else if (Mathf.Sign(Input.GetAxisRaw("Horizontal")) != Mathf.Sign(moveSpeed))
            {
                moveSpeed *= Mathf.Pow(1f - moveDampingWhenTurning, Time.fixedDeltaTime * 10f);
            }
            else
            {
                moveSpeed *= Mathf.Pow(1f - moveDampingBasic, Time.fixedDeltaTime * 10f);
            }

            Player_RigidBody.velocity = new Vector2(moveSpeed, Player_RigidBody.velocity.y);

            if (grounded)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    sprintingingBeforeJump = true;
                }
                else
                {
                    sprintingingBeforeJump = false;
                }
            }

            if (sprintingingBeforeJump)
            {
                if (Mathf.Abs(Player_RigidBody.velocity.x) > maxMoveSpeedSprint)
                {
                    Player_RigidBody.velocity = new Vector2(maxMoveSpeedSprint * Input.GetAxisRaw("Horizontal"), Player_RigidBody.velocity.y);
                }
                else
                {
                    Player_RigidBody.velocity = new Vector2(moveSpeed, Player_RigidBody.velocity.y);
                }
            }
            else
            {
                if (Mathf.Abs(Player_RigidBody.velocity.x) > maxMoveSpeedNormal)
                {
                    Player_RigidBody.velocity = new Vector2(maxMoveSpeedNormal * Input.GetAxisRaw("Horizontal"), Player_RigidBody.velocity.y);
                }
                else
                {
                    Player_RigidBody.velocity = new Vector2(moveSpeed, Player_RigidBody.velocity.y);
                }
            }


            #endregion



            #region Vertical movment

            if (verticalControlOff_timeLeft < 0)
            {
                if (!grounded)
                {
                    groundedRememberTime -= Time.fixedDeltaTime;
                }
                else
                {
                    groundedRememberTime = setGroundedRememberTime;
                }

                jumpPressedRemember -= Time.fixedDeltaTime;
                if (Input.GetAxisRaw("Vertical") == 1)
                {
                    jumpPressedRemember = setJumpPressedRememberTime;
                }
                if (groundedRememberTime > 0 && jumpPressedRemember > 0)
                {
                    var jumpVelocity = jumpForce * 5;

                    Player_RigidBody.velocity = new Vector2(Player_RigidBody.velocity.x, jumpVelocity);
                }

                if (Player_RigidBody.velocity.y < 0)
                {
                    //Player_RigidBody.velocity += (Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime);
                    Player_RigidBody.velocity = new Vector2(Player_RigidBody.velocity.x, Player_RigidBody.velocity.y + Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime);
                }
                else if (Player_RigidBody.velocity.y > 0 && Input.GetAxisRaw("Vertical") != 1)
                {
                    //Player_RigidBody.velocity += (Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime);
                    Player_RigidBody.velocity = new Vector2(Player_RigidBody.velocity.x, Player_RigidBody.velocity.y + Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime);
                }
            }

            #endregion
        }
    }

    private void turnOffControl(float time)
    {
        controlOff_timeLeft = time;

        jumpPressedRemember -= time;
        groundedRememberTime -= time;
    }

    private void turnOffControlVertical(float time)
    {
        verticalControlOff_timeLeft = time;

        jumpPressedRemember -= time;
        groundedRememberTime -= time;
    }

    private void OnDestroy()
    {
        //Events.turnOffControl -= turnOffControl;
        //Events.turnOffControlVertical -= turnOffControlVertical;
    }

}
