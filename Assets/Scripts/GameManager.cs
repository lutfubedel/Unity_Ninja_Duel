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

    PhotonView pw;    
    
    void Start()
    {
        pw = GetComponent<PhotonView>();

        player1_health = 100;
        player2_health = 100;
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



}
