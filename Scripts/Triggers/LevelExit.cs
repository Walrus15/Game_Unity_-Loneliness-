using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [Header("Название следующей сцены")]
    public string nextLevelName = "Level2";

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LoadNextLevel();
        }
    }

    void LoadNextLevel()
    {
        if (Application.CanStreamedLevelBeLoaded(nextLevelName))
        {
            SceneManager.LoadScene(nextLevelName);
        }
        else
        {
            Debug.LogError("Сцена с названием '" + nextLevelName + "' не найдена или не добавлена в Build Settings!");
            Debug.Log("Пожалуйста, добавьте сцену '" + nextLevelName + "' в File -> Build Settings...");
        }
    }
}