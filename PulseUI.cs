using UnityEngine;

public class PulseUI : MonoBehaviour
{
    public float pulseMagnitude = 0.1f; // How much it grows (0.1 = 10%)
    public float pulseSpeed = 3f;      // How fast it pulses

    private Vector3 startScale;

    void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        // Use UnscaledTime so it works while the game is paused!
        float scaleFactor = 1f + Mathf.Sin(Time.unscaledTime * pulseSpeed) * pulseMagnitude;
        transform.localScale = startScale * scaleFactor;
    }
}