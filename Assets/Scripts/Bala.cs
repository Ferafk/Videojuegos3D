using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bala : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;
    public float rotationspeed = 360f;

    private float lifeTimer;
    private Vector3 shootDir;

    private void OnEnable()
    {
        lifeTimer = lifeTime;
        shootDir = transform.forward;
    }

    private void Update()
    {
        transform.position += shootDir * speed * Time.deltaTime;
        transform.Rotate(Vector3.right, rotationspeed * Time.deltaTime);

        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0 )
            gameObject.SetActive(false);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Breakable"))
        {
            Breakable tobreak = collision.gameObject.GetComponent<Breakable>();
            tobreak.TakeDamage();
            lifeTimer = 0;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyBehab enemyB = collision.gameObject.GetComponent<EnemyBehab>();
            enemyB.ReceiveDamage(1, false);
        }
    }

}
