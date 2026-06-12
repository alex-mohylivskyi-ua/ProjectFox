using UnityEngine;

public class PlayerMovement
{
    private Rigidbody2D rb;
    private Player player;

    public PlayerMovement(Player player, Rigidbody2D rb)
    {
        this.player = player;
        this.rb = rb;
    }

    // public void Move(float xInput, float speed)
    // {
    //     rb.velocity = new Vector2(xInput * speed, rb.velocity.y);
    // }

    public void Jump(float jumpForce)
    {
        player.SetVelocity(rb.linearVelocity.x, jumpForce);
    }

    public void JumpCut(float multiplier)
    {
        if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * multiplier);
        }
    }
    
}