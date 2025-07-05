using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Элементы UI")]
    public GameObject optionsPanel;

    [Header("Имя игровой сцены")]
    public string gameSceneName = "Lvl_1_School";

    [Header("Настройки Аудио")]
    public AudioSource uiAudioSource;
    public AudioClip buttonClickSound;

    void Start()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (uiAudioSource == null)
        {
            uiAudioSource = GetComponent<AudioSource>();
            if (uiAudioSource == null)
            {
                uiAudioSource = gameObject.AddComponent<AudioSource>();
                Debug.LogWarning("AudioSource для UI не назначен и не найден на объекте " + gameObject.name + ". Добавлен новый. Не забудьте настроить его.");
            }
        }
    }

    void PlayButtonClickSound()
    {
        if (uiAudioSource != null && buttonClickSound != null)
        {
            uiAudioSource.PlayOneShot(buttonClickSound);
        }
    }

    public void PlayGame()
    {
        PlayButtonClickSound();
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenOptions()
    {
        PlayButtonClickSound();
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
        }
    }

    public void CloseOptions()
    {
        PlayButtonClickSound();
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }

    public void ExitGame()
    {
        PlayButtonClickSound();
        Application.Quit();
    }
}