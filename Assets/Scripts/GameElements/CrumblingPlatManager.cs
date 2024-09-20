using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumblingPlatManager : MonoBehaviour
{
    private Animator animator;
    private BoxCollider2D bc;
    private SpriteRenderer sr;
    private AudioSource audioSource;
    public AudioClip jingle;

    private bool crumbling = false;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Equals("Player") && collision.gameObject.transform.position.y - (2.87f - 1.125f) > transform.position.y && !crumbling) StartCoroutine(BeginCrumble());
    }

    private IEnumerator BeginCrumble()
    {
        crumbling = true;
        animator.Play("Crumble");
        audioSource.PlayOneShot(jingle, 1f);
        yield return new WaitForSeconds(110f/180f);

        bc.enabled = false;
        sr.color = new Color(1, 1, 1, 0);

        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3f);

        animator.Play("Idle");

        bc.enabled = true;

        while (sr.color.a < 1)
        {
            var col = sr.color;
            col.a += Time.deltaTime * 10f;
            sr.color = col;

            yield return null;
        }

        sr.color = Color.white;
        crumbling = false;
    }
}
