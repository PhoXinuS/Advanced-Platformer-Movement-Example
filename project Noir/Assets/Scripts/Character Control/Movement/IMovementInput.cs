using UnityEngine;

public interface IMovementInput
{
    float horizontalInput { get; }
    float jumpInput { get; }
    float crouchInput { get; }
}
    
