using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Cinemachine;


public class Player : MonoBehaviour
{
    public float health;
    public Image healthBar;
    public bool canHit;
    public bool canTakeDamage;
    public GameObject enemy;

    Rigidbody2D rb;
    PhotonView pw;
    CinemachineVirtualCamera virtualCamera;


    private void Start()
    {
        Application.targetFrameRate = 60;
        health = 100;
        enemy = gameObject.CompareTag("Player1") ? GameObject.FindWithTag("Player2") : GameObject.FindWithTag("Player1");

        rb = GetComponent<Rigidbody2D>();
        pw = GetComponent<PhotonView>();

        if (pw.IsMine)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("PlayerTag1 Added");
                pw.RPC("SetPlayerTag", RpcTarget.AllBuffered, "Player1", "SpawnPoint_1");
            }
            else
            {
                Debug.Log("PlayerTag2 Added");
                pw.RPC("SetPlayerTag", RpcTarget.AllBuffered, "Player2", "SpawnPoint_2");
            }
        }

        canTakeDamage = true;

        if (pw.IsMine)
        {
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            virtualCamera.Follow = this.transform;
            virtualCamera.LookAt = this.transform;
        }
    }

    [PunRPC]
    public void SetPlayerTag(string tag, string spawnPointTag)
    {
        gameObject.tag = tag;
        transform.position = GameObject.FindWithTag(spawnPointTag).transform.position;
        transform.rotation = GameObject.FindWithTag(spawnPointTag).transform.rotation;
    }


    [PunRPC]
    public void HitPlayer()
    {
        if (canHit)
        {
            health -= 10; // Decrease health locally on each client
        }
    }

    // Call this method from your animation event
    public void TriggerHitPlayer()
    {
        // This will call HitPlayer on all clients
        pw.RPC("HitPlayer", RpcTarget.All);
    }

}
