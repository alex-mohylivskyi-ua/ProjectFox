using UnityEngine;

public class Chest : MonoBehaviour, IDamagable
{
    [SerializeField] private Vector2 knockBack = new Vector2(0, 5);

    private Animator anim => GetComponentInChildren<Animator>();
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();
    private Entity_VFX vfx => GetComponent<Entity_VFX>();

    public void TakeDamage(float damage, Transform damageDealer)
    {
        // throw new System.NotImplementedException();
        Debug.Log("chest attacked");
        vfx.PlayOnDamageVFX();
        anim.SetBool("chestOpen", true);
        
        
        if (rb != null)
        {
            rb.linearVelocity = knockBack;
            rb.angularVelocity = Random.Range(-200f, 200f);
            
            // Drop items
        }
    }
}