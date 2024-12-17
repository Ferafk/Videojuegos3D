using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    [Header("Floating Parameters")]
    public float floatHeight = 0.5f;
    public float floatSpeed = 1f;
    public float rotationSpeed = 30f;

    [Header("Attraction Parameters")]
    public float attractionRadius = 3f;
    public float attractionSpeed = 5f;

    private Vector3 startPosition;
    private bool isBeingPickedUp = false;

    public ItemType ingredientType;


    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = startPosition + Vector3.up * yOffset;

        transform.RotateAround(transform.position, Vector3.up, rotationSpeed * Time.deltaTime);
    }

    // Método opcional para depuración visual del radio de atracción
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attractionRadius);
    }

}
