using UnityEngine;
using System.Collections;
using System;

// Player
//     відповідає за:
// input
//     таймінги (buffer, coyote time)
// рішення

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
    public PlayerMovement movement { get; private set; }



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
    [SerializeField] public float jumpForce = 20;
    [SerializeField] public float jumpBufferTime = 0.1f;
    private float jumpBufferTimer;
    public bool jumpBuffered => jumpBufferTimer > 0;
    public bool bufferedJumpReleased { get; private set; }
    [Range(0.1f, 1f)] public float jumpCutMultiplier = 0.4f;
    public Vector2 wallJumpForce;
    [Range(0, 1)]
    public float airMoveMultiplyier = 0.9f; // TODO HK 0.9 - 1
    [Range(0, 100)]
    public float airMoveDeceleration = 10f; // Increase
    [Range(0, 1)]
    public float wallSlideSlowMultiplyier = 0.3f;
    
    public Vector2 moveInput { get; private set; }
    public float dashSpeed = 10;
    public float dashDuration = 0.25f;
    
    
    // Events
    public static event Action OnPlayerDeath;
    

    // Here we use Entity_Health controller even without requiring it;
    public override void EntityDeath()
    {
        // Here we trigger event
        OnPlayerDeath?.Invoke();
        
        // We set false to prevent Enemy detecting player, what triggers moving to Enemy_BattleState
        rb.simulated = false;
        
        base.EntityDeath();
        
        
        Debug.Log("Player is dead");
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

        movement = new PlayerMovement(this, rb);
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
    
    protected override void Update()
    {
        HandleJumpBuffer();
        base.Update();
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

    private void HandleJumpBuffer()
    {
        if (input.Player.Jump.WasPressedThisFrame())
        {
            jumpBufferTimer = jumpBufferTime;
            bufferedJumpReleased = false;
        }
        else if (jumpBufferTimer > 0)
        {
            jumpBufferTimer -= Time.deltaTime;
        }
    }
    
    public void ConsumeJumpBuffer()
    {
        bufferedJumpReleased = !input.Player.Jump.IsPressed();
        jumpBufferTimer = 0;
    }
    
    public void ClearBufferedJumpRelease()
    {
        bufferedJumpReleased = false;
    }
}
