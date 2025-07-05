using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Настройки Карабканья")]
    public float climbOverOffset = 0.2f;
    public float climbOffsetForward = 0.2f;
    public LayerMask climbableLayer;
    public float minClimbableHeight = 0.5f;
    public float maxClimbableHeight = 2.0f;
    public float raycastDistance = 1.0f;

    [Header("Настройки Прыжка")]
    public LayerMask groundLayer;
    public Transform groundCheckerTransform;
    public float jumpForce = 10f;
    public float gravityMultiplier = 2f;
    private float groundCheckRadius = 0.3f;

    [Header("Настройки Передвижения")]
    public float speed = 5.0f;
    public float rotationSpeed = 10f;

    [Header("Настройки Анимации")]
    public Animator playerAnimator;
    public string climbTriggerName = "Climb";
    public float animationMatchMoveDuration = 0.8f;
    public string jumpTriggerName = "Jump";
    public string isGroundedBoolName = "isGrounded";
    public string animSpeedParamName = "speed";
    public string animVerticalVelParamName = "verticalVelocity";

    [Header("Настройки Аудио")]
    public AudioSource playerAudioSource;
    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioClip[] footstepSounds;
    public float footstepDelay = 0.4f;
    private float footstepTimer;
    private bool playedLandSound = false;

    // Приватные переменные состояния
    private Rigidbody rb;
    private bool isGrounded;
    private bool isClimbing = false;
    private Vector3 initialClimbPosition;
    private bool inputEnabled = true;

    /// <summary>
    /// Вызывается при первом кадре игры.
    /// </summary>
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

        if (playerAudioSource == null)
        {
            playerAudioSource = GetComponent<AudioSource>();
            if (playerAudioSource == null)
            {
                Debug.LogError("AudioSource не найден на объекте игрока! Добавьте его.", this);
            }
        }

        footstepTimer = footstepDelay;
    }

    /// <summary>
    /// Вызывается каждый фиксированный кадр физики.
    /// </summary>
    void FixedUpdate()
    {
        if (isClimbing)
        {
            return;
        }

        if (rb.linearVelocity.y < 0 && !isGrounded)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (gravityMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// Вызывается каждый кадр.
    /// </summary>
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheckerTransform.position, groundCheckRadius, groundLayer);
        if (playerAnimator != null)
        {
            playerAnimator.SetBool(isGroundedBoolName, isGrounded);
        }

        if (isGrounded && !playedLandSound && rb.linearVelocity.y < -0.1f)
        {
            PlaySound(landSound);
            playedLandSound = true;
        }
        else if (!isGrounded && playedLandSound)
        {
            playedLandSound = false;
        }

        if (!inputEnabled)
        {
            return;
        }

        if (isClimbing)
        {
            UpdateAnimation();
            return;
        }

        Move();
        UpdateAnimation();

        if (Input.GetButtonDown("Jump"))
        {
            RaycastHit hit;
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            Vector3 direction = transform.forward;

            if (Physics.Raycast(origin, direction, out hit, raycastDistance, climbableLayer))
            {
                if (hit.collider.CompareTag("Climbable"))
                {
                    float obstacleHeight = hit.point.y - transform.position.y;

                    if (obstacleHeight >= minClimbableHeight && obstacleHeight <= maxClimbableHeight)
                    {
                        float playerHeadHeight = GetComponent<Collider>().bounds.size.y;
                        float headClearanceCheckY = hit.point.y + climbOverOffset + playerHeadHeight - 0.1f;

                        if (!Physics.CheckSphere(new Vector3(transform.position.x, headClearanceCheckY, transform.position.z), 0.3f, climbableLayer))
                        {
                            StartCoroutine(Climb(hit.point.y));
                            return;
                        }
                        else
                        {
                            Debug.Log("Нет места над препятствием для карабканья (голова упрется).");
                        }
                    }
                    else
                    {
                        Debug.Log("Препятствие слишком низкое или слишком высокое для карабканья: " + obstacleHeight.ToString("F2") + "м.");
                    }
                }
            }

            if (isGrounded)
            {
                Jump();
            }
        }
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (isGrounded && horizontalVelocity.magnitude > 0.1f && !isClimbing)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0)
            {
                PlayFootstepSound();
                footstepTimer = footstepDelay;
            }
        }
        else
        {
            footstepTimer = footstepDelay;
        }
    }

    /// <summary>
    /// Обрабатывает передвижение персонажа на основе пользовательского ввода.
    /// </summary>
    void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        // float vertical = Input.GetAxis("Vertical");

        Vector3 directionVector = new Vector3(0, 0, horizontal);

        if (directionVector.magnitude > 1f)
            directionVector.Normalize();

        if (directionVector.magnitude > 0.05f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionVector);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        rb.angularVelocity = Vector3.zero;

        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, directionVector.z * speed);

        playerAnimator.SetFloat(animSpeedParamName, directionVector.magnitude);
    }

    /// <summary>
    /// Применяет вертикальную силу для выполнения прыжка.
    /// </summary>
    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger(jumpTriggerName);
        }
        PlaySound(jumpSound);
    }

    /// <summary>
    /// Обновляет параметры Animator для управления анимациями.
    /// </summary>
    void UpdateAnimation()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetFloat(animVerticalVelParamName, rb.linearVelocity.y);
        }
    }

    /// <summary>
    /// Корутина, управляющая процессом карабканья персонажа.
    /// </summary>
    private IEnumerator Climb(float targetClimbY)
    {
        isClimbing = true;
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        initialClimbPosition = transform.position;

        Vector3 targetPosition = new Vector3(
            transform.position.x + transform.forward.x * climbOffsetForward,
            targetClimbY + climbOverOffset,
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

    /// <summary>
    /// Визуализирует сферу проверки земли в редакторе Unity для отладки.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (groundCheckerTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheckerTransform.position, groundCheckRadius);
        }
    }

    /// <summary>
    /// Вспомогательный метод для воспроизведения звука.
    /// </summary>
    void PlaySound(AudioClip clip)
    {
        if (playerAudioSource != null && clip != null)
        {
            playerAudioSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// Метод для воспроизведения случайного звука шага из массива.
    /// </summary>
    void PlayFootstepSound()
    {
        if (playerAudioSource != null && footstepSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, footstepSounds.Length);
            PlaySound(footstepSounds[randomIndex]);
        }
    }

    /// <summary>
    /// Публичный метод для внешнего включения/отключения ввода игрока.
    /// </summary>
    public void SetInputEnabled(bool enable)
    {
        inputEnabled = enable;
        if (!enable)
        {
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            if (playerAnimator != null)
            {
                playerAnimator.SetFloat(animSpeedParamName, 0);
            }
        }
    }
}