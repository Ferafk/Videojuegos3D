using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateBreak : MonoBehaviour
{
    public ParticleSystem breakParticles;
    public AudioClip breakSound;
    
    public List<Breakable> boxes = new List<Breakable>();


    void Start()
    {
        RegisterBoxes();

    }

    private void Update()
    {
        // Monitorear las cajas registradas
        for (int i = boxes.Count - 1; i >= 0; i--)
        {
            if (boxes[i].health <= 0)
            {
                DestroyBox(boxes[i]);
                boxes.RemoveAt(i);
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

    private void DestroyBox(Breakable box)
    {
        if (breakParticles != null && breakSound != null)
        {
            Instantiate(breakParticles, box.transform.position, Quaternion.identity).Play();
            AudioSource.PlayClipAtPoint(breakSound, box.transform.position);
        }

        Destroy(box.gameObject);
    }

}
