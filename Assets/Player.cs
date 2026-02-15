using UnityEngine;

public class Player : MonoBehaviour
{
    // [ContextMenu("Special attack!")]

    private StateMachine stateMachine;

    private EntityState idleState;

    private void Awake()
    {
        stateMachine = new StateMachine();
        idleState = new EntityState(stateMachine, "Idle State");
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
