using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrapplingHookInput: IGrapplingHookInput
{
    public PlayerGrapplingHookInput(GameObject playerGameObject)
    {
        readGrapplingHookInput = playerGameObject.AddComponent<PlayerGrapplingHookReadInput>();
    }
    
    private readonly PlayerGrapplingHookReadInput readGrapplingHookInput;
    
    public Vector2 aimInput => readGrapplingHookInput.aimInput;
    
    public float grappleInput => readGrapplingHookInput.grappleInput;
    public float prevGrappleInput => readGrapplingHookInput.prevGrappleInput;
}

public class PlayerGrapplingHookReadInput : MonoBehaviour
{
    internal Vector2 aimInput { get; private set; }
    internal float grappleInput { get; private set; }
    internal float prevGrappleInput { get; private set; }

    private void OnAim(InputValue input)
    {
        aimInput = input.Get<Vector2>();
    }
    private void OnGrapple(InputValue input)
    {
        grappleInput = input.Get<float>();
    }
    
    private void LateUpdate()
    {
        prevGrappleInput = grappleInput;
    }
}