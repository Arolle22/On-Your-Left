using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip PlaySFX;
    public AudioClip CancelSFX;
    public AudioClip SettingsSFX;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Tutorial()
    {
        StartCoroutine(PlaySFXAndLoadScene(1));
    }

    public void PlayGame()
    {
        StartCoroutine(PlaySFXAndLoadScene(2));
    }

    public void QuitGame()
    {
        StartCoroutine(PlaySFXThenQuit());
    }

    public void GameSettings()
    {
        if (SettingsSFX != null)
            audioSource.PlayOneShot(SettingsSFX);
    }

    public void Cancel()
    {
        if (CancelSFX != null)
            audioSource.PlayOneShot(CancelSFX);
    }

    IEnumerator PlaySFXAndLoadScene(int sceneIndex)
    {
        if (PlaySFX != null)
        {
            audioSource.PlayOneShot(PlaySFX);
            yield return new WaitForSeconds(PlaySFX.length);
        }
        SceneManager.LoadScene(sceneIndex);
    }

    IEnumerator PlaySFXThenQuit()
    {
        if (CancelSFX != null)
        {
            audioSource.PlayOneShot(CancelSFX);
            yield return new WaitForSeconds(CancelSFX.length);
        }
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
