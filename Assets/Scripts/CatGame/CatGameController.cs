using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class CatGameController : MonoBehaviour
{
    public string currGame;
    public List<GameObject> catPrefabs;
    public float maxCatSpeed;
    public float maxRotationSpeed;

    private bool dialogueOpen = false;
    public DialogueRunner dialogueRunner;

    public UnityAction endAction;

    private AudioSource music;

    public static int score = 0;
    public float goalScore = 4;

    public TMP_Text scoreText;

    private void Start()
    {
        music = GetComponent<AudioSource>();
        music.Pause();
        StartCoroutine(RunGame(CatGames.GetGame(currGame)));
    }

    private IEnumerator RunGame(List<float> times)
    {
        if (!GameManager.hasDoneCatTutorial) yield return StartCoroutine(Tutorial());

        dialogueOpen = true;
        dialogueRunner.StartDialogue("GetReady");

        yield return new WaitUntil(() => !dialogueOpen);

        music.Play();
        float startTime = Time.time;
        score = 0;
        goalScore = 30;

        foreach (float time in times)
        {
            yield return new WaitUntil(new System.Func<bool>(() => Time.time - startTime > time - 1f));

            GameObject cat = Instantiate(catPrefabs[Random.Range(0, catPrefabs.Count)]);
            cat.SetActive(true);
            cat.transform.position = new Vector2(Random.Range(-5f, 5f), 5.2f);
            cat.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-maxRotationSpeed, maxRotationSpeed);
            cat.GetComponent<Rigidbody2D>().velocity = Vector2.right * Random.Range(-maxCatSpeed, maxCatSpeed);
        }

        yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Cat").Length == 0); // Wait until all cats are gone

        yield return new WaitForSeconds(1.5f);

        if (score > 30)
        {
            dialogueOpen = true;
            dialogueRunner.StartDialogue("EndGood");

            yield return new WaitUntil(() => !dialogueOpen);
        }
        else
        {
            dialogueOpen = true;
            dialogueRunner.StartDialogue("EndBad");

            yield return new WaitUntil(() => !dialogueOpen);
        }

        endAction.Invoke();
    }

    private void Update()
    {
        scoreText.text = $"{score} / {goalScore}";
    }

    private IEnumerator Tutorial()
    {
        GameManager.hasDoneCatTutorial = true;

        dialogueOpen = true;
        dialogueRunner.StartDialogue("Introduce");

        yield return new WaitUntil(() => !dialogueOpen);

        yield return StartCoroutine(CatTutorial());

        dialogueOpen = true;
        dialogueRunner.StartDialogue("Congrats");

        yield return new WaitUntil(() => !dialogueOpen);
    }

    private IEnumerator CatTutorial()
    {
        score = 0;

        float startTime = Time.time;

        foreach (float time in CatGames.GetGame("Tutorial"))
        {
            yield return new WaitUntil(new System.Func<bool>(() => Time.time - startTime > time));

            GameObject cat = Instantiate(catPrefabs[Random.Range(0, catPrefabs.Count)]);
            cat.SetActive(true);
            cat.transform.position = new Vector2(Random.Range(-5f, 5f), 6.15f);
            cat.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-maxRotationSpeed, maxRotationSpeed);
            cat.GetComponent<Rigidbody2D>().velocity = Vector2.right * Random.Range(-maxCatSpeed, maxCatSpeed);
        }

        yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Cat").Length == 0); // Wait until all cats are gone

        yield return new WaitForSeconds(1.5f);

        if(score < 4)
        {
            dialogueOpen = true;
            dialogueRunner.StartDialogue("Disappointment");

            yield return new WaitUntil(() => !dialogueOpen);

            yield return StartCoroutine(CatTutorial());
        }
    }

    public void CloseDialogue()
    {
        dialogueOpen = false;
    }
}

class CatGames
{
    public static List<float> GetGame(string gameName)
    {
        switch (gameName)
        {
            case "Game": return ParseGame(Game);
            case "Tutorial": return ParseGame(Tutorial);
            default: return new List<float>();
        }
    }

    private static List<float> ParseGame(string game)
    {
        List<float> l = new List<float>();
        foreach (string t in game.Split(" "))
        {
            l.Add(float.Parse(t));
        }
        return l;
    }

    private static readonly string Game = "10.0 10.634 11.268 11.901 12.535 13.38 13.803 14.225 14.648 15.07 16.338 16.972 17.606 18.873 19.296 19.718 20.141 21.408 22.254 22.465 22.676 23.521 23.732 23.944 24.014 24.085 24.155 24.789 25.0 25.211 26.479 27.324 27.535 27.746 28.592 28.803 29.014 29.648 30.282 41.69 42.113 42.324 42.535 42.746 42.958 43.38 43.803 44.648 44.859 45.07 45.282 45.493 45.915 46.338 46.761 47.183 47.394 47.606 47.817 48.028 48.451 48.873 49.296 49.93 50.563";
    private static readonly string Tutorial = "0 1.0 2.0 3.0 4.0";
}