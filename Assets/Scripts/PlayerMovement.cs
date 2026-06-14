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
            player.SetVelocity(rb.linearVelocity.x, rb.linearVelocity.y * multiplier);
            // rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * multiplier);
        }
    }

    public void AirMove(float xInput, float speed, float multiplier, float deceleration)
    {
        float targetSpeed = xInput * speed * multiplier;
        float newXVelocity;
        
        if (xInput != 0)
        {
            newXVelocity = targetSpeed;
        }
        else
        {
            newXVelocity = Mathf.MoveTowards(rb.linearVelocity.x, 0, deceleration * Time.deltaTime);
        }

        player.SetVelocity(newXVelocity, rb.linearVelocity.y);
    }

}