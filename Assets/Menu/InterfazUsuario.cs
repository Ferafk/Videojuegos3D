using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class InterfazUsuario : MonoBehaviour
{

    public GameObject pauseCanvas;
    public GameObject ganasteCanvas;

    private bool isPaused = false;

    public ItemManager itemManager;
    public CrateBreak crateBreak;

    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemCantidad;

    void Start()
    {
        // Asegurar que el juego comience correctamente desbloqueado
        InitializeGameState();
    }

    void Update()
    {
        HandlePauseInput();
        UpdateUI();
    }

    private void HandlePauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void UpdateUI()
    {
        if (itemManager.currentItemAmount > 0)
        {
            itemName.text = itemManager.currentProjectileName;
            itemCantidad.text = itemManager.currentItemAmount.ToString();
        }
        else
        {
            itemName.text = "Municiones";
            itemCantidad.text = "0";
        }
    }

    private void InitializeGameState()
    {
        // Asegurar que el juego esté activo al iniciar
        isPaused = false;
        Time.timeScale = 1f;
        LockCursor();
        pauseCanvas.SetActive(false);
        if (ganasteCanvas != null) ganasteCanvas.SetActive(false);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        UnlockCursor();
        isPaused = true;
        Debug.Log("Juego pausado");
        pauseCanvas.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        LockCursor();
        isPaused = false;
        Debug.Log("Juego reanudado");
        pauseCanvas.SetActive(false);
    }

    private void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ReturnToMainMenu()
    {
        UnlockCursor();
        SceneManager.LoadScene("MainMenu");
    }

    public void Ganaste()
    {
        PauseGame();
        ganasteCanvas.SetActive(true);
    }

    public void Perdiste()
    {
        UnlockCursor();
        SceneManager.LoadScene("Nivel1");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
