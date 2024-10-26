using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private int extraJumps = 1;
    [SerializeField] private int jumpCount;

    [Header("Ground Checker")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Booleans")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool canFire;
    [SerializeField] private bool isSliding;
    [SerializeField] private bool isAirAttacking;
    [SerializeField] public bool isHitbyTrap;

    [Header("Fire")]
    [SerializeField] private GameObject fireBall;
    [SerializeField] private GameObject firePoint;

    Rigidbody2D rb;
    Animator anim;
    PhotonView pw;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        pw = GetComponent<PhotonView>();

        rb.freezeRotation = true;
        canFire = true;
        isAirAttacking = false;

        jumpCount = extraJumps;
    }

    void Update()
    {
        if (pw.IsMine)
        {
            MakeDizzy();

            if (!anim.GetBool("Dizzy"))
            {
                Move();
                Jump();
                Attack();
            }
        }

    }

    private void MakeDizzy()
    {
        if (isHitbyTrap)
        {
            anim.SetBool("Dizzy", true);
            Invoke(nameof(ChangeDizzy), 3f);
        }
    }

    private void Fire()
    {
        if (pw.IsMine)
        {
            GameObject newFireBall = PhotonNetwork.Instantiate("FireBall", firePoint.transform.position, Quaternion.identity, 0, null);

            newFireBall.tag = gameObject.CompareTag("Player1") ? "Player1Fire" : "Player2Fire";

            if (transform.eulerAngles.y == 180)
            {
                newFireBall.GetComponent<FireBall>().moveInput = -1;
                newFireBall.transform.rotation = new Quaternion(0, 180, 0, 0);
            }
            else
            {
                newFireBall.GetComponent<FireBall>().moveInput = 1;
            }
        }
    }

    private void Attack()
    {
        bool attack = Input.GetKeyDown(KeyCode.Mouse0);
        bool fireAttack = Input.GetKeyDown(KeyCode.Mouse1);

        if (!isSliding && attack)
        {
            if (isGrounded)
            {
                anim.SetBool("Attack", true);
            }
            else if (!isAirAttacking)
            {
                StartCoroutine(AirAttackRoutine());
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                anim.SetBool("Move", true);
            }

            anim.SetBool("Attack", false);
        }

        if (fireAttack && canFire)
        {
            anim.SetBool("FireAttack", true);
            Invoke(nameof(ChangeCanFire), 0.75f);
            canFire = false;
        }
        else
        {
            anim.SetBool("FireAttack", false);
        }
    }
    private void Move()
    {
        bool isMoving = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
        isSliding = Input.GetKeyDown(KeyCode.LeftControl);

        float moveInput = 0f;
        if (Input.GetKey(KeyCode.A))
        {
            moveInput = -1f;
            transform.rotation = new Quaternion(0, 180, 0, 0);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveInput = 1f;
            transform.rotation = new Quaternion(0, 0, 0, 0);
        }

        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        anim.SetBool("Move", isMoving);
        anim.SetBool("Dash", isSliding);
    }
    private void Jump()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        anim.SetBool("Jump", !isGrounded);

        if (isGrounded)
        {
            jumpCount = extraJumps;
        }

        if ((isGrounded || jumpCount > 0) && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            anim.SetBool("Jump", true);

            if (!isGrounded)
            {
                jumpCount--;
            }
        }
    }


    private void ChangeCanFire()
    {
        canFire = true;
    }

    private void ChangeDizzy()
    {
        isHitbyTrap = false;
        anim.SetBool("Dizzy", false);
    }



    private IEnumerator AirAttackRoutine()
    {
        isAirAttacking = true;
        anim.SetBool("AirAttack", true);

        yield return new WaitForSeconds(0.25f);

        anim.SetBool("AirAttack", false);
        isAirAttacking = false;
    }
}
