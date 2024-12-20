using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Cinemachine;


public class Player : MonoBehaviour
{
    public Image healthBar;
    public bool canHit;
    public bool canTakeDamage;
    private bool hasCollectedCrystal = false;
    public GameObject enemy;

    Rigidbody2D rb;
    PhotonView pw;
    CinemachineVirtualCamera virtualCamera;
    GameManager manager;


    private void Start()
    {
        Application.targetFrameRate = 60;

        rb = GetComponent<Rigidbody2D>();
        pw = GetComponent<PhotonView>();
        manager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        if (pw.IsMine)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("PlayerTag1 Added");
                pw.RPC("SetPlayerTag", RpcTarget.AllBuffered, "Player1", "SpawnPoint_1" , "Player1_HealthBar");
            }
            else
            {
                Debug.Log("PlayerTag2 Added");
                pw.RPC("SetPlayerTag", RpcTarget.AllBuffered, "Player2", "SpawnPoint_2" , "Player2_HealthBar");
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

    private void Update()
    {
        if(pw.IsMine)
        {
            if (!canTakeDamage)
            {
                Invoke(nameof(ChangeCanTakeDamage), 2f);
            }
        }

    }

    [PunRPC]
    public void SetPlayerTag(string tag, string spawnPointTag, string healthbarTag)
    {
        gameObject.tag = tag;
        transform.position = GameObject.FindWithTag(spawnPointTag).transform.position;
        transform.rotation = GameObject.FindWithTag(spawnPointTag).transform.rotation;

        transform.GetChild(3).transform.GetChild(1).tag = healthbarTag;
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        enemy = gameObject.CompareTag("Player1") ? GameObject.FindWithTag("Player2") : GameObject.FindWithTag("Player1");

        if (collision.CompareTag("HitBlock"))
        {
            enemy.GetComponent<Player>().canHit = true;
        }

        if (collision.CompareTag("Trap") && canTakeDamage)
        {
            GetComponent<PlayerController>().isHitbyTrap = true;
            Vector2 forceDirection = (collision.transform.position - transform.position).normalized;

            rb.AddForce(forceDirection * 500, ForceMode2D.Force);
            TrapDamage();
            canTakeDamage = false;
        }

        if (collision.CompareTag("HealthPotion"))
        {
            if (gameObject.CompareTag("Player1"))
            {
                manager.GetComponent<PhotonView>().RPC("HealthPlus", RpcTarget.All, 1);
            }
            else if (gameObject.CompareTag("Player2"))
            {
                manager.GetComponent<PhotonView>().RPC("HealthPlus", RpcTarget.All, 2);
            }

            PhotonView objPhotonView = collision.GetComponent<PhotonView>();

            if (PhotonNetwork.IsMasterClient)
            {
                if (objPhotonView != null)
                {
                    PhotonNetwork.Destroy(objPhotonView.gameObject);
                }
            }

        }

        if (collision.CompareTag("Crystal") && !hasCollectedCrystal && pw.IsMine)
        {
            PhotonView crystalPhotonView = collision.GetComponent<PhotonView>();

            if (PhotonNetwork.IsMasterClient && crystalPhotonView != null)
            {
                if (gameObject.CompareTag("Player1"))
                {
                    manager.GetComponent<PhotonView>().RPC("TakeCrystal", RpcTarget.All, 1);
                }
                else if (gameObject.CompareTag("Player2"))
                {
                    manager.GetComponent<PhotonView>().RPC("TakeCrystal", RpcTarget.All, 2);
                }

                PhotonNetwork.Destroy(crystalPhotonView.gameObject);
            }

            hasCollectedCrystal = true;
            Invoke(nameof(ResetCrystalCollectionFlag), 0.5f);
        }


    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        enemy = gameObject.CompareTag("Player1") ? GameObject.FindWithTag("Player2") : GameObject.FindWithTag("Player1");

        if (collision.CompareTag("HitBlock"))
        {
            enemy.GetComponent<Player>().canHit = false;
        }
    }

    public void HitPlayer()
    {
        if (canHit && pw.IsMine)
        {
            if(gameObject.CompareTag("Player1"))
            {
                manager.GetComponent<PhotonView>().RPC("PlayerDamage", RpcTarget.All, 2, 10f);
            }
            else
            {
                manager.GetComponent<PhotonView>().RPC("PlayerDamage", RpcTarget.All, 1, 10f);
            }
        }
    }

    public void TrapDamage()
    {
        if (pw.IsMine && canTakeDamage)
        {
            if (gameObject.CompareTag("Player1"))
            {
                manager.GetComponent<PhotonView>().RPC("PlayerDamage", RpcTarget.All, 1, 10f);
            }
            else
            {
                manager.GetComponent<PhotonView>().RPC("PlayerDamage", RpcTarget.All, 2, 10f);
            }
        }
    }


    private void ResetCrystalCollectionFlag()
    {
        hasCollectedCrystal = false;
    }


    private void ChangeCanTakeDamage()
    {
        canTakeDamage = true;
    }






}
