using UnityEngine;
using System.Collections;

public static class JuiceAnimations
{
    // THIS IS THE MISSING METHOD
    public static IEnumerator GameOverPunch(Transform target, float intensity, float duration)
    {
        Vector3 originalPos = target.localPosition;
        Vector3 originalScale = target.localScale;
        Vector3 punchScale = originalScale * 1.15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Using unscaledDeltaTime is key so it doesn't freeze on Game Over
            elapsed += Time.unscaledDeltaTime;

            // Shake
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;
            target.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            // Scale Bounce
            float t = elapsed / duration;
            if (t < 0.2f)
                target.localScale = Vector3.Lerp(originalScale, punchScale, t / 0.2f);
            else
                target.localScale = Vector3.Lerp(punchScale, originalScale, (t - 0.2f) / 0.8f);

            yield return null;
        }

        target.localPosition = originalPos;
        target.localScale = originalScale;
    }

    public static IEnumerator SpringyPulse(Transform target, Vector3 targetScale, float duration)
    {
        Vector3 startScale = target.localScale;
        float elapsed = 0f;

        while (elapsed < duration * 0.4f)
        {
            elapsed += Time.unscaledDeltaTime;
            target.localScale = Vector3.Lerp(startScale, targetScale, elapsed / (duration * 0.4f));
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < duration * 0.6f)
        {
            elapsed += Time.unscaledDeltaTime;
            target.localScale = Vector3.Lerp(targetScale, startScale, elapsed / (duration * 0.6f));
            yield return null;
        }

        target.localScale = startScale;
    }

    public static IEnumerator Shake(Transform target, float intensity, float duration)
    {
        Vector3 originalPos = target.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;
            target.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            yield return null;
        }

        target.localPosition = originalPos;
    }
}