using UnityEngine;
using System.Collections.Generic;

public class MainMenuButtons : MonoBehaviour
{
    public MainServer mainServer;
    public GameObject lobbyListUI;
    public Dictionary<string, GameObject> UIPanels;
    public string currentOpenPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        UIPanels = new Dictionary<string, GameObject>
        {
            { "LobbyList", lobbyListUI }
        };
    }
    public void ListLobbiesButton()
    {
        UIPanels["LobbyList"].SetActive(true);
        mainServer.ListLobbies();
        currentOpenPanel = "LobbyList";
    }
    public void OnServerMessageReceived(string message)
    {
        Debug.Log("Server Message Received: " + message);

    }
    public void CloseCurrentPanel()
    {
        if (currentOpenPanel != null && UIPanels.ContainsKey(currentOpenPanel))
        {
            UIPanels[currentOpenPanel].SetActive(false);
            currentOpenPanel = null;
        }
    }
    public void Update()
    {
        // Example: Close panel on Escape key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseCurrentPanel();
        }
    }
}
