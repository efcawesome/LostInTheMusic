using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SustainTileController : MonoBehaviour
{
    public float speed;
    public float bottomCircleCenterOffset;
    public bool hasBeenStarted = false;
    public bool hasBeenReleased = false;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.down * speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y + bottomCircleCenterOffset < -7) Destroy(gameObject);
    }
}
