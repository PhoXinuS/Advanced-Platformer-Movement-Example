using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : IMovementInput
{
    public PlayerMovement(GameObject playerGameObject)
    {
        readInputMovement = playerGameObject.AddComponent<PlayerMovementReadInput>();
    }
    
    private readonly PlayerMovementReadInput readInputMovement;
    public Vector2 movementInputNormalized => readInputMovement.movementInput;
}

public class PlayerMovementReadInput : MonoBehaviour
{
    internal Vector2 movementInput { get; private set; }

    private void OnMovement(InputValue input)
    {
        movementInput = input.Get<Vector2>();
    }
}