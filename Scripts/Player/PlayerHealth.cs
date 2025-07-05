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
            Debug.LogError("RespawnManagerObject не назначен в инспекторе PlayerHealth!");
            respawnManager = FindObjectOfType<RespawnManager>();
            if (respawnManager == null)
            {
                Debug.LogError("RespawnManager не найден в сцене!");
            }
        }
        else
        {
            respawnManager = respawnManagerObject.GetComponent<RespawnManager>();
            if (respawnManager == null)
            {
                Debug.LogError("На назначенном RespawnManagerObject нет компонента RespawnManager!");
            }
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Игрок получил урон. Текущее здоровье: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Игрок мертв!");

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
