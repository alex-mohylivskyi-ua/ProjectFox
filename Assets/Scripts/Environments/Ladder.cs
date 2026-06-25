using UnityEngine;

public class Ladder : MonoBehaviour
{
    [Header("Top Exit")]
    [SerializeField] private bool allowTopExit = true;
    public bool AllowTopExit => allowTopExit;
    
    [Header("Centering")]
    [SerializeField] private bool centerPlayerOnLadder = true;
    [SerializeField, Min(0f)] private float centerSpeed = 12f;
    [SerializeField, Min(0f)] private float centerSnapDistance = 0.03f;

    public bool CenterPlayerOnLadder => centerPlayerOnLadder;
    public float CenterSpeed => centerSpeed;
    public float CenterSnapDistance => centerSnapDistance;
    public float CenterX => transform.position.x;
    
    
    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();

        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();

        if (player == null)
        {
            return;
        }

        player.SetCurrentLadder(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();

        if (player == null)
        {
            return;
        }

        player.ClearCurrentLadder(this);
    }
}