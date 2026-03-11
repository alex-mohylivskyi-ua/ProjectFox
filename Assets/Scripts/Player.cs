using UnityEngine;
using System.Collections;

public class Player : Entity
{
    public PlayerInputSet input { get; private set; }

    // Player states
    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    public Player_FallState fallState { get; private set; }
    public Player_WallSlideState wallSlideState { get; private set; }
    public Player_WallJumpState wallJumpState { get; private set; }
    public Player_DashState dashState { get; private set; }
    public Player_BasicAttackState basicAttackState { get; private set; }
    public Player_JumpAttackState jumpAttackState { get; private set; }
    public Player_DeadState deadState { get; private set; }



    [Header("Attack details")]
    public Vector2[] attackVelocity;
    public float attackVelocityDuration = .1f;
    public float comboResetTime = 1;
    private Coroutine queuedAttackCo;

    [Header("Jump Attack details")]
    public Vector2 jumpAttackVelocity;

    [Header("Movement details")]
    [Range(0, 20)]
    public float moveSpeed;
    [Range(0, 20)]
    public float jumpForce = 5;
    public Vector2 wallJumpForce;
    
    public Vector2 moveInput { get; private set; }
    public float dashSpeed = 10;
    public float dashDuration = 0.25f;

    [Range(0, 1)]
    public float airMoveMultiplyier = 0.7f;
    [Range(0, 1)]
    public float wallSlideSlowMultiplyier = 0.7f;

    // Here we use Entity_Health controller even without requiring it;
    public override void EntityDeath()
    {
        base.EntityDeath();
        
        stateMachine.ChangeState(deadState);
    }

    protected override void Awake()
    {
        base.Awake();
        input = new PlayerInputSet();
        idleState = new Player_IdleState(this, stateMachine, "idle");
        moveState = new Player_MoveState(this, stateMachine, "move");
        jumpState = new Player_JumpState(this, stateMachine, "jumpFall");
        fallState = new Player_FallState(this, stateMachine, "jumpFall");
        wallSlideState = new Player_WallSlideState(this, stateMachine, "wallSlide");
        wallJumpState = new Player_WallJumpState(this, stateMachine, "jumpFall");
        dashState = new Player_DashState(this, stateMachine, "dash");
        basicAttackState = new Player_BasicAttackState(this, stateMachine, "basicAttack");
        jumpAttackState = new Player_JumpAttackState(this, stateMachine, "jumpAttack");
        deadState = new Player_DeadState(this, stateMachine, "dead");
    }

    protected void OnEnable()
    {
        input.Enable();
        // input.Player.Movement.started
        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    private void OnDisable()
    {
        input.Disable();
    }

    public void EnterAttackStateWithDelay()
    {
        if (queuedAttackCo != null)
        {
            StopCoroutine(queuedAttackCo);
        }
        queuedAttackCo = StartCoroutine(EnterAttackStateWithDelayCo());
    }

    private IEnumerator EnterAttackStateWithDelayCo()
    {
        yield return new WaitForEndOfFrame();
        stateMachine.ChangeState(basicAttackState);
    }
}
