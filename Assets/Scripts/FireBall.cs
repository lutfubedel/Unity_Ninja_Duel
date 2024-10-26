using System.Collections;
using UnityEngine;
using Photon.Pun;

public class FireBall : MonoBehaviour
{
    public float fireBallSpeed;
    public int moveInput;

    [SerializeField] private float damageSize;

    PhotonView pw;
    GameManager manager;

    private void Start()
    {
        pw = GetComponent<PhotonView>();
        manager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        pw.RPC("DestroyWithTime", RpcTarget.All, pw.ViewID, 5f);
    }

    private void Update()
    {
        transform.position += new Vector3(moveInput * fireBallSpeed * Time.deltaTime, 0, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.CompareTag("Player1Fire") && collision.gameObject.CompareTag("Player2"))
        {
            GameObject expo = PhotonNetwork.Instantiate("Explosion", new Vector3(collision.transform.position.x, collision.transform.position.y, collision.transform.position.z - 0.1f), Quaternion.identity, 0, null);
            PhotonView expoPhotonView = expo.GetComponent<PhotonView>();

            manager.GetComponent<PhotonView>().RPC("PlayerDamage", RpcTarget.All, 2, damageSize);

            pw.RPC("DestroyWithTime", RpcTarget.All, expoPhotonView.ViewID,0.25f);
        }

        if(gameObject.CompareTag("Player2Fire") && collision.gameObject.CompareTag("Player1"))
        {
            GameObject expo = PhotonNetwork.Instantiate("Explosion", new Vector3(collision.transform.position.x, collision.transform.position.y, collision.transform.position.z - 0.1f), Quaternion.identity, 0, null);
            PhotonView expoPhotonView = expo.GetComponent<PhotonView>();

            manager.GetComponent<PhotonView>().RPC("PlayerDamage", RpcTarget.All, 1, damageSize);

            pw.RPC("DestroyWithTime", RpcTarget.All, expoPhotonView.ViewID, 0.25f);
        }


    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (gameObject.CompareTag("Player1Fire") && collision.gameObject.CompareTag("Player2") || gameObject.CompareTag("Player2Fire") && collision.gameObject.CompareTag("Player1"))
        {
            fireBallSpeed = 0;
            pw.RPC("DestroyWithTime", RpcTarget.All, pw.ViewID, 0.25f);
        }
    }


    [PunRPC]
    public void DestroyWithTime(int viewID, float delay)
    {
        StartCoroutine(DestroyAfterTime(viewID, delay));
    }

    private IEnumerator DestroyAfterTime(int viewID, float delay)
    {
        yield return new WaitForSeconds(delay);

        PhotonView objPhotonView = PhotonView.Find(viewID);
        if (objPhotonView != null && pw.IsMine)
        {
            PhotonNetwork.Destroy(objPhotonView.gameObject);
        }
    }

}
