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
    public PlayerInputReader inputReader { get; private set; }
    public Vector2 moveInput => inputReader.moveInput;
    
    public PlayerAbilities abilities { get; private set; }
    
    
    [Header("Data")]
    [SerializeField] private PlayerMovementData movementData;
    [SerializeField] private PlayerCombatData combatData;
    public PlayerMovementData MovementData => movementData;
    public PlayerCombatData CombatData => combatData;
    
    // Soft transition proxies.
    // Existing states can still use player.moveSpeed, player.jumpForce, etc.
    public float moveSpeed => movementData.moveSpeed;
    public float jumpForce => movementData.jumpForce;
    public float jumpBufferTime => movementData.jumpBufferTime;
    public float coyoteTime => movementData.coyoteTime;
    public float jumpCutMultiplier => movementData.jumpCutMultiplier;
    public float jumpCutMinVelocity => movementData.jumpCutMinVelocity;
    public Vector2 wallJumpForce => movementData.wallJumpForce;
    public float airMoveMultiplier => movementData.airMoveMultiplier;
    public float airMoveDeceleration => movementData.airMoveDeceleration;
    public float apexThreshold => movementData.apexThreshold;
    public float apexMoveMultiplier => movementData.apexMoveMultiplier;
    public float fallGravityMultiplier => movementData.fallGravityMultiplier;
    public float maxFallSpeed => movementData.maxFallSpeed;
    public float wallSlideSlowMultiplier => movementData.wallSlideSlowMultiplier;
    public float dashSpeed => movementData.dashSpeed;
    public float dashDuration => movementData.dashDuration;
    
    // Combat data proxies.
    // Existing states can still use player.attackVelocity, player.comboResetTime, etc.
    public Vector2[] attackVelocity => combatData.attackVelocity;
    public float attackVelocityDuration => combatData.attackVelocityDuration;
    public float comboResetTime => combatData.comboResetTime;
    public Vector2 jumpAttackVelocity => combatData.jumpAttackVelocity;


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
    private Coroutine queuedAttackCo;
    private float jumpBufferTimer;
    public bool jumpBuffered => jumpBufferTimer > 0;
    public bool bufferedJumpReleased { get; private set; }
   
    private float coyoteTimer;
    public bool canUseCoyoteJump => coyoteTimer > 0;
    
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
        inputReader = new PlayerInputReader(input);
        movement = new PlayerMovement(this, rb);
        abilities = GetComponent<PlayerAbilities>();
        
        if (abilities == null)
        {
            Debug.LogError($"{nameof(PlayerAbilities)} is not assigned on {name}.", this);
        }
        
        if (movementData == null)
        {
            Debug.LogError($"{nameof(PlayerMovementData)} is not assigned on {name}.", this);
        }
        
        if (combatData == null)
        {
            Debug.LogError($"{nameof(PlayerCombatData)} is not assigned on {name}.", this);
        }
        
        // States
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
        inputReader.Enable();
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }
    
    protected override void Update()
    {
        inputReader.ReadInput();
        HandleJumpBuffer();
        base.Update();
        HandleCoyoteTime();
    }

    private void OnDisable()
    {
        inputReader.Disable();
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
        if (inputReader.jumpPressed)
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
        bufferedJumpReleased = !inputReader.jumpHeld;
        jumpBufferTimer = 0;
    }
    
    public void ClearBufferedJumpRelease()
    {
        bufferedJumpReleased = false;
    }
    
    private void HandleCoyoteTime()
    {
        if (groundDetected)
        {
            coyoteTimer = coyoteTime;
            return;
        }

        if (coyoteTimer > 0)
        {
            coyoteTimer -= Time.deltaTime;
        }
    }
    
    public void ConsumeCoyoteTime()
    {
        coyoteTimer = 0;
    }
}
