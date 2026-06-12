using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 7;
    public float smoothMoveTime = .1f;
    public float turnSpeed = 8;

    public event Action onReachedEndOfLevel;
    float angle;
    float smoothInputMagnitude;
    float smoothMoveVelocity;
    Vector3 velocity;

    bool disabled;

    Rigidbody rigidBody;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        Guard.onGuardHasSpottedPlayer += Disable;
    }

    void Update()
    {
        Vector3 inputDirection = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0,
            Input.GetAxisRaw("Vertical")
        ).normalized;

        if (disabled)
        {
            inputDirection = Vector3.zero;
        }

        float inputMagnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(
            smoothInputMagnitude,
            inputMagnitude,
            ref smoothMoveVelocity,
            smoothMoveTime
        );

        float targetAngel = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngel, Time.deltaTime * turnSpeed * inputMagnitude);
        velocity = transform.forward * moveSpeed * smoothInputMagnitude;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Finish")
        {
            Disable();
            if (onReachedEndOfLevel != null)
            {
                onReachedEndOfLevel();
            }
        }
    }

    void Disable()
    {
        disabled = true;
    }

    void FixedUpdate()
    {
        rigidBody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        rigidBody.MovePosition(rigidBody.position + velocity * Time.deltaTime);
    }

    void OnDestroy()
    {
        Guard.onGuardHasSpottedPlayer -= Disable;
    }
}
