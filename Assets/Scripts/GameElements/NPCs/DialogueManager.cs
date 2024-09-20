using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour
{
    private bool dialogueOpen = false;
    private PlayerMovement pm;
    private PlayerAttack pa;
    public DialogueRunner dr;
    public Animator animator;
    public string[] dialogueTitles;
    public int currDialogueInd;

    private void Start()
    {
        pm = GameObject.Find("Player").GetComponent<PlayerMovement>();
        pa = GameObject.Find("Player").GetComponent<PlayerAttack>();
    }

    private void Update()
    {
        if(animator != null)
        {
            animator.SetBool("dialogueOpen", dialogueOpen);
        }
    }

    public void OnOpen()
    {
        if (!dialogueOpen)
        {
            dialogueOpen = true;
            pm.RevokeControl();
            dr.StartDialogue(dialogueTitles[currDialogueInd]); // Start Dialogue Runner

            if(currDialogueInd < dialogueTitles.Length - 1) currDialogueInd++; // Go to next dialogue
        }
    }

    public void GiveMovement()
    {
        dialogueOpen = false;
        pm.GiveControl();
    }

    /*public void OnOpen() // Called when the player presses up by InteractableObject
    {
        if(!dialogueOpen)
        {
            pm.canMove = false;
            pm.canJump = false;
            OpenDialogue();
        }
    }

    private void OpenDialogue() // Open a new dialogue UI
    {
        dialogueOpen = true;

        dialogue = Instantiate(dialogueTemplate);
        dialogue.GetComponent<TextMeshProUGUI>().text = dialogues[currDialogueInd];

        dialogue.transform.SetParent(transform);
        dialogue.transform.localPosition = new Vector2(0, yOffset);

        dialogue.SetActive(true);

        if (currDialogueInd < dialogues.Count - 1) currDialogueInd++;
    }

    private void ContinueDialogue() // Progress dialogue to the next dialogue text
    {
        if (currDialogueInd < dialogues.Count - 1) // If not on last dialogue continue the dialogue
        {
            dialogue.GetComponent<TextMeshProUGUI>().text = dialogues[currDialogueInd];
            currDialogueInd++;
        }
        else // Otherwise close the dialogue
        {
            StartCoroutine(CloseDialogue());
        }
        
    }

    private IEnumerator CloseDialogue() // Fade text out and then destroy the dialogue GameObject
    {
        yield return StartCoroutine(GameManager.FadeTextOut(dialogue.GetComponent<TextMeshProUGUI>(), 5));

        dialogueOpen = false;

        pm.canMove = true;
        pm.canJump = true;

        Destroy(dialogue);
        dialogue = null;
    }*/
}
