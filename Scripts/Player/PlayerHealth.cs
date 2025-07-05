using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth = 1;
    public GameObject respawnManagerObject;
    private RespawnManager respawnManager;

    void Start()
    {
        if (respawnManagerObject == null)
        {
            Debug.LogError("RespawnManagerObject �� �������� � ���������� PlayerHealth!");
            respawnManager = FindObjectOfType<RespawnManager>();
            if (respawnManager == null)
            {
                Debug.LogError("RespawnManager �� ������ � �����!");
            }
        }
        else
        {
            respawnManager = respawnManagerObject.GetComponent<RespawnManager>();
            if (respawnManager == null)
            {
                Debug.LogError("�� ����������� RespawnManagerObject ��� ���������� RespawnManager!");
            }
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("����� ������� ����. ������� ��������: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("����� �����!");

        if (respawnManager != null)
        {
            respawnManager.RespawnPlayer(this.gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }

        currentHealth = 1;
    }
}
