using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader
{
    private readonly PlayerInput playerInput;
    private readonly InputAction movementAction;
    private readonly InputAction jumpAction;
    private readonly InputAction attackAction;
    private readonly InputAction dashAction;

    public Vector2 moveInput { get; private set; }
    public bool jumpPressed { get; private set; }
    public bool jumpHeld { get; private set; }
    public bool attackPressed { get; private set; }
    public bool dashPressed { get; private set; }
    
    public PlayerInputReader(PlayerInput playerInput)
    {
        this.playerInput = playerInput;

        InputActionMap playerActionMap = playerInput.actions.FindActionMap("Player", true);

        movementAction = playerActionMap.FindAction("Movement", true);
        jumpAction = playerActionMap.FindAction("Jump", true);
        attackAction = playerActionMap.FindAction("Attack", true);
        dashAction = playerActionMap.FindAction("Dash", true);
    }

    public void Enable()
    {
        playerInput.actions.Enable();
    }

    public void Disable()
    {
        playerInput.actions.Disable();
    }

    public void ReadInput()
    {
        moveInput = movementAction.ReadValue<Vector2>();
        jumpPressed = jumpAction.WasPressedThisFrame();
        jumpHeld = jumpAction.IsPressed();
        attackPressed = attackAction.WasPressedThisFrame();
        dashPressed = dashAction.WasPressedThisFrame();
    }
}