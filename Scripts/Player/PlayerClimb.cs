using UnityEngine;
using System.Collections;

public class PlayerClimber : MonoBehaviour
{
    public float climbHeight = 1.2f;
    public float climbOffsetForward = 0.2f;
    public LayerMask climbableLayer;

    [Header("Настройки Прыжка")]
    public LayerMask groundLayer;
    public Transform groundCheckerTransform;
    public float jumpForce = 10f;
    public float gravityMultiplier = 2f;

    [Header("Настройки Анимации")]
    public Animator playerAnimator;
    public string climbTriggerName = "Climb";
    public float animationMatchMoveDuration = 0.8f;
    public string jumpTriggerName = "Jump";
    public string isGroundedBoolName = "isGrounded";

    private float groundCheckRadius = 0.3f;
    private bool isGrounded;
    private Rigidbody rb;
    private bool isClimbing = false;
    private Vector3 initialClimbPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody не найден на объекте игрока!", this);
        }

        playerAnimator = GetComponent<Animator>();
        if (playerAnimator == null)
        {
            Debug.LogError("Animator не найден на объекте игрока!", this);
        }
    }

    void FixedUpdate()
    {
        if (rb.linearVelocity.y < 0 && !isGrounded && !isClimbing)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (gravityMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheckerTransform.position, groundCheckRadius, groundLayer);
        if (playerAnimator != null)
        {
            playerAnimator.SetBool(isGroundedBoolName, isGrounded);
        }

        if (isClimbing) return;

        if (Input.GetButtonDown("Jump"))
        {
            RaycastHit hit;
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            Vector3 direction = transform.forward;

            if (Physics.Raycast(origin, direction, out hit, 1f, climbableLayer))
            {
                if (hit.collider.CompareTag("Climbable"))
                {
                    Vector3 checkOverObstacleOrigin = new Vector3(transform.position.x, hit.point.y + climbHeight + 0.1f, transform.position.z);
                    if (!Physics.CheckSphere(checkOverObstacleOrigin, 0.3f, ~climbableLayer))
                    {
                        StartCoroutine(Climb(hit.point));
                        return;
                    }
                    else
                    {
                        Debug.Log("Нет места над препятствием для карабканья.");
                    }
                }
            }

            if (isGrounded)
            {
                Jump();
            }
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger(jumpTriggerName);
            Debug.Log("Запускаем анимацию прыжка: " + jumpTriggerName);
        }
    }

    private IEnumerator Climb(Vector3 hitPoint)
    {
        Debug.Log("Climb Coroutine: START");
        isClimbing = true;
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        initialClimbPosition = transform.position;

        Vector3 targetPosition = new Vector3(
            transform.position.x + transform.forward.x * climbOffsetForward,
            hitPoint.y + climbHeight,
            transform.position.z + transform.forward.z * climbOffsetForward
        );

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger(climbTriggerName);
            Debug.Log("Climb Coroutine: Триггер анимации установлен: " + climbTriggerName);
        }

        yield return new WaitForEndOfFrame();

        float currentClipLength = 0f;
        if (playerAnimator != null)
        {
            AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Climb"))
            {
                currentClipLength = stateInfo.length;
                Debug.Log("Climb Coroutine: Длительность анимации Climb: " + currentClipLength);
            }
            else
            {
                Debug.LogWarning("Animator не в состоянии 'Climb' при запуске корутины. Проверьте переходы. Использую fallback длительность.");
                currentClipLength = animationMatchMoveDuration / 0.8f;
                if (currentClipLength <= 0) currentClipLength = 1.0f;
            }
        }
        else
        {
            currentClipLength = 1.0f;
        }

        if (currentClipLength <= 0.01f)
        {
            currentClipLength = 1.0f;
            Debug.LogWarning("Climb Coroutine: currentClipLength оказался слишком мал, установлен в 1.0f");
        }

        float moveDuration = currentClipLength * animationMatchMoveDuration;
        Debug.Log("Climb Coroutine: Длительность движения (moveDuration): " + moveDuration);

        float timer = 0f;
        while (timer < moveDuration)
        {
            transform.position = Vector3.Lerp(initialClimbPosition, targetPosition, timer / moveDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        Debug.Log("Climb Coroutine: Движение завершено. Позиция: " + transform.position);

        float remainingAnimTime = currentClipLength - moveDuration;
        if (remainingAnimTime > 0)
        {
            Debug.Log("Climb Coroutine: Ждем остаток анимации: " + remainingAnimTime + " сек.");
            yield return new WaitForSeconds(remainingAnimTime);
        }

        isClimbing = false;
        rb.isKinematic = false;
        Debug.Log("Climb Coroutine: isClimbing = false, rb.isKinematic = false. Физика включена.");

        if (playerAnimator != null)
        {
            playerAnimator.ResetTrigger(climbTriggerName);
        }
        Debug.Log("Climb Coroutine: END.");
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheckerTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheckerTransform.position, groundCheckRadius);
        }
    }
}