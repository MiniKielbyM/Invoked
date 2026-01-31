using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LobbyMenuButtons : MonoBehaviour
{
    public MainServer mainServer;
    public TMP_InputField lobbyNameInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void CreateLobbyButton()
    {
        if(lobbyNameInput.text == "")
        {
            Debug.LogWarning("Lobby name cannot be empty.");
            return;
        }
        Debug.Log("Create Lobby Button Pressed");
        mainServer.CreateLobby(lobbyNameInput.text);
    }

}
