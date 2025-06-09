using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    public Rigidbody rb;
    public LayerMask groundMask;
    public float groundCheckDistance = 0.1f;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    public void Move(Vector3 direction, float speed)
    {
        Vector3 velocity = direction * speed;
        velocity.y = rb.velocity.y; // Preserve vertical velocity
        rb.velocity = velocity;
    }

    public void Jump(float jumpVelocity)
    {
        if (IsGrounded())
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpVelocity, ForceMode.Impulse);
        }
    }

    public void Roll(Vector3 direction, float force)
    {
        rb.AddForce(direction.normalized * force, ForceMode.VelocityChange);
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.1f, groundMask);
    }
}
