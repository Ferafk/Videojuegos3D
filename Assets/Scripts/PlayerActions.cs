using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public Animator animator;
    public ObjectPool objectPool;
    public Transform firepoint;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();

    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");

            GameObject bullet = objectPool.GetPooledObject();
            bullet.transform.position = firepoint.position;
            bullet.transform.rotation = firepoint.rotation;
            bullet.SetActive(true);
        }

        if (Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("Eat");
        }

    }

}
