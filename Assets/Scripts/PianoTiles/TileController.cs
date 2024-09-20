using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public float speed;
    public bool hasBeenPressed;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.down * speed;
    }

    private void Update()
    {
        if (transform.position.y < -7) Destroy(gameObject);
    }
}
