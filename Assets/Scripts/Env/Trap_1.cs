using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_1 : MonoBehaviour
{
    public float rotationSpeed;
    public float moveSpeed;

    private Transform wallRight;
    private Transform wallLeft;
    private void Start()
    {
        wallRight = GameObject.FindWithTag("WallRight").transform;
        wallLeft = GameObject.FindWithTag("WallLeft").transform;
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, -1 * rotationSpeed * Time.deltaTime));
        transform.position += new Vector3(1 * moveSpeed * Time.deltaTime, 0, 0);

        if (transform.position.x - 2 <= wallLeft.transform.position.x || transform.position.x + 2 >= wallRight.transform.position.x)
        {
            moveSpeed *= -1;
        }
    }
}
