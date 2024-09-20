using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretConcealer : MonoBehaviour
{
    private SpriteRenderer sr;
    private Coroutine inCo;
    private Coroutine outCo;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
        {
            if (inCo != null) StopCoroutine(inCo);
            outCo = StartCoroutine(FadeOut());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
        {
            if (outCo != null) StopCoroutine(outCo);
            inCo = StartCoroutine(FadeIn());
        }
    }
    private IEnumerator FadeIn()
    {
        while(sr.color.a < 1)
        {
            var col = sr.color;
            col.a += Time.deltaTime * 5;
            sr.color = col;
            yield return null;
        }

        var c = sr.color;
        c.a = 1;
        sr.color = c;
    }

    private IEnumerator FadeOut()
    {
        while (sr.color.a > 0)
        {
            var col = sr.color;
            col.a -= Time.deltaTime * 5;
            sr.color = col;
            yield return null;
        }

        var c = sr.color;
        c.a = 0;
        sr.color = c;
    }
}
