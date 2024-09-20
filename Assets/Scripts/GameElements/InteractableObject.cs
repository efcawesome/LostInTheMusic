using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    private GameObject wsCanvas;
    public TMP_Text openPrompt;
    public string displayText;
    public float fadeInSpeed;
    public float fadeOutSpeed;
    private Coroutine curCo;
    public UnityEvent openEvent;

    private bool hasInteracted = false;

    private void Start()
    {
        openPrompt.alpha = 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Player")) {
            if(curCo != null) StopCoroutine(curCo);
            curCo = StartCoroutine(GameManager.FadeTextIn(openPrompt, fadeInSpeed)); // Fade text in
        }
    }

    private void Update()
    {
        if(hasInteracted && Input.GetAxis("Vertical") == 0) {
            hasInteracted = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetAxis("Vertical") > 0 && collision.gameObject.name.Equals("Player") && !hasInteracted && !GameManager.mapOpen && !GameManager.inventoryOpen)
        {
            hasInteracted = true;
            openEvent.Invoke(); // Call the set event when the player interacts with the object (presses W)
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
        {
            if (curCo != null) StopCoroutine(curCo); // Stop fading in
            curCo = StartCoroutine(GameManager.FadeTextOut(openPrompt, fadeOutSpeed)); // Fade text out
        }
    }

    private void OnDestroy()
    {
        if(openPrompt != null) GameObject.Find("GameManager").GetComponent<GameManager>().GuaranteedFadeTextOut(openPrompt, fadeOutSpeed);
    }
}
