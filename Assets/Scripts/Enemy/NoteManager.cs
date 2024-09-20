using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NoteManager : MonoBehaviour
{
    public bool isHit;
    public float timeAlive;
    private float intensity;
    public float flickerSpeed;
    public float flickerAmp;
    public AudioClip goodSound;
    public AudioSource audioSource;

    private void Start()
    {
        intensity = GetComponentInChildren<Light2D>().intensity;
    }

    private void Update()
    {
        timeAlive += Time.deltaTime;

        GetComponentInChildren<Light2D>().intensity = intensity + flickerAmp * Mathf.Sin(flickerSpeed * timeAlive);
    }
}
