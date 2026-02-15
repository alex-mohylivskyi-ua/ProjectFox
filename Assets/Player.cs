using UnityEngine;

public class Player : MonoBehaviour
{
    // [ContextMenu("Special attack!")]

    private StateMachine stateMachine;
    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }

    private void Awake()
    {
        stateMachine = new StateMachine();
        idleState = new Player_IdleState(this, stateMachine, "Idle");
        moveState = new Player_MoveState(this, stateMachine, "Move");
    }

    private void Start()
    {
        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        // It runs Update inside EntityState without having MonoBehaviour
        stateMachine.currentState.Update();
    }
}
