using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public List<Rooms> menuRooms;
    public float flickerSpeed = 0.5f; 
    private float flickerTimer;

    void Start()
    {
       
        if (menuRooms.Count == 0)
            menuRooms = new List<Rooms>(Object.FindObjectsByType<Rooms>(FindObjectsSortMode.None));

     
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

   
        int randomIndex = Random.Range(0, menuRooms.Count);

       
        menuRooms[randomIndex].MenuToggle();
    }

    public void StartGame()
    {
   
    }
}
