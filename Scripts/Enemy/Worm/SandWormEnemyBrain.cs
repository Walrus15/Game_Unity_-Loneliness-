using UnityEngine;
using System.Collections;

public class SandWormEnemyBrain : MonoBehaviour
{
    public enum WormState
    {
        Passive,
        Attacking,
        Retreating
    }

    [Header("Настройки состояний")]
    public WormState currentState = WormState.Passive;

    [Header("Настройки игрока")]
    public Transform playerTransform;

    [Header("Настройки атаки")]
    public float attackSpeed = 3f;
    public float attackRange = 1.5f;
    public int attackDamage = 10;
    public float attackCooldown = 0.5f;
    private bool canAttack = true;

    [Header("Настройки появления/скрытия")]
    public float emergeHeight = 1f;
    public float emergeSpeed = 2f;

    [Header("Объекты и компоненты червя")]
    public GameObject wormModel;
    public Collider wormAttackCollider;
    public Transform wormVisualParent;

    private Vector3 hiddenVisualLocalPosition;
    private Vector3 emergedVisualLocalPosition;

    private Coroutine stateChangeCoroutine;
    private bool playerIsCurrentlyInSandZone = false;

    void Start()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            else
            {
                Debug.LogError("Player Transform не назначен и объект с тэгом 'Player' не найден! Червь не сможет атаковать.");
                enabled = false;
                return;
            }
        }

        if (wormModel == null)
        {
            Debug.LogError("Ошибка: В поле 'Worm Model' на объекте '" + gameObject.name + "' не назначена 3D-модель червя.");
            enabled = false;
            return;
        }

        if (wormVisualParent == null)
        {
            wormVisualParent = wormModel.transform;
        }

        hiddenVisualLocalPosition = wormVisualParent.localPosition;
        emergedVisualLocalPosition = new Vector3(wormVisualParent.localPosition.x, wormVisualParent.localPosition.y + emergeHeight, wormVisualParent.localPosition.z);

        SetState(WormState.Passive);
    }

    void Update()
    {
        switch (currentState)
        {
            case WormState.Passive:
                if (playerIsCurrentlyInSandZone)
                {
                    SetState(WormState.Attacking);
                }
                break;

            case WormState.Attacking:
                if (!playerIsCurrentlyInSandZone)
                {
                    SetState(WormState.Retreating);
                }
                else
                {
                    Vector3 targetPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, attackSpeed * Time.deltaTime);

                    if (Vector3.Distance(transform.position, playerTransform.position) < attackRange && canAttack)
                    {
                        PlayerHealth playerHealth = playerTransform.GetComponent<PlayerHealth>();
                        if (playerHealth != null)
                        {
                            playerHealth.TakeDamage(attackDamage);
                        }
                        else
                        {
                            Debug.LogWarning("У игрока нет компонента PlayerHealth! Червь не может нанести урон.");
                        }
                        StartCoroutine(AttackCooldown());
                    }
                }
                break;

            case WormState.Retreating:
                if (playerIsCurrentlyInSandZone)
                {
                    SetState(WormState.Attacking);
                }
                break;
        }
    }
    public void PlayerEnteredSandZone()
    {
        playerIsCurrentlyInSandZone = true;
    }

    public void PlayerExitedSandZone()
    {
        playerIsCurrentlyInSandZone = false;
    }

    void SetState(WormState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        Debug.Log("Червь перешел в состояние: " + currentState);

        if (stateChangeCoroutine != null)
        {
            StopCoroutine(stateChangeCoroutine);
        }

        switch (currentState)
        {
            case WormState.Passive:
                stateChangeCoroutine = StartCoroutine(HideWormVisual());
                break;
            case WormState.Attacking:
                stateChangeCoroutine = StartCoroutine(EmergeWormVisual());
                break;
            case WormState.Retreating:
                stateChangeCoroutine = StartCoroutine(HideWormVisual());
                break;
        }
    }

    IEnumerator EmergeWormVisual()
    {
        wormModel.SetActive(true);
        if (wormAttackCollider != null) wormAttackCollider.enabled = true;

        Vector3 startLocalPos = wormVisualParent.localPosition;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * emergeSpeed;
            wormVisualParent.localPosition = Vector3.Lerp(startLocalPos, emergedVisualLocalPosition, t);
            yield return null;
        }
        wormVisualParent.localPosition = emergedVisualLocalPosition;
    }

    IEnumerator HideWormVisual()
    {
        if (wormAttackCollider != null) wormAttackCollider.enabled = false;

        Vector3 startLocalPos = wormVisualParent.localPosition;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * emergeSpeed;
            wormVisualParent.localPosition = Vector3.Lerp(startLocalPos, hiddenVisualLocalPosition, t);
            yield return null;
        }
        wormVisualParent.localPosition = hiddenVisualLocalPosition;

        if (currentState == WormState.Passive)
        {
            wormModel.SetActive(false);
        }
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void OnDrawGizmosSelected()
    {
        if (playerTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}