using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatController : MonoBehaviour
{
    private Rigidbody2D rb;
    public Action landedCallback;

    private bool landed = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (transform.position.y < -7) Destroy(gameObject);
        if (!landed && rb.velocity == Vector2.zero)
        {
            landed = true;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            landedCallback();
        }
    }
}
