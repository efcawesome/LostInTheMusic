using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TromboneTile : MonoBehaviour
{
    public float moveSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;
    }
}
