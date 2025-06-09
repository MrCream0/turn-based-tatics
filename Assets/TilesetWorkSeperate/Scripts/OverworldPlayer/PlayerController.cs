using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float rollVelocity = 1.5f;
    public float rollDuration = 0.35f;
    public float jumpVelocity = 7f;
    public float rollCooldown = 1f;

    private PlayerMotor motor;
    private Transform cam;
    private Vector2 input;
    private PlayerAnimationController animationController;

    private bool canRoll = true;
    private bool isRolling = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        motor = GetComponent<PlayerMotor>();
        animationController = GetComponent<PlayerAnimationController>();
        cam = Camera.main.transform;
    }

    void Update()
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        Vector3 moveDir = cam.forward * input.y + cam.right * input.x;
        moveDir.y = 0;
        moveDir.Normalize();

        motor.Move(moveDir, moveSpeed);

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canRoll && moveDir != Vector3.zero)
        {
            StartCoroutine(Roll(moveDir));
        }
    }

    private IEnumerator Roll(Vector3 direction)
    {
        isRolling = true;
        canRoll = false;
        animationController.TriggerRoll();

        float elapsed = 0f;
        while (elapsed < rollDuration)
        {
            motor.Move(direction, rollVelocity);
            elapsed += Time.deltaTime;
            yield return null;
        }

        animationController.ResetRoll();
        isRolling = false;

        yield return new WaitForSeconds(rollCooldown);

        canRoll = true;
    }

    public float GetSpeed()
    {
        return moveSpeed;
    }

    public void Jump()
    {
        if (motor.IsGrounded())
        {
            animationController.TriggerJump();
            motor.Jump(jumpVelocity);
        }
    }
}
