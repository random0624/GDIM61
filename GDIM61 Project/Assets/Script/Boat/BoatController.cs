using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 3f;
    [SerializeField] private float acceleration = 0.5f;
    [SerializeField] private float deceleration = 0.5f;
    [SerializeField] private float turnSpeed = 60f;
    private float currentSpeed = 0f;
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
        if (moveInput > 0)
        {
            currentSpeed += acceleration * Time.deltaTime;
            BoatFuel.Instance.ConsumeFuel(acceleration * Time.deltaTime);
        }
        else if (moveInput < 0)
        {
            currentSpeed -= deceleration * Time.deltaTime;
        }
        else
        {
            if (currentSpeed > 0)
            {
                currentSpeed -= deceleration * Time.deltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0);
            }
            else if (currentSpeed < 0)
            {
                currentSpeed += deceleration * Time.deltaTime;
                currentSpeed = Mathf.Min(currentSpeed, 0);
            }

        }
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        transform.position += transform.forward * currentSpeed * Time.deltaTime;

        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            transform.Rotate(0f, turnInput * turnSpeed * Time.deltaTime, 0f);
        }
    }
}
