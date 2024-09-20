using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Yarn.Unity;
using UnityEngine.SceneManagement;

public class StartCreditsDialogueManager : MonoBehaviour
{
    private bool dialogueOpen = false;
    private PlayerMovement pm;
    public DialogueRunner dr;
    public string[] dialogueTitles;
    public int currDialogueInd;

    private void Start()
    {
        pm = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    public void OnOpen()
    {
        if (!dialogueOpen && SceneManager.sceneCount < 2)
        {
            dialogueOpen = true;
            pm.RevokeControl();
            dr.StartDialogue(dialogueTitles[currDialogueInd]); // Start Dialogue Runner

            if (currDialogueInd < dialogueTitles.Length - 1) currDialogueInd++; // Go to next dialogue
        }
    }

    public void CloseDialogue()
    {
        GameManager.talkedToSpirit = true;

        if(dr.Dialogue.CurrentNode == "StartCredits")
        {
            dialogueOpen = false;
            GameObject.Find("GameManager").GetComponent<GameManager>().StartCredits();
        }
        else GiveMovement();
    }

    private void GiveMovement()
    {
        dialogueOpen = false;
        pm.GiveControl();
    }
}
