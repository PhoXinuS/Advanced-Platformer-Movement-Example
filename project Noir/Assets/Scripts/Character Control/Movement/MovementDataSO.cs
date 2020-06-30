using UnityEngine;

[CreateAssetMenu(fileName = "Movement Data", menuName = "Movement Data", order = 0)]
public class MovementDataSO : ScriptableObject
{
    public float horizontalSpeed = 4f;
    public float crouchSpeedMultiplier = 0.4f;
    public float accelerationTime = 0.2f;
    public float decelerationTime = 0.1f;
    
    public float jumpHeight = 8f;
    public int availableJumps = 2;
}
