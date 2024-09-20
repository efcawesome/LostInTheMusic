using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieAfter : MonoBehaviour
{
    private float timeAlive = 0.0f;
    public float lifeTime;
    private bool isDying = false;
    public float fadeSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeAlive += Time.deltaTime;
        if (!isDying && timeAlive > lifeTime) StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        isDying = true;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        while(sr.color.a > 0f)
        {
            var col = sr.color;
            col.a -= Time.deltaTime * fadeSpeed;
            sr.color = col;

            yield return null;
        }

        Destroy(gameObject);
    }
}
