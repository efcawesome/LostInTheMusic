using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn.Unity;

public class IntroCutsceneManager : MonoBehaviour
{
    public Image sr;
    public DialogueRunner runner;
    private bool dialogueOpen;
    // Start is called before the first frame update
    void Start()
    {
        sr.color = new Color(1, 1, 1, 0);
        StartCoroutine(Cutscene());
    }

    private IEnumerator Cutscene()
    {
        yield return new WaitForSecondsRealtime(1f);

        while(sr.color.a < 1)
        {
            var col = sr.color;
            col.a += Time.deltaTime;
            sr.color = col;
            yield return null;
        }

        var c = sr.color;
        c.a = 1f;
        sr.color = c;

        dialogueOpen = true;
        runner.StartDialogue("PipDream");

        yield return new WaitUntil(() => !dialogueOpen);

        while (sr.color.a > 0)
        {
            var col = sr.color;
            col.a -= Time.deltaTime;
            sr.color = col;
            yield return null;
        }

        var c1 = sr.color;
        c1.a = 0f;
        sr.color = c1;

        SceneManager.LoadScene("1_BatonScene");
    }

    public void CloseDialogue()
    {
        dialogueOpen = false;
    }
}
