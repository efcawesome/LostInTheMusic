using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageableObject : MonoBehaviour
{
    public int life;
    public UnityEvent onHitEvent;
    public UnityEvent onDeathEvent;

    public void Hit(int damage)
    {
        onHitEvent.Invoke();

        life -= damage;
        Debug.Log(life);
        if (life <= 0) Kill();
    }

    public void Kill()
    {
        onDeathEvent.Invoke();

        Destroy(gameObject);
    }
}
