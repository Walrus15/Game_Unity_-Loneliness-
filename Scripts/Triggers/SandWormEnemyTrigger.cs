using UnityEngine;

public class SandZoneTriggerHandler : MonoBehaviour
{
    [Header("Настройки игрока")]
    public string playerTag = "Player";

    [Header("Ссылка на Врага-Червя")]
    public SandWormEnemyBrain sandWormEnemyBrain;

    void Start()
    {
        if (sandWormEnemyBrain == null)
        {
            Debug.LogError("Ошибка: В поле 'Sand Worm Enemy Brain' на объекте '" + gameObject.name + "' не назначен скрипт SandWormEnemyBrain. " +
                           "Перетащите GameObject червя в это поле в инспекторе.");
            enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Игрок вошел в зону песка. Сообщаем червю.");
            sandWormEnemyBrain.PlayerEnteredSandZone();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Игрок вышел из зоны песка. Сообщаем червю.");
            sandWormEnemyBrain.PlayerExitedSandZone();
        }
    }
}