using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketManager : MonoBehaviour
{
    public float speed;
    private Rigidbody2D rb;
    private AudioSource audioSource;
    public AudioClip completionClip;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetAxisRaw("Horizontal") > 0 && transform.position.x <= 5.065f) rb.velocity = Vector2.right * speed;
        else if (Input.GetAxisRaw("Horizontal") < 0 && transform.position.x >= -5.06f) rb.velocity = Vector2.left * speed;
        else rb.velocity = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag.Contains("Cat"))
        {
            CatGameController.score++;
            audioSource.PlayOneShot(completionClip, 0.5f);
            Destroy(collision.gameObject);
        }
    }
}
