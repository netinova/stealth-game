using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 7;
    public float smoothMoveTime = .1f;
    public float turnSpeed = 8;

    float angle;
    float smoothInputMagnitude;
    float smoothMoveVelocity;

    void Update()
    {
        Vector3 inputDirection = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0,
            Input.GetAxisRaw("Vertical")
        ).normalized;

        float inputMagnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(
            smoothInputMagnitude,
            inputMagnitude,
            ref smoothMoveVelocity,
            smoothMoveTime
        );

        float targetAngel = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngel, Time.deltaTime * turnSpeed * inputMagnitude);
        transform.eulerAngles = Vector3.up * angle;

        transform.Translate(
            transform.forward * moveSpeed * Time.deltaTime * smoothInputMagnitude,
            Space.World
        );
    }
}
