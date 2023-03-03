using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float duration;
    [SerializeField] private int flashCount = 1;



    private SpriteRenderer spriteRenderer;

    private Material originalMaterial;

    private Coroutine flashRoutine;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
    }
    public void StartFlash()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        
        if(FeedbackController.Instance.HitEffect)
            flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        float flashTime = duration/(2 * flashCount - 1);
        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.material = flashMaterial;
            yield return new WaitForSeconds(flashTime);
            spriteRenderer.material = originalMaterial;
            if (i < flashCount - 1)
            {
                yield return new WaitForSeconds(flashTime);
            }
        }
        flashRoutine = null;
    }


}
