using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbController : MonoBehaviour
{
    private float initPosY;
    private float lifeTime;

    public float undulationSpeed;
    public float undulationAmplitude;

    private void Start()
    {
        initPosY = transform.position.y;
    }

    private void Update()
    {
        transform.position = new Vector2(transform.position.x, initPosY + undulationAmplitude * Mathf.Sin(lifeTime * undulationSpeed));
        lifeTime += Time.deltaTime;
    }

    public void OnHit()
    {

    }

    public void OnDeath()
    {

    }
}
