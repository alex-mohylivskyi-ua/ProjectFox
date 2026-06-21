using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;

// Player
//     відповідає за:
// input
//     таймінги (buffer, coyote time)
// рішення

public class Player : Entity
{
    private PlayerInput playerInput;
    public PlayerInputReader inputReader { get; private set; }
    public Vector2 moveInput => inputReader.moveInput;
    
    public PlayerAbilities abilities { get; private set; }
    private PlayerOneWayPlatformDrop oneWayPlatformDrop;
    private PlayerInteraction interaction;
    
    [Header("One way platform")]
    [SerializeField, Min(0f)] private float ignoreGroundAfterOneWayDropDuration = 0.15f;

    [Header("Data")] 
    // На перший погляд здається: “Навіщо другий рядок? Чому просто не брати movementData напряму?”
    // Але відповідь така:
    // movementData потрібен Unity Inspector-у, щоб ми могли призначити asset.
    // MovementData потрібен іншим класам, щоб вони могли читати цей asset без права напряму змінювати поле
    [SerializeField] private PlayerMovementData movementData;
    [SerializeField] private PlayerCombatData combatData;
    public PlayerMovementData MovementData => movementData;
    // Ось що означає ця стрілка, це простий геттер
    // public PlayerMovementData MovementData
    // {
    //     get
    //     {
    //         return movementData;
    //     }
    // }
    public PlayerCombatData CombatData => combatData;
    
    
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
    public bool droppedThroughOneWayPlatformThisFrame { get; private set; }
    private float ignoreGroundAfterOneWayDropTimer;
    public bool shouldIgnoreGroundAfterOneWayDrop => ignoreGroundAfterOneWayDropTimer > 0;
    
    private float coyoteTimer;
    public bool canUseCoyoteJump => coyoteTimer > 0;
    
    // HUD start
    public float coyoteTimeLeft => coyoteTimer;
    public float jumpBufferTimeLeft => jumpBufferTimer;
    public float ignoreGroundAfterOneWayDropTimeLeft => ignoreGroundAfterOneWayDropTimer;
    // HUD end
    
    // Events
    // Carries the Player payload so listeners can tell WHICH player died (local co-op).
    public event Action<Player> OnPlayerDeath;
    

    // Here we use Entity_Health controller even without requiring it;
    public override void EntityDeath()
    {
        // Here we trigger event
        OnPlayerDeath?.Invoke(this);
        
        // We set false to prevent Enemy detecting player, what triggers moving to Enemy_BattleState
        rb.simulated = false;
        
        base.EntityDeath();
        
        
        Debug.Log("Player is dead");
        stateMachine.ChangeState(deadState);
    }

    protected override void Awake()
    {
        base.Awake();
        
        playerInput = GetComponent<PlayerInput>();
        inputReader = new PlayerInputReader(playerInput);
        movement = new PlayerMovement(this, rb);
        abilities = GetComponent<PlayerAbilities>();
        oneWayPlatformDrop = GetComponent<PlayerOneWayPlatformDrop>();
        interaction = GetComponent<PlayerInteraction>();
        
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
        interaction?.HandleInteraction();
        
        droppedThroughOneWayPlatformThisFrame = false;
        HandleOneWayPlatformGroundIgnoreTimer();
        
        if (oneWayPlatformDrop != null && oneWayPlatformDrop.TryDropThroughPlatform())
        {
            droppedThroughOneWayPlatformThisFrame = true;
            StartIgnoringGroundAfterOneWayDrop();
            ClearJumpBuffer();
            ConsumeCoyoteTime();
        }
        else
        {
            HandleJumpBuffer();
        }
        
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
            jumpBufferTimer = MovementData.jumpBufferTime;
            bufferedJumpReleased = false;
        }
        else if (jumpBufferTimer > 0)
        {
            jumpBufferTimer -= Time.deltaTime;
        }
    }
    
    private void StartIgnoringGroundAfterOneWayDrop()
    {
        ignoreGroundAfterOneWayDropTimer = ignoreGroundAfterOneWayDropDuration;
    }

    private void HandleOneWayPlatformGroundIgnoreTimer()
    {
        if (ignoreGroundAfterOneWayDropTimer > 0)
        {
            ignoreGroundAfterOneWayDropTimer -= Time.deltaTime;
        }
    }
    
    public void ConsumeJumpBuffer()
    {
        bufferedJumpReleased = !inputReader.jumpHeld;
        jumpBufferTimer = 0;
    }
    
    public void ClearJumpBuffer()
    {
        bufferedJumpReleased = false;
        jumpBufferTimer = 0;
    }
    
    public void ClearBufferedJumpRelease()
    {
        bufferedJumpReleased = false;
    }
    
    private void HandleCoyoteTime()
    {
        if (shouldIgnoreGroundAfterOneWayDrop)
        {
            coyoteTimer = 0;
            return;
        }
        
        if (groundDetected)
        {
            coyoteTimer = MovementData.coyoteTime;
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
