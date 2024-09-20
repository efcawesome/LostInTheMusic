using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerAI : MonoBehaviour
{
    public LayerMask groundLayer;
    private bool facingRight = true;
    private BoxCollider2D cd;
    private Rigidbody2D rb;
    public float moveSpeed;
    private bool canMoveForward;
    public Transform feetPos;

    // Start is called before the first frame update
    void Start()
    {
        cd = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!canMoveForward) facingRight = !facingRight;

        var vel = rb.velocity;
        vel.x = moveSpeed * (facingRight ? 1 : -1); // Move right or left
        rb.velocity = vel;
    }

    private void Update()
    {
        canMoveForward = facingRight ? // Check if there is ground in front of the lil guy
            Physics2D.OverlapBox(new Vector2(cd.transform.position.x + cd.offset.x + cd.size.x / 2 + 0.05f, cd.transform.position.y + cd.offset.y - cd.size.y / 2), new Vector2(0.1f, 0.1f), 0, groundLayer) :
            Physics2D.OverlapBox(new Vector2(cd.transform.position.x + cd.offset.x - cd.size.x / 2 - 0.05f, cd.transform.position.y + cd.offset.y - cd.size.y / 2), new Vector2(0.1f, 0.1f), 0, groundLayer);

        GetComponent<SpriteRenderer>().flipX = !facingRight; // Flip sprite if facing left
    }
}
