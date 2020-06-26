using UnityEngine;

[CreateAssetMenu(fileName = "Movement Data", menuName = "Movement Data", order = 0)]
public class MovementDataSO : ScriptableObject
{
    public float horizontalSpeed = 1f;
    public float accelerationTime = 1f;
    public float decelerationTime = 1f;
    
    public float jumpHeight = 1f;
    public int availableJumps = 2;
}
