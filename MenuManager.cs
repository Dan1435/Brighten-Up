using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public List<Rooms> menuRooms;
    public float flickerSpeed = 0.5f; // How fast lights change
    private float flickerTimer;

    void Start()
    {
        // Find all rooms in the scene if not assigned
        if (menuRooms.Count == 0)
            menuRooms = new List<Rooms>(Object.FindObjectsByType<Rooms>(FindObjectsSortMode.None));

        // Ensure time is running for the animations/flickers
        Time.timeScale = 1;
    }

    void Update()
    {
        flickerTimer -= Time.deltaTime;
        if (flickerTimer <= 0)
        {
            ToggleRandomRoom();
            flickerTimer = flickerSpeed;
        }
    }

    void ToggleRandomRoom()
    {
        if (menuRooms.Count == 0) return;

        // Pick a random room and toggle it visually
        int randomIndex = Random.Range(0, menuRooms.Count);

        // We use a custom method so we don't trigger the CharchterManager game logic
        menuRooms[randomIndex].MenuToggle();
    }

    public void StartGame()
    {
        // If your game is in a different scene:
        // SceneManager.LoadScene("GameScene");

        // If your game is in the same scene, just enable your CharchterManager 
        // and disable the Menu UI/MenuManager.
    }
}