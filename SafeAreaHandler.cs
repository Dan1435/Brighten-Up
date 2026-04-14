using UnityEngine;

public class SafeAreaHandler : MonoBehaviour
{
    RectTransform rectTransform;
    Rect lastSafeArea = new Rect(0, 0, 0, 0);

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        Refresh();
    }

    void Update()
    {
        // Check if the screen orientation or safe area changed
        if (lastSafeArea != Screen.safeArea)
        {
            Refresh();
        }
    }

    void Refresh()
    {
        Rect safeArea = Screen.safeArea;

        // Convert safe area rectangle from pixels to normalized anchors
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;

        lastSafeArea = safeArea;
    }
}