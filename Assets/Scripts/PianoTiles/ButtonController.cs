using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonController : MonoBehaviour
{
    public bool pressing = false;
    private SpriteRenderer sr;
    private GameObject collidingTile;
    private CircleCollider2D col;
    public KeyCode inputButton;
    public static float score = 0;

    public Color particleColor;
    public GameObject particle;
    
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<CircleCollider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        pressing = Input.GetKey(inputButton);

        if (Input.GetKeyDown(inputButton)) Pressed();
        if (Input.GetKeyUp(inputButton)) Released();
        if (pressing) Held();

        if (pressing)
        {
            sr.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
        else sr.color = Color.white;
    }

    private void Pressed()
    {
        if (collidingTile == null) print("Miss");
        else if (collidingTile.CompareTag("Tile"))
        {
            StartCoroutine(DoParticle());
            float colPosY = collidingTile.transform.position.y;
            float dist = colPosY - transform.position.y;

            if (dist < col.radius * 0.2f) score += 100; // Perfect
            else if (dist < col.radius * 0.5f) score += 80; // Great
            else if (dist < col.radius) score += 60; // Good
            else if (colPosY - (transform.position.y + col.radius) > 0) score += 40; // Ok

            collidingTile.GetComponent<TileController>().hasBeenPressed = true;
            collidingTile.GetComponent<SpriteRenderer>().color = Color.green;

            Destroy(collidingTile);
            collidingTile = null;
        }
        else if (collidingTile.CompareTag("SustainTile"))
        {
            SustainTileController controller = collidingTile.GetComponent<SustainTileController>();

            if (!controller.hasBeenStarted)
            {
                float colPosY = collidingTile.transform.position.y - controller.bottomCircleCenterOffset;
                float dist = colPosY - transform.position.y;

                if (dist > col.radius) { }
                else
                {
                    if (dist < col.radius * 0.2f) score += 100; // Perfect
                    else if (dist < col.radius * 0.5f) score += 80f; // Great
                    else if (dist < col.radius) score += 60f; // Good

                    collidingTile.GetComponent<SpriteRenderer>().color = Color.green;
                    controller.hasBeenStarted = true;
                }
            }
        }
    }

    private void Released()
    {
        if (collidingTile == null) { }
        else if (collidingTile.CompareTag("Tile")) { }
        else if (collidingTile.CompareTag("SustainTile"))
        {
            SustainTileController controller = collidingTile.GetComponent<SustainTileController>();

            if (controller.hasBeenStarted && !controller.hasBeenReleased)
            {
                controller.hasBeenReleased = true;
                float colPosY = collidingTile.transform.position.y + controller.bottomCircleCenterOffset;
                float dist = colPosY - transform.position.y;

                if (dist < col.radius * 0.2f) score += 100; // Perfect
                else if (dist < col.radius * 0.5f) score += 80; // Great
                else if (dist < col.radius) score += 60; // Good
                else if (colPosY - (transform.position.y + col.radius) > 0) score += 40; // Ok


                collidingTile.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }
    private void Held()
    {
        if (collidingTile == null) { }
        else if (collidingTile.CompareTag("Tile")) { }
        else if (collidingTile.CompareTag("SustainTile"))
        {
            SustainTileController controller = collidingTile.GetComponent<SustainTileController>();
            if (controller.hasBeenStarted && !controller.hasBeenReleased) score += Time.deltaTime * 400f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collidingTile = collision.gameObject;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collidingTile = null;
        if (collision.gameObject.CompareTag("Tile"))
        {
            TileController controller = collision.gameObject.GetComponent<TileController>();
            if (!controller.hasBeenPressed) collision.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else if (collision.gameObject.CompareTag("SustainTile"))
        {
            SustainTileController controller = collision.gameObject.GetComponent<SustainTileController>();
            if (!controller.hasBeenStarted || !controller.hasBeenReleased) collision.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    private IEnumerator DoParticle()
    {
        GameObject p = Instantiate(particle);
        p.transform.position = transform.position;
        p.SetActive(true);
        p.GetComponent<ParticleSystem>().startColor = particleColor; // deprecated but I dont care enough
        p.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(0.25f);
        p.GetComponent<ParticleSystem>().Stop();
        Destroy(p);
    }
}
