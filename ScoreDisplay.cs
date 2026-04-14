using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    private TextMeshProUGUI scoreText;

    void Awake()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
    }


    public void UpdateScoreUI(int currentScore)
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();

         
            scoreText.color = Color.yellow;

          
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
