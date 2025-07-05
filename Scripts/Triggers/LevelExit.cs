using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [Header("�������� ��������� �����")]
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
            Debug.LogError("����� � ��������� '" + nextLevelName + "' �� ������� ��� �� ��������� � Build Settings!");
            Debug.Log("����������, �������� ����� '" + nextLevelName + "' � File -> Build Settings...");
        }
    }
}