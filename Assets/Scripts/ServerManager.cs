using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class ServerManager : MonoBehaviourPunCallbacks
{

    private TMP_Text serverInfo;
    private Button button_saveName;
    private Button button_joinRoom;
    private Button button_createRoom;

    private void Start()
    {
        serverInfo = GameObject.FindWithTag("ServerInfo").GetComponent<TextMeshProUGUI>();

        if (!PlayerPrefs.HasKey("UserName"))
        {
            button_saveName = GameObject.FindWithTag("Button_SaveName").GetComponent<Button>();
        }
        else
        {
            button_joinRoom = GameObject.FindWithTag("Button_JoinRoom").GetComponent<Button>();
            button_createRoom = GameObject.FindWithTag("Button_CreateRoom").GetComponent<Button>();
        }

        PhotonNetwork.ConnectUsingSettings();
        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnConnectedToMaster()
    {
        serverInfo.text = "Connected to Server";
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        serverInfo.text = "Connected to Lobby";

        if (!PlayerPrefs.HasKey("UserName"))
        {
            button_saveName.interactable = true;
        }
        else
        {
            button_joinRoom.interactable = true;
            button_createRoom.interactable = true;
        }
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
        PhotonNetwork.LoadLevel(1);
    }

    public void CreateRoomAndJoin()
    {
        string roomName = "Room_" + Random.Range(0, 9999999);
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = 2, IsOpen = true, IsVisible = true }, TypedLobby.Default);
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Odaya Girildi");

        GameObject myPlayer = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0, null);
        myPlayer.GetComponent<PhotonView>().Owner.NickName = PlayerPrefs.GetString("UserName");

        if (PhotonNetwork.PlayerList.Length == 2)
        {
            GameObject.FindWithTag("GameManager").GetComponent<PhotonView>().RPC("HealthPotionSpawner", RpcTarget.All);
            GameObject.FindWithTag("GameManager").GetComponent<PhotonView>().RPC("SpikeSpawner", RpcTarget.All);

        }
    }



























    public override void OnLeftLobby()
    {
        base.OnLeftLobby();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
    }



    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        serverInfo.text = "Odaya Girilemedi";
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        serverInfo.text = "Random Bir Odaya Girilemedi";
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        serverInfo.text = "Oda Oluþturulamadý";
    }

}