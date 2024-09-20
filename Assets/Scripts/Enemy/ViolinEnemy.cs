using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViolinEnemy : MonoBehaviour
{
    private ShootController sc;
    public float speed;
    private bool movingRight = false;
    public float moveDist;
    private float initX;
    public float moveSpeed;
    private Rigidbody2D rb;
    public int noteAmount;

    public AudioClip badSound;

    public List<AudioClip> soundList;
    public List<float> timeList;

    private AudioSource audioSource;

    private Coroutine ssc;

    private Color[] colors = new Color[5] { Color.blue, Color.green, Color.yellow, Color.cyan, Color.magenta };

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(-moveSpeed, 0);
        initX = transform.position.x;
        sc = GetComponent<ShootController>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!movingRight && transform.position.x < initX - moveDist)
        {
            movingRight = true;
            rb.velocity = new Vector2(moveSpeed, 0);
        }
        else if (movingRight && transform.position.x > initX)
        {
            movingRight = false;
            rb.velocity = new Vector2(-moveSpeed, 0);
        }

    }

    public void StartShootCycle()
    {
        ssc = StartCoroutine(ShootCycle());
    }

    public void StopShootCycle()
    {
        if(ssc != null) StopCoroutine(ssc);
    }

    private IEnumerator ShootCycle()
    {
        GameObject player = GameObject.Find("Player");
        for(int i = 0; i < noteAmount; i++)
        {
            sc.ShootNote((player.transform.position - transform.position).normalized * speed, colors[i % colors.Length], soundList[i]);
            audioSource.PlayOneShot(badSound);
            yield return new WaitForSeconds(timeList[i]);
        }
    }
}
