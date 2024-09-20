using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatManager : MonoBehaviour
{
    public List<Sprite> catSprites;
    private float lifeTime;

    public AudioClip meow;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        GetComponent<SpriteRenderer>().sprite = catSprites[Random.Range(0, catSprites.Count)];
    }

    private void Update()
    {
        if(transform.position.y < -8) Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        audioSource.PlayOneShot(meow, 0.25f);
    }
}
