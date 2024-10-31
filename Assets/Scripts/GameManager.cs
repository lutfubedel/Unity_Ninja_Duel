using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Player Health UI")]
    [SerializeField] private Image player1_HealthBar;
    [SerializeField] private Image player2_HealthBar;

    [Header("Player Health")]
    [SerializeField] private float player1_health;
    [SerializeField] private float player2_health;

    [Header("Prize Items")]
    [SerializeField] private bool isSpawnerStarted;
    [SerializeField] private int limit;
    [SerializeField] private int ciycleCount;
    [SerializeField] private float waitingTime;

    [Header("Prize Items")]
    [SerializeField] private bool isSpawnerStarted2;
    [SerializeField] private float waitingTime2;

    [Header("Player Crystal Image")]
    [SerializeField] private GameObject player1_crystalParent;
    [SerializeField] private GameObject player2_crystalParent;

    [Header("Player Crystal Count")]
    [SerializeField] private int player1_crystal;
    [SerializeField] private int player2_crystal;

    [Header("Crystal Items")]
    [SerializeField] private bool isSpawnerStarted3;
    [SerializeField] private float waitingTime3;


    PhotonView pw;

    private void Awake()
    {
        isSpawnerStarted = false;
    }

    void Start()
    {
        player1_health = 100;
        player2_health = 100;

        limit = 10;
        waitingTime = 20f;
        waitingTime2 = 5f;
        waitingTime3 = 30f;

        player1_crystal = 0;
        player2_crystal = 0;

        pw = GetComponent<PhotonView>();

    }



    [PunRPC]
    public void PlayerDamage(int choice, float damageSize)
    {
        if (choice == 1)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                player1_health -= damageSize;
                pw.RPC("UpdateHealthBar", RpcTarget.All, 1, player1_health);

                if(player1_health <= 0)
                {
                    player1_health = 0;
                    pw.RPC("PlayerDead", RpcTarget.All,"Player1");
                    pw.RPC("ShowEndGamePanel", RpcTarget.All, GameObject.FindWithTag("Player2").GetComponent<PhotonView>().Owner.NickName);
                }
            }
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                player2_health -= damageSize;
                pw.RPC("UpdateHealthBar", RpcTarget.All, 2, player2_health);

                if (player2_health <= 0)
                {
                    player2_health = 0;
                    pw.RPC("PlayerDead", RpcTarget.All, "Player2");
                    pw.RPC("ShowEndGamePanel", RpcTarget.All, GameObject.FindWithTag("Player1").GetComponent<PhotonView>().Owner.NickName);
                }
            }
        }
    }

    [PunRPC]
    public void UpdateHealthBar(int playerNo, float currentHealth)
    {

        player1_HealthBar = GameObject.FindWithTag("Player1_HealthBar").GetComponent<Image>();
        player2_HealthBar = GameObject.FindWithTag("Player2_HealthBar").GetComponent<Image>();

        if (playerNo == 1)
        {
            player1_health = currentHealth;
            player1_HealthBar.fillAmount = player1_health / 100;
        }
        else
        {
            player2_health = currentHealth;
            player2_HealthBar.fillAmount = player2_health / 100;
        }
    }

    [PunRPC]
    public void HealthPlus(int playerNo)
    {
        player1_HealthBar = GameObject.FindWithTag("Player1_HealthBar").GetComponent<Image>();
        player2_HealthBar = GameObject.FindWithTag("Player2_HealthBar").GetComponent<Image>();

        if (playerNo == 1)
        {
            player1_health += 30;

            if (player1_health >= 100)
            {
                player1_health = 100;
                player1_HealthBar.fillAmount = player1_health / 100;
            }
            else
            {
                player1_HealthBar.fillAmount = player1_health / 100;
            }
        }
        else
        {
            player2_health += 30;
            if (player2_health >= 100)
            {
                player2_health = 100;
                player2_HealthBar.fillAmount = player2_health / 100;
            }
            else
            {
                player2_HealthBar.fillAmount = player2_health / 100;
            }
        }
    }

    [PunRPC]
    public void HealthPotionSpawner()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            isSpawnerStarted = true;
            StartCoroutine(StartHealthPotionSpawner());
        }
    }

    [PunRPC]
    public void SpikeSpawner()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            isSpawnerStarted2 = true;
            StartCoroutine(StartSpikeSpawner());
        }
    }

    [PunRPC]
    public void CrystalSpawner()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            isSpawnerStarted3 = true;
            StartCoroutine(StartCrystalSpawner());
        }
    }

    [PunRPC]
    public void TakeCrystal(int playerNo)
    {
        player1_crystalParent = GameObject.FindWithTag("Player1").transform.GetChild(3).GetChild(2).gameObject;
        player2_crystalParent = GameObject.FindWithTag("Player2").transform.GetChild(3).GetChild(2).gameObject;

        if(playerNo == 1)
        {
            player1_crystal++;
            for(int i=0; i < player1_crystal; i++)
            {
                player1_crystalParent.transform.GetChild(i).GetComponent<Image>().enabled = true;
            }

            if(player1_crystal == 3)
            {
                pw.RPC("PlayerDead", RpcTarget.All, "Player2");
                pw.RPC("ShowEndGamePanel", RpcTarget.All, GameObject.FindWithTag("Player1").GetComponent<PhotonView>().Owner.NickName);
            }
        }
        else
        {
            player2_crystal++;
            for (int i = 0; i < player2_crystal; i++)
            {
                player2_crystalParent.transform.GetChild(i).GetComponent<Image>().enabled = true;
            }

            if (player2_crystal == 3)
            {
                pw.RPC("PlayerDead", RpcTarget.All, "Player1");
                pw.RPC("ShowEndGamePanel", RpcTarget.All, GameObject.FindWithTag("Player2").GetComponent<PhotonView>().Owner.NickName);
            }
        }

    }


    [PunRPC]
    public void PlayerDead(string playerTag)
    {
        GameObject.FindWithTag(playerTag).GetComponent<Animator>().SetBool("Dead", true);

        GameObject.FindWithTag(playerTag).GetComponent<SpriteRenderer>().enabled = false;
        GameObject.FindWithTag(playerTag).transform.GetChild(3).GetComponent<Canvas>().enabled = false;

        GameObject.FindWithTag("Player1").GetComponent<PlayerController>().canFire = false;
        GameObject.FindWithTag("Player1").GetComponent<PlayerController>().canMove = false;

        GameObject.FindWithTag("Player2").GetComponent<PlayerController>().canFire = false;
        GameObject.FindWithTag("Player2").GetComponent<PlayerController>().canMove = false;
    }


    [PunRPC]
    public void ShowEndGamePanel(string winnerName)
    {
        foreach (GameObject item in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (item.CompareTag("Panel_End"))
            {
                item.SetActive(true);
                GameObject.FindWithTag("Panel_End_PlayerName").GetComponent<TextMeshProUGUI>().text = winnerName + " Wins!";
            }
        }
    }


    public void MainMenu()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();

        PhotonNetwork.LoadLevel(0);
    }



    IEnumerator StartHealthPotionSpawner()
    {
        ciycleCount = 0;
        while (isSpawnerStarted)
        {
            if (limit == ciycleCount)
            {
                isSpawnerStarted = false;
            }

            yield return new WaitForSeconds(waitingTime);
            Vector3 spawnPoint = new Vector3(Random.Range(-35f,35f),Random.Range(2f,26f),0);
            PhotonNetwork.Instantiate("HealthPotion", spawnPoint, Quaternion.identity, 0, null);
            ciycleCount++;
        }
    }
    IEnumerator StartCrystalSpawner()
    {
        ciycleCount = 0;
        while (isSpawnerStarted3)
        {
            if (GameObject.FindWithTag("Crystal") == null)
            {
                if (limit == ciycleCount)
                {
                    isSpawnerStarted3 = false;
                }

                Vector3 spawnPoint = new Vector3(Random.Range(-35f, 35f), Random.Range(2f, 26f), 0);
                PhotonNetwork.Instantiate("Crystal", spawnPoint, Quaternion.identity, 0, null);
                ciycleCount++;
            }

            yield return new WaitForSeconds(waitingTime3);
        }
    }
    IEnumerator StartSpikeSpawner()
    {
        while (isSpawnerStarted2)
        {
            yield return new WaitForSeconds(waitingTime2);
            Vector3 spawnPoint = new Vector3(Random.Range(-32f, 32f), 30f, 0);
            PhotonNetwork.Instantiate("Spike", spawnPoint, Quaternion.Euler(0,0,180), 0, null);
        }
    }

}
