using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_3 : MonoBehaviour
{
    public float moveSpeed;
    public float rotationSpeed;

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
        transform.Rotate(new Vector3(0, 0, -1 * rotationSpeed * Time.deltaTime));

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
}
