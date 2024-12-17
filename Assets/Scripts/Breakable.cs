using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public int health = 1;

    public void TakeDamage()
    {
        health --;
    }

    public void Damage(int damage)
    {
        health -= damage;
    }

}
