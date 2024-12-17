using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [Header("Canvas References")]
    public GameObject levelCanvas;
    public GameObject helpCanvas;

    public void ShowLevelsCanvas()
    {
        if (levelCanvas != null)
        {
            levelCanvas.SetActive(true);
        }
    }

    public void CloseLevelsCanvas()
    {
        if (levelCanvas != null)
        {
            levelCanvas.SetActive(false);
        }
    }

    public void ShowHelpCanvas()
    {
        if (helpCanvas != null)
        {
            helpCanvas.SetActive(true);
        }
    }

    public void CloseHelpCanvas()
    {
        if (helpCanvas != null)
        {
            helpCanvas.SetActive(false);
        }
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene("Nivel1");
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("Nivel2");
    }

    // Cerrar el juego
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
