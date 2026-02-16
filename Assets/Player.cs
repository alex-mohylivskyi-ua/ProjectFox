using UnityEngine;

public class Player : MonoBehaviour
{
    // [ContextMenu("Special attack!")]

    private StateMachine stateMachine;
    private PlayerInputSet input;
    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Vector2 moveInput { get; private set; }
    public Animator anim { get; private set; }

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();

        input = new PlayerInputSet();
        stateMachine = new StateMachine();

        idleState = new Player_IdleState(this, stateMachine, "idle");
        moveState = new Player_MoveState(this, stateMachine, "move");
    }
    
    private void OnEnable()
    {
        input.Enable();
        // input.Player.Movement.started
        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Start()
    {
        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        // It runs Update inside EntityState without having MonoBehaviour
        stateMachine.UpdateActiveState();
    }
}
