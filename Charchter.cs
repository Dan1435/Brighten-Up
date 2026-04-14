using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Required for Image component

public class Character : MonoBehaviour
{
    [Header("Sprite Settings")]
    public Sprite awakeLightSprite;
    public Sprite awakeDarkSprite; // Use this for the Silhouette

    public bool isDog = false;

    [Header("Placement Tweaks")]
    public float yOffsetAdjustment = 0f;
    public float xOffsetAdjustment = 0f;

    // CHANGED: Using Image instead of SpriteRenderer
    [HideInInspector] public Image sr;
    [HideInInspector] public Rooms currentRoom;
    private Vector3 originalScale;

    void Awake()
    {
        // CHANGED: Get Image component
        sr = GetComponent<Image>();
        originalScale = transform.localScale;
        if (sr != null) sr.enabled = true;
    }

    public void ForceStandardScale()
    {
        StopAllCoroutines();
        transform.localScale = originalScale;
        if (sr != null)
        {
            sr.enabled = true;
            if (currentRoom != null)
                sr.sprite = currentRoom.IsLit ? awakeLightSprite : awakeDarkSprite;
        }
    }

    public void SetRoom(Rooms newRoom, bool shouldFade = false)
    {
        StopAllCoroutines();
        if (sr != null) sr.enabled = true;
        transform.localScale = originalScale;
        StartCoroutine(AnimateMove(newRoom, shouldFade));
    }

    private IEnumerator AnimateMove(Rooms newRoom, bool shouldFade)
    {
        float duration = 0.12f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, elapsed / duration);
            yield return null;
        }

        currentRoom = newRoom;

        // CHANGED: Using localPosition for UI. 
        // This keeps the character inside the Canvas/House structure.
        transform.localPosition = new Vector3(
            newRoom.transform.localPosition.x + xOffsetAdjustment,
            newRoom.transform.localPosition.y + newRoom.floorYOffset + yOffsetAdjustment,
            0f // Z is usually 0 in UI
        );

        UpdateCharacterAppearance(false);

        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, elapsed / duration);
            yield return null;
        }
        transform.localScale = originalScale;
    }

    public void UpdateCharacterAppearance(bool shouldBounce = true)
    {
        if (currentRoom == null || sr == null) return;
        sr.enabled = true;

        sr.sprite = currentRoom.IsLit ? awakeLightSprite : awakeDarkSprite;

        if (shouldBounce && gameObject.activeInHierarchy)
        {
            StartCoroutine(JuiceAnimations.SpringyPulse(transform, originalScale * 1.04f, 0.1f));
        }
    }
}