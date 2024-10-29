using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    [Header("Player Health")]
    [SerializeField] private Image player1_HealthBar;
    [SerializeField] private Image player2_HealthBar;

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
            }
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                player2_health -= damageSize;
                pw.RPC("UpdateHealthBar", RpcTarget.All, 2, player2_health);
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
