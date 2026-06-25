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
    
    public void ClimbLadder(float xInput, float yInput, float climbSpeed, float horizontalSpeedMultiplier)
    {
        float xVelocity = xInput * player.MovementData.moveSpeed * horizontalSpeedMultiplier;
        float yVelocity = yInput * climbSpeed;

        player.SetVelocity(xVelocity, yVelocity);
    }
    
    public void ClimbLadderCentered(float yInput, float climbSpeed, float ladderCenterX, float centerSpeed, float snapDistance)
    {
        float xDifference = ladderCenterX - rb.position.x;
        float xVelocity = Mathf.Sign(xDifference) * centerSpeed;

        if (Mathf.Abs(xDifference) <= snapDistance)
        {
            rb.position = new Vector2(ladderCenterX, rb.position.y);
            xVelocity = 0f;
        }

        float yVelocity = yInput * climbSpeed;

        player.SetVelocity(xVelocity, yVelocity);
    }

    public void JumpFromLadder(float jumpForce)
    {
        
        player.SetVelocity(rb.linearVelocity.x, jumpForce);
    }

    public void Jump(float jumpForce)
    {
        player.SetVelocity(rb.linearVelocity.x, jumpForce);
    }

    public void WallJump()
    {
        player.SetVelocity(
            player.MovementData.wallJumpForce.x * -player.facingDirection,
            player.MovementData.wallJumpForce.y);
        }

    public void JumpCut(float multiplier, float minVelocity)
    {
        if (rb.linearVelocity.y > 0)
        {
            float currentYVelocity = rb.linearVelocity.y;
            float newYVelocity = currentYVelocity * multiplier;
            
            if (minVelocity > 0)
            {
                newYVelocity = Mathf.Max(newYVelocity, minVelocity);
                newYVelocity = Mathf.Min(newYVelocity, currentYVelocity);
            }
            
            player.SetVelocity(rb.linearVelocity.x, newYVelocity);
            
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
    
    public void AirMoveWithApexControl(
        float xInput,
        float speed,
        float airMoveMultiplier,
        float deceleration,
        float apexThreshold,
        float apexMoveMultiplier
    )
    {
        float finalMultiplier = airMoveMultiplier;

        if (apexThreshold > 0 && Mathf.Abs(rb.linearVelocity.y) < apexThreshold)
        {
            finalMultiplier *= apexMoveMultiplier;
        }

        AirMove(xInput, speed, finalMultiplier, deceleration);
    }
    
    public void ApplyFallGravity(float fallGravityMultiplier, float maxFallSpeed)
    {
        if (rb.linearVelocity.y >= 0)
        {
            return;
        }

        float extraGravity = Physics2D.gravity.y * (fallGravityMultiplier - 1f) * Time.deltaTime;
        float newYVelocity = rb.linearVelocity.y + extraGravity;

        newYVelocity = Mathf.Max(newYVelocity, -maxFallSpeed);

        player.SetVelocity(rb.linearVelocity.x, newYVelocity);
    }

    public void PlayerAlignCenter(Ladder ladder)
    {
        float ladderCenterX = ladder.GetComponent<BoxCollider2D>().bounds.center.x;
        player.transform.position = new Vector2(ladderCenterX, player.transform.position.y);
    }

}