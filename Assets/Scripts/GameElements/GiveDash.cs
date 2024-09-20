using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveDash : MonoBehaviour
{
    private void Start()
    {
        if(GameObject.Find("Player").GetComponent<PlayerMovement>().canDash) Destroy(gameObject);
    }
    public void OnPickup()
    {
        GameObject.Find("Player").GetComponent<PlayerMovement>().canDash = true;
        // Unlock screen
        Destroy(gameObject);
    }
}
