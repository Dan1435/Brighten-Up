using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject menuCanvas; 
    public CharchterManager charManager; 

    void Start()
    {
   
        Time.timeScale = 0;
        menuCanvas.SetActive(true);
    }

    public void StartTheGame()
    {
    
        menuCanvas.SetActive(false);
        Time.timeScale = 1;

    
        charManager.isGameOver = false;
        charManager.NextRound();
    }
}
