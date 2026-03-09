using UnityEngine;
using System.Collections;

public class Entity_VFX : MonoBehaviour
{
    private SpriteRenderer sr;
    
    [Header("On damage VFX")]
    [SerializeField] private Material onDamageMaterial;
    [SerializeField] private float onDamageVFXDuration = .2f;

    private Material originalMaterial;
    private Coroutine damageVFXCoroutine;
    
    private void  Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = sr.material;
    }

    public void PlayOnDamageVFX()
    {
        if (damageVFXCoroutine != null)
        {
            StopCoroutine(damageVFXCoroutine);
        }

        damageVFXCoroutine = StartCoroutine(OnDamageVFXCoroutine());
    }

    private IEnumerator OnDamageVFXCoroutine()
    {
        sr.material = onDamageMaterial;
        
        yield return new WaitForSeconds(onDamageVFXDuration);
        sr.material = originalMaterial;
        damageVFXCoroutine = null;
    }
}
