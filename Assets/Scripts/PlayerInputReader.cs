using UnityEngine;

public class PlayerInputReader
{
    private readonly PlayerInputSet input;

    public Vector2 moveInput { get; private set; }
    public bool jumpPressed { get; private set; }
    public bool jumpHeld { get; private set; }
    public bool attackPressed { get; private set; }
    public bool dashPressed { get; private set; }

    public PlayerInputReader(PlayerInputSet input)
    {
        this.input = input;
    }

    public void Enable()
    {
        input.Enable();
    }

    public void Disable()
    {
        input.Disable();
    }

    public void ReadInput()
    {
        moveInput = input.Player.Movement.ReadValue<Vector2>();
        jumpPressed = input.Player.Jump.WasPressedThisFrame();
        jumpHeld = input.Player.Jump.IsPressed();
        attackPressed = input.Player.Attack.WasPressedThisFrame();
        dashPressed = input.Player.Dash.WasPressedThisFrame();
    }
}