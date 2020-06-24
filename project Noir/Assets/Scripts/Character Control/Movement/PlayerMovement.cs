using UnityEngine;
using UnityEngine.InputSystem;

[SerializeField]
public class PlayerMovement : IMovementInput
{
    private PlayerMovementVertical verticalMovement;
    public PlayerMovement(GameObject playerGameObject)
    {
        verticalMovement = playerGameObject.AddComponent<PlayerMovementVertical>();
    }

    public Vector2 movementInputNormalized => GetMovementInput();

    private Vector2 GetMovementInput()
    {
        var verticalInput = verticalMovement.verticalInput;
        return new Vector2(verticalInput, 0f);
    }
}

public class PlayerMovementVertical : MonoBehaviour
{
    internal float verticalInput { get; private set; }

    private void OnMovement(InputValue input)
    {
        verticalInput = input.Get<float>();
    }
}