using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public TMP_Text text_PlayerName;
    public GameObject playerNameScene;
    public GameObject mainMenuScene;
    public TMP_InputField input_PlayerName;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("UserName"))
        {
            playerNameScene.SetActive(true);
            mainMenuScene.SetActive(false);

        }
        else
        {
            playerNameScene.SetActive(false);
            mainMenuScene.SetActive(true);
            text_PlayerName.text = "Username : " + PlayerPrefs.GetString("UserName");
        }
    }

    public void SavePlayerName()
    {
        PlayerPrefs.SetString("UserName", input_PlayerName.text);
        playerNameScene.SetActive(false);
        mainMenuScene.SetActive(true);
        text_PlayerName.text = "Username : " + PlayerPrefs.GetString("UserName");

        GameObject.FindWithTag("Button_JoinRoom").GetComponent<Button>().interactable = true;
        GameObject.FindWithTag("Button_CreateRoom").GetComponent<Button>().interactable = true;
    }
}
