using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class HelpTextManager : MonoBehaviour
{
    public TMPro.TMP_Text helpText;
    public Func<bool> gotHelp;
    public Func<bool> preHelp;
    private UnityAction savingAction;
    private bool isStopping = false;
    public string type;
    private Coroutine fadeInCor;
    private bool helpAppeared = false;
    private bool canDisappear = false;

    private void Start()
    {
        switch(type)
        {
            case "inventory":
                savingAction += SaveInventory;
                break;
            case "dash":
                savingAction += SaveDash;
                break;
            case "glide":
                savingAction += SaveGlide;
                break;
            case "map":
                savingAction += SaveMap;
                break;
            default: 
                savingAction = null; 
                break;
        }

        helpText.alpha = 0;
        gotHelp += GetHelpType();
        preHelp += GetPreHelp();

        switch (type)
        {
            case "inventory":
                if (GameManager.seenInventoryHelp) Destroy(gameObject);
                break;
            case "dash":
                if (GameManager.seenDashHelp) Destroy(gameObject);
                break;
            case "glide":
                if (GameManager.seenGlideHelp) Destroy(gameObject);
                break;
            case "map":
                if (GameManager.seenMapHelp) Destroy(gameObject);
                break;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!helpAppeared && collision.gameObject.name.Equals("Player") && (preHelp == null || preHelp.Invoke()))
        {
            StartCoroutine(StartHelp());
            helpAppeared = true;
        }
    }

    private void Update()
    {
        if(canDisappear && !isStopping && gotHelp.Invoke())
        {
            isStopping = true;
            StartCoroutine(StopHelp());
        }
    }

    private IEnumerator StartHelp()
    {
        yield return new WaitForSeconds(1);
        yield return fadeInCor = StartCoroutine(GameManager.FadeTextIn(helpText, 5));
        canDisappear = true;
    }

    private IEnumerator StopHelp()
    {
        yield return StartCoroutine(GameObject.Find("GameManager").GetComponent<GameManager>().WaitFadeTextOut(helpText, 5));
        Destroy(helpText);
        Destroy(gameObject);
        if(savingAction != null) savingAction.Invoke();
    }

    private Func<bool> GetHelpType()
    {
        switch(type)
        {
            case "walk":
                return () => GameObject.Find("Player").GetComponent<Rigidbody2D>().velocity.x != 0;
            case "interact":
                return () => Input.GetAxisRaw("Vertical") > 0 || GameObject.Find("Dialogue System").GetComponent<DialogueRunner>().IsDialogueRunning;
            case "attack":
                return () => Input.GetButtonDown("Attack");
            case "pogo":
                return () => Input.GetAxisRaw("Vertical") < 0 && Input.GetButton("Attack");
            case "glidepogo":
                return () => Input.GetAxisRaw("Vertical") < 0 && Input.GetButton("Attack") && GameObject.Find("Player").GetComponent<Rigidbody2D>().gravityScale < 1;
            case "inventory":
                return () => Input.GetButtonDown("Inventory");
            case "dash":
                return () => Input.GetButtonDown("Dash");
            case "glide":
                return () => GameObject.Find("Player").GetComponent<Rigidbody2D>().gravityScale < 3;
            case "map":
                return () => Input.GetButtonDown("Map");
            default:
                return () => false;
        }
    }

    private Func<bool> GetPreHelp()
    {
        switch(type)
        {
            case "attack": return () => GameObject.Find("Player").GetComponent<PlayerAttack>().hasBaton;
            case "dash": return () => GameObject.Find("Player").GetComponent<PlayerMovement>().canDash;
            case "glide": return () => GameObject.Find("Player").GetComponent<PlayerMovement>().slowFallUnlocked;
            case "map": return () => GameObject.Find("InventoryHelpObject") == null && GameObject.Find("Player").GetComponent<PlayerMovement>().canMove;
            default: return null;
        }
    }

    private void SaveInventory()
    {
        GameManager.seenInventoryHelp = true;
    }

    private void SaveDash()
    {
        GameManager.seenDashHelp = true;
    }

    private void SaveGlide()
    {
        GameManager.seenGlideHelp = true;
    }

    private void SaveMap()
    {
        GameManager.seenMapHelp = true;
    }
}
