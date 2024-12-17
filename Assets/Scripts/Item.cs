using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    [Header("Floating Parameters")]
    public float floatHeight = 0.5f;
    public float floatSpeed = 1f;
    public float rotationSpeed = 30f;

    private Vector3 startPosition;

    public bool isReceta;

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

}
