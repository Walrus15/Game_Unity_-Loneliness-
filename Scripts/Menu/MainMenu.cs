using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("�������� UI")]
    public GameObject optionsPanel;

    [Header("��� ������� �����")]
    public string gameSceneName = "Lvl_1_School";

    [Header("��������� �����")]
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
                Debug.LogWarning("AudioSource ��� UI �� �������� � �� ������ �� ������� " + gameObject.name + ". �������� �����. �� �������� ��������� ���.");
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