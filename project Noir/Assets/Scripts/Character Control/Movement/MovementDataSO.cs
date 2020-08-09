using UnityEngine;

[CreateAssetMenu(fileName = "Movement Data", menuName = "Movement Data", order = 0)]
public class MovementDataSO : ScriptableObject
{
    public bool calculateVertical = true;
    public bool calculateHorizontal = true;
    public bool calculateClimb = true;
    public bool calculateSlide = true;
    public bool calculateCrouch = true;
    
    public float horizontalSpeed = 8f;
    public float crouchSpeedMultiplier = 0.4f;
    public float accelerationTime = 0.2f;
    public float decelerationTime = 0.1f;
    public float spaceAccelerationTime = 1f;
    public float spaceDecelerationTime = 0.8f;  
    public float climbAccelerationTime = 0.3f;
    public float climbDecelerationTime = 0.2f;

    public float slideDecelerationTime = 1f;
    public float slideDuration = 0.8f;
    public float wallSlideSpeed = 2f;
    public float wallClimbSpeed = 6f;
    public float ceilingClimbSpeed = 4f;

    public float wallJumpHorizontalPower = 6f;
    public float wallJumpVerticalPower = 6f;
    public float wallJumpOffControlTime = 0.4f;
    public float jumpHeight = 8f;
    public int availableJumps = 2;
}
