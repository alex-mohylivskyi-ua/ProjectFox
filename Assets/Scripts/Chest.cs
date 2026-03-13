using UnityEngine;

public class Chest : MonoBehaviour, IDamagable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage, Transform damageDealer)
    {
        // throw new System.NotImplementedException();
        Debug.Log("chest attacked");
        GetComponentInChildren<Animator>().SetBool("chestOpen", true);
    }
}
