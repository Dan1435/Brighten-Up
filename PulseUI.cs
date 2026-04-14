using UnityEngine;

public class PulseUI : MonoBehaviour
{
    public float pulseMagnitude = 0.1f; 
    public float pulseSpeed = 3f;     

    private Vector3 startScale;

    void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
    
        float scaleFactor = 1f + Mathf.Sin(Time.unscaledTime * pulseSpeed) * pulseMagnitude;
        transform.localScale = startScale * scaleFactor;
    }
}
