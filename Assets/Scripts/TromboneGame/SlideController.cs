using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideController : MonoBehaviour
{
    public static int score;
    private SpriteRenderer sr;
    public float moveSpeed;
    private bool pressing;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Jump"))
        {
            sr.color = Color.green;
            pressing = true;
        }
        else
        {
            sr.color = Color.white;
            pressing = false;
        }
        if (Input.GetAxisRaw("Vertical") < 0 && transform.position.y > -4.5f)
        {
            transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        }
        else if (Input.GetAxisRaw("Vertical") > 0 && transform.position.y < 4.5f)
        {
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("TromboneTile"))
        {
            if (pressing)
            {
                collision.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                score += 1;
            }
            else
            {
                collision.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
    }
}
