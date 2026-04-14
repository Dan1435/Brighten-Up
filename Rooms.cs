using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Rooms : MonoBehaviour, IPointerDownHandler
{
    [Header("Sprites")]
    public Sprite lightSprite;
    public Sprite darkSprite;
    public Sprite redSprite;

    [Header("Room Settings")]
    public float roomWidth = 2.0f;
    public float floorYOffset = -80f; // Adjusted for UI pixel space

    private Image sr;
    private CharchterManager manager;
    public bool IsLit { get; private set; }

    void Awake()
    {
        sr = GetComponent<Image>();
        manager = Object.FindFirstObjectByType<CharchterManager>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (manager == null || manager.isGameOver || Time.timeScale <= 0) return;

        IsLit = !IsLit;

        if (sr != null)
            sr.sprite = IsLit ? lightSprite : darkSprite;

        manager.UpdateCharactersInRoom(this);

        if (!IsCurrentlyIncorrect())
        {
            StartCoroutine(JuiceAnimations.SpringyPulse(transform, transform.localScale * 1.05f, 0.1f));
        }
    }

    private bool IsCurrentlyIncorrect()
    {
        if (manager == null) return false;

        bool needsLight = false;
        foreach (Character c in manager.characters)
        {
            if (c != null && c.gameObject.activeSelf && c.currentRoom == this && !c.isDog)
            {
                needsLight = true;
                break;
            }
        }
        return (needsLight && !IsLit) || (!needsLight && IsLit);
    }

    public void FlashRed()
    {
        StopAllCoroutines();
        if (sr != null) sr.sprite = redSprite;
        // The dopamine red shake!
        StartCoroutine(JuiceAnimations.GameOverPunch(transform, 12f, 0.45f));
    }

    public void MenuToggle()
    {
        IsLit = !IsLit;
        if (sr != null) sr.sprite = IsLit ? lightSprite : darkSprite;
    }
}