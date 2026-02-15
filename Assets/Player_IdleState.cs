using UnityEngine;

public class Player_IdleState : EntityState
{
    public Player_IdleState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {

    }

    override public void Update()
    {
        base.Update();

        // if (Input.GetKeyDown(KeyCode.D))
        // {
        //     stateMachine.ChangeState(player.moveState);
        // }

        // if (Keyboard.current.spaceKey.wasPressedThisFrame)
        // {
        //     stateMachine.ChangeState(player.moveState);
        // }
    }   
}
