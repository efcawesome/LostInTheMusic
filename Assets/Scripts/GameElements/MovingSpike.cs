using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using UnityEngine;

public class MovingSpike : MonoBehaviour
{
    public bool extended = false;
    public float extendTime;
    private float extendTimer;

    private bool coroutineRunning = false;

    public string extendDirection;
    public float extendSpeed;

    private Vector2 initPos;

    void Start()
    {
        switch (extendDirection)
        {
            case "up":
                initPos = extended ? transform.position + Vector3.down : transform.position;
                break;
            case "down":
                initPos = extended ? transform.position + Vector3.up : transform.position;
                break;
            case "right":
                initPos = extended ? transform.position + Vector3.left : transform.position;
                break;
            case "left":
                initPos = extended ? transform.position + Vector3.right : transform.position;
                break;
        }

        extendTimer = extendTime;
    }

    void Update()
    {
        //extendTimer -= Time.deltaTime;
        //if (!coroutineRunning && extendTimer <= 0)
        //{
        //    StartCoroutine(SwapState());
        //}
    }

    public IEnumerator SwapState()
    {
        coroutineRunning = true;
        extended = !extended;

        float t = 0;
        if(!extended) // If down
        {
            switch(extendDirection)
            {
                case "up":
                    while (transform.position.y < initPos.y + 1)
                    {
                        t += Time.deltaTime;
                        transform.position = Vector2.Lerp(transform.position, initPos + Vector2.up, t * extendSpeed);
                        yield return null;
                    }
                    break;
                case "down":
                    while (transform.position.y > initPos.y - 1)
                    {
                        t += Time.deltaTime;
                        transform.position = Vector2.Lerp(transform.position, initPos + Vector2.down, t * extendSpeed);
                        yield return null;
                    }
                    break;
                case "right":
                    while (transform.position.x < initPos.x + 1)
                    {
                        t += Time.deltaTime;
                        transform.position = Vector2.Lerp(transform.position, initPos + Vector2.right, t * extendSpeed);
                        yield return null;
                    }
                    break;
                case "left":
                    while (transform.position.x > initPos.x - 1)
                    {
                        t += Time.deltaTime;
                        transform.position = Vector2.Lerp(transform.position, initPos + Vector2.left, t * extendSpeed);
                        yield return null;
                    }
                    break;
            }
            
        }
        else // Otherwise unextend
        {
            switch(extendDirection)
            {
                case "up":
                    while (transform.position.y > initPos.y)
                    {
                        t += Time.deltaTime;
                        transform.position = Vector2.Lerp(transform.position, initPos, t * extendSpeed);
                        yield return null;
                    }
                    break;
                case "down":
                    while (transform.position.y < initPos.y)
                    {
                        t += Time.deltaTime;
                        transform.position = Vector2.Lerp(transform.position, initPos, t * extendSpeed);
                        yield return null;
                    }
                    break;
                case "right":
                    while (transform.position.x > initPos.x)
                    {
                        t += Time.deltaTime;
                        transform.position = Vector2.Lerp(transform.position, initPos, t * extendSpeed);
                        yield return null;
                    }
                    break;
                case "left":
                    while (transform.position.x < initPos.x)
                    {
                        t += Time.deltaTime;
                        transform.position = Vector2.Lerp(transform.position, initPos, t * extendSpeed);
                        yield return null;
                    }
                    break;
            }
            
        }

        extendTimer = extendTime; // Reset Timer
        coroutineRunning = false;
    }
}
