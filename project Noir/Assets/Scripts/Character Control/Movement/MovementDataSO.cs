using UnityEngine;

[CreateAssetMenu(fileName = "Movement Data", menuName = "Movement Data", order = 0)]
public class MovementDataSO : ScriptableObject
{
    public float horizontalSpeed = 8f;
    public float crouchSpeedMultiplier = 0.4f;
    public float accelerationTime = 0.2f;
    public float decelerationTime = 0.1f;

    public float slideDecelerationTime = 1f;
    public float slideDuration = 0.8f;
    public float wallSlideSpeed = 2f;
    public float wallClimbSpeed = 6f;
    
    public float jumpHeight = 8f;
    public int availableJumps = 2;
}
