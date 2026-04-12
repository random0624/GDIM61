using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 180f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        float moveInput = Input.GetAxisRaw("Vertical");     // W/S
        float turnInput = Input.GetAxisRaw("Horizontal");   // A/D

        // 前后移动
        transform.position += transform.forward * moveInput * moveSpeed * Time.deltaTime;

        // 左右转向
        transform.Rotate(0f, turnInput * turnSpeed * Time.deltaTime, 0f);
    }
}
