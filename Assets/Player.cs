using UnityEngine;

public class Player : MonoBehaviour
{
    // [ContextMenu("Special attack!")]

    private StateMachine stateMachine;

    private Player_IdleState idleState;

    private void Awake()
    {
        stateMachine = new StateMachine();
        idleState = new Player_IdleState(stateMachine, "Idle");
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
