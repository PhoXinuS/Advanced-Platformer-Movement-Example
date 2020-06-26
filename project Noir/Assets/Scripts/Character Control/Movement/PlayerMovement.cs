using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : IMovementInput
{
    public PlayerMovement(GameObject playerGameObject)
    {
        readInputMovement = playerGameObject.AddComponent<PlayerMovementReadInput>();
    }
    
    private readonly PlayerMovementReadInput readInputMovement;
    public float horizontalInput => readInputMovement.horizontalInput;
    public float jumpInput => readInputMovement.jumpInput;
    public float crouchInput=> readInputMovement.crouchInput;
}

public class PlayerMovementReadInput : MonoBehaviour
{
    internal float horizontalInput { get; private set; }
    internal float jumpInput { get; private set; }
    internal float crouchInput { get; private set; }

    private void OnHorizontal(InputValue input)
    {
        horizontalInput = input.Get<float>();
    }
    private void OnJump(InputValue input)
    {
        jumpInput = input.Get<float>();
    }
    private void OnCrouch(InputValue input)
    {
        crouchInput = input.Get<float>();
    }
}