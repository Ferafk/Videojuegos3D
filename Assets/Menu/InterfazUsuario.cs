using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class InterfazUsuario : MonoBehaviour
{

    public GameObject pauseCanvas;

    private bool isPaused = false;

    public ItemManager itemManager;
    private ItemPickupData itemPickupData;

    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemCantidad;

    void Update()
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

    public void PauseGame()
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        isPaused = true;
        Debug.Log("Juego pausado");
        pauseCanvas.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
        Debug.Log("Juego reanudado");
        pauseCanvas.SetActive(false);
    }

    void OnEnable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnDisable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
