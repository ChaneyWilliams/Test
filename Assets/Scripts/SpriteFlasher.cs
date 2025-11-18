using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SpriteFlasher : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public bool hit = false;
    public bool isFlashing = false;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }


    public IEnumerator Flash(float flashDuration, Color flashColor, float numberOfFlashes)
    {

        isFlashing = true;
        Color startColor = spriteRenderer.color;
        float elapsedFlashtime = 0;
        float elapsedFlashPercent = 0;

        while (elapsedFlashtime < flashDuration && spriteRenderer != null)
        {
            elapsedFlashtime += Time.deltaTime;
            elapsedFlashPercent = elapsedFlashtime / flashDuration;
            if (elapsedFlashPercent > 1)
            {
                elapsedFlashPercent = 1;
            }

            float pingPongPercent = Mathf.PingPong(elapsedFlashPercent * numberOfFlashes * 2, 1);
            spriteRenderer.color = Color.Lerp(startColor, flashColor, pingPongPercent);
            yield return null;
        }
        isFlashing = false;
    }
}
