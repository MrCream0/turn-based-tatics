using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private PlayerController movement; // For Explore phase input/logic
    private PlayerMotor motor; // For Explore phase Rigidbody movement
    //private PlayerTileMovement tileMovement; // For Trap phase tile-based movement

    [SerializeField] private float tileMoveSpeedEquivalent = 3f; // Speed value for tile movement animation (e.g., matches Walk)

    void Start()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerController>();
        motor = GetComponent<PlayerMotor>();
        //tileMovement = GetComponent<PlayerTileMovement>();
    }

    void Update()
    {
        float speed = 0f;
        if (motor != null && motor.rb != null)
        {
            speed = new Vector3(motor.rb.velocity.x, 0, motor.rb.velocity.z).magnitude;
        }
        /*// Determine speed based on game phase
        if (GameManager.Instance.currentPhase == GameManager.GamePhase.Trap)
        {
            // Tile-based movement
            if (tileMovement != null && tileMovement.isMoving)
            {
                // Use a constant speed for tile movement or calculate based on progress
                speed = tileMoveSpeedEquivalent;
                // Optional: Calculate pseudo-velocity for smoother transitions
                // speed = tileMovement.GetMovementSpeed();
            }
        }
        else if (GameManager.Instance.currentPhase == GameManager.GamePhase.Explore)
        {
            // Free movement
            if (motor != null && motor.rb != null)
            {
                speed = new Vector3(motor.rb.velocity.x, 0, motor.rb.velocity.z).magnitude;
            }
        }*/

        animator.SetFloat("Speed", speed);
        //Debug.Log($"Animation Speed: {speed}, Phase: {GameManager.Instance.currentPhase}");

        // Attack triggers (optional: restrict to Explore phase)
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("LightAttack");
        }
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("HeavyAttack");
        }
    }

    public void TriggerJump()
    {
        animator.SetBool("IsJumping", true);
    }

    public void TriggerRoll()
    {
        animator.SetBool("IsRolling", true);
    }

    public void ResetJump()
    {
        animator.SetBool("IsJumping", false);
    }

    public void ResetRoll()
    {
        animator.SetBool("IsRolling", false);
    }
}