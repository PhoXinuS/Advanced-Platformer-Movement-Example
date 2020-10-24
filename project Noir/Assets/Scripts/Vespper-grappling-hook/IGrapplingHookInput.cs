using UnityEngine;

public interface IGrapplingHookInput
{
    Vector2 aimInput { get; }
    float grappleInput { get; }
    float prevGrappleInput { get; }
}