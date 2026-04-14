using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    private TextMeshProUGUI scoreText;

    void Awake()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
    }

    // Combined into one single function
    public void UpdateScoreUI(int currentScore)
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();

            // Visual feedback: turn yellow
            scoreText.color = Color.yellow;

            // Return to white after 0.1 seconds
            Invoke("ResetColor", 0.1f);
        }
    }

    void ResetColor()
    {
        if (scoreText != null)
        {
            scoreText.color = Color.white;
        }
    }
}