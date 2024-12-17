using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CrateBreak : MonoBehaviour
{
    public ParticleSystem breakParticles;
    public AudioClip breakSound;
    public InterfazUsuario breakUsuario;

    public List<EnemyBehab> Enemies = new List<EnemyBehab>();
    public List<Breakable> boxes = new List<Breakable>();

    // Referencias a los textos de TextMeshPro
    public TextMeshProUGUI boxesText;
    public TextMeshProUGUI enemiesText;

    private bool isInitialized;

    void Start()
    {
        RegisterBoxes();
        RegisterEnemies();
        UpdateUI();
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;

        // Monitorear las cajas registradas
        for (int i = boxes.Count - 1; i >= 0; i--)
        {
            if (boxes[i].health <= 0)
            {
                DestroyBox(boxes[i]);
                boxes.RemoveAt(i);
                UpdateUI(); // Actualiza la interfaz después de destruir una caja
            }
        }

        // Monitorear los enemigos registrados
        for (int i = Enemies.Count - 1; i >= 0; i--)
        {
            if (Enemies[i] == null) // Si el enemigo ha sido destruido
            {
                Enemies.RemoveAt(i);
                UpdateUI(); // Actualiza la interfaz después de eliminar un enemigo
            }
        }

    }

    public void RegisterBoxes()
    {
        Breakable[] boxesNew = FindObjectsOfType<Breakable>();

        if (boxesNew != null)
        {
            foreach (Breakable b in boxesNew)
            {
                boxes.Add(b);
            }
        }
    }

    public void RegisterEnemies()
    {
        EnemyBehab[] enemiesNew = FindObjectsOfType<EnemyBehab>();

        if (enemiesNew != null)
        {
            foreach (EnemyBehab b in enemiesNew)
            {
                Enemies.Add(b);
            }
        }
    }

    private void DestroyBox(Breakable box)
    {
        if (breakParticles != null && breakSound != null)
        {
            Instantiate(breakParticles, box.transform.position, Quaternion.identity).Play();
            AudioSource.PlayClipAtPoint(breakSound, box.transform.position);
        }

        Destroy(box.gameObject);
    }

    private void UpdateUI()
    {
        // Actualiza los textos de la interfaz
        if (boxesText != null)
        {
            boxesText.text = $"Cajas: {boxes.Count}";
        }

        if (enemiesText != null)
        {
            enemiesText.text = $"Enemigos: {Enemies.Count}";
        }

        if (isInitialized) // Solo verifica condiciones después de la inicialización
        {
            CheckWinOrLose();
        }
    }

    private void CheckWinOrLose()
    {
        // Llamar a Ganaste si los enemigos son 0
        if (Enemies.Count == 0 && breakUsuario != null)
        {
            breakUsuario.Ganaste();
        }

        // Llamar a Perdiste si las cajas son 0
        if (boxes.Count == 0 && breakUsuario != null)
        {
            breakUsuario.Perdiste();
        }
    }
}
