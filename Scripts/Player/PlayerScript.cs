using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rigidbody;
    private Animator animator;

    public float speed = 2.0f;
    public float rotationSpeed = 10f;

    public Transform groundCheckerTransform;
    public LayerMask groundLayer;
    public float jumpForce = 10f;
    public float gravityMultiplier = 2f;
    private float groundCheckRadius = 0.3f;
    private bool isGrounded;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        UpdateAnimation();
    }

    void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 directionVector = new Vector3(0, 0, horizontal);
        if (directionVector.magnitude > 0.05f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(directionVector), Time.deltaTime * rotationSpeed);
        }

        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.linearVelocity = new Vector3(directionVector.x * speed, rigidbody.linearVelocity.y, directionVector.z * speed);

        animator.SetFloat("speed", directionVector.magnitude);
    }

    void Jump()
    {
        isGrounded = Physics.CheckSphere(groundCheckerTransform.position, groundCheckRadius, groundLayer);



        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rigidbody.linearVelocity = new Vector3(rigidbody.linearVelocity.x, jumpForce, rigidbody.linearVelocity.z);
            animator.SetTrigger("Jump");
        }
    }

    void UpdateAnimation()
    {
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("verticalVelocity", rigidbody.linearVelocity.y);
    }
}
