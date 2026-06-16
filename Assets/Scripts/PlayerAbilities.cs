using UnityEngine;
// Чому це компонент, а не ScriptableObject?
//     Бо abilities — це стан конкретного гравця.
//     Наприклад:
// Player 1 відкрив dash;
// Player 2 ще ні;
// після завантаження сейву abilities зміняться;
// під час cutscene можна тимчасово заблокувати dash;
// upgrade pickup може викликати UnlockDash().
//     Тому для abilities краще MonoBehaviour на Player, а не shared asset.
public class PlayerAbilities : MonoBehaviour
{
    [Header("Movement abilities")]
    [SerializeField] private bool canDash = true;
    [SerializeField] private bool canWallSlide = true;
    [SerializeField] private bool canWallJump = true;
    [SerializeField] private bool canDoubleJump = false;

    [Header("Combat abilities")]
    [SerializeField] private bool canAirAttack = true;

    public bool CanDash => canDash;
    public bool CanWallSlide => canWallSlide;
    public bool CanWallJump => canWallJump;
    public bool CanDoubleJump => canDoubleJump;
    public bool CanAirAttack => canAirAttack;

    public void UnlockDash()
    {
        canDash = true;
    }

    public void UnlockWallSlide()
    {
        canWallSlide = true;
    }

    public void UnlockWallJump()
    {
        canWallJump = true;
    }

    public void UnlockDoubleJump()
    {
        canDoubleJump = true;
    }

    public void UnlockAirAttack()
    {
        canAirAttack = true;
    }

    public void LockDash()
    {
        canDash = false;
    }

    public void LockWallSlide()
    {
        canWallSlide = false;
    }

    public void LockWallJump()
    {
        canWallJump = false;
    }

    public void LockDoubleJump()
    {
        canDoubleJump = false;
    }

    public void LockAirAttack()
    {
        canAirAttack = false;
    }
}