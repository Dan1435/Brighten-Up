using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject menuCanvas; // Drag your Start Screen here
    public CharchterManager charManager; // Drag the Manager with the timer here

    void Start()
    {
        // Stop time so nothing moves while at the menu
        Time.timeScale = 0;
        menuCanvas.SetActive(true);
    }

    public void StartTheGame()
    {
        // Hide the menu and unfreeze time
        menuCanvas.SetActive(false);
        Time.timeScale = 1;

        // Tell the other script to start the logic
        charManager.isGameOver = false;
        charManager.NextRound();
    }
}