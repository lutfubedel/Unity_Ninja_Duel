using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_1 : MonoBehaviour
{
    public float moveSpeed;

    public Vector3 firstPos;
    public Vector3 targetPos;

    public bool movingToTarget;

    private void Start()
    {
        firstPos = transform.position;
        targetPos = transform.GetChild(0).transform.position;

        movingToTarget = true;
    }

    private void Update()
    {
        if (movingToTarget)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            if (transform.position == targetPos)
            {
                movingToTarget = false; 
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, firstPos, moveSpeed * Time.deltaTime);
            if (transform.position == firstPos)
            {
                movingToTarget = true; 
            }
        }

    }


    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.StartsWith("Player"))
        {
            collision.transform.parent.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag.StartsWith("Player"))
        {
            collision.transform.parent.SetParent(null);
        }
    }
    
}
