using UnityEngine;

public class Player_FallState : Player_AiredState
{
    public Player_FallState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    override public void Update()
    {
        base.Update();

        if (player.groundDetected)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
