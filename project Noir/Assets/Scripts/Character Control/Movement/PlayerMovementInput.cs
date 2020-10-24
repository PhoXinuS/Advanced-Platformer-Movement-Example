using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementInput : IMovementInput
{
    public PlayerMovementInput(GameObject playerGameObject)
    {
        readInputMovement = playerGameObject.AddComponent<PlayerMovementReadInput>();
    }
    
    private readonly PlayerMovementReadInput readInputMovement;
    public float horizontalInput => readInputMovement.horizontalInput;
    public float verticalInput => readInputMovement.verticalInput;
    public float jumpInput => readInputMovement.jumpInput;
}

public class PlayerMovementReadInput : MonoBehaviour
{
    internal float horizontalInput { get; private set; }
    internal float jumpInput { get; private set; }
    internal float verticalInput { get; private set; }

    private void OnHorizontal(InputValue input)
    {
        horizontalInput = input.Get<float>();
    }
    private void OnVertical(InputValue input)
    {
        verticalInput = input.Get<float>();
    }
    private void OnJump(InputValue input)
    {
        jumpInput = input.Get<float>();
    }
}