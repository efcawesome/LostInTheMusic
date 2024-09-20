using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class DoorFunc : MonoBehaviour
{

    private bool dialogueOpen = false;
    private PlayerMovement pm;
    private PlayerAttack pa;
    public DialogueRunner dr;
    public Animator animator;
    public GameObject soto;

    private void Start()
    {
        soto.SetActive(false);
        pm = GameObject.Find("Player").GetComponent<PlayerMovement>();
        pa = GameObject.Find("Player").GetComponent<PlayerAttack>();
    }

    public void OnOpen()
    {
        if (!dialogueOpen)
        {
            if(GameObject.Find("GameManager").GetComponent<GameManager>().HasAllSpirits())
            {
                GameObject.Find("GameManager").GetComponent<GameManager>().StartHideScreen(1f);
                SceneManager.LoadScene("17_EndingScene");
                pm.gameObject.transform.position = new Vector3(-1.80751169f, -4.90840006f, 0);
            }
            else
            {
                StartCoroutine(TalkSpirit());
                dialogueOpen = true;
            }
        }
    }

    private IEnumerator TalkSpirit()
    {
        pm.RevokeControl();
     
        soto.SetActive(true);
        animator.Play("SpiritOfTheOrchestraSpawn");
        yield return new WaitForSeconds(2f);
       
        dr.StartDialogue("NotAllSpirits");

        yield return new WaitUntil(() => !dialogueOpen);

        SpriteRenderer sr = soto.GetComponent<SpriteRenderer>();

        while(sr.color.a > 0)
        {
            var col = sr.color;
            col.a -= Time.deltaTime * 5;
            sr.color = col;
            yield return null;
        }

        soto.SetActive(false);
        sr.color = Color.white;
        pm.GiveControl();
    }

    public void CloseDialogue()
    {
        dialogueOpen = false;
    }

}
