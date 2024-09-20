using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using Yarn.Unity;
using static UnityEngine.Rendering.DebugUI.Table;

public class PianoTilesGame : MonoBehaviour
{ 
    public string currGame;
    private float currSpeed;
    private List<string> gameTiles;
    public GameObject tilePrefab;
    public GameObject sustainTilePrefab;
    public float[] rowPos;
    public Sprite[] tileSprites;
    public Sprite[] sustainTileSprites;

    public int fullMoonScore;
    private int initFullMoonScore;
    private int tutorialMoonScore;
    public float moonDownPosition; //2.506
    public float moonUpPosition; //5.851
    public float moonSpeed;

    public UnityAction endGameAction;
    private Transform moonCover;

    public AudioClip introMusic;
    public AudioClip scaleMusic;
    public AudioClip sustainMusic;

    public DialogueRunner dialogueRunner;
    private bool dialogueOpen = false;

    private void Start()
    {
        initFullMoonScore = fullMoonScore;
        moonCover = GameObject.Find("MoonCover").transform;
        gameTiles = Games.GetGame(currGame);
        currSpeed = float.Parse(gameTiles[0].Split(" = ")[1]);

        StartCoroutine(RunGame());
    }

    private void Update()
    {
        float newY = moonDownPosition + ((moonUpPosition - moonDownPosition) / fullMoonScore) * ButtonController.score;
        if (moonCover.position.y < newY)
        {
            moonCover.position = new Vector2(moonCover.position.x, Mathf.Min(newY, moonCover.position.y + moonSpeed * Time.deltaTime));
        }
    }

    private void SpawnTile(int row)
    {
        GameObject tile = Instantiate(tilePrefab);
        tile.transform.position = new Vector2(rowPos[row], 6);
        tile.GetComponent<TileController>().speed = currSpeed;
        tile.GetComponent<SpriteRenderer>().sprite = tileSprites[row];
        tile.SetActive(true);
    }

    private IEnumerator RunGame()
    {
        // Run Tutorial
        if(!GameManager.hasDonePianoTutorial) yield return StartCoroutine(Tutorial());

        // Run rest of game
        fullMoonScore = initFullMoonScore;
        ButtonController.score = 0;
        moonCover.position = new Vector2(moonCover.position.x, moonDownPosition);
        StartCoroutine(WaitStartMusic());
        float time = Time.time;
        var gt = gameTiles.Skip(1);
        foreach (string line in gt)
        {
            List<int> rows = new List<int>();
            foreach(string num in line.Split(" - ")[0].Split(", ")) rows.Add(Int32.Parse(num));

            float start_time, end_time;

            if (line.Contains(':'))
            {
                start_time = float.Parse(line.Split(" - ")[1].Split(":")[0]);
                end_time = float.Parse(line.Split(" - ")[1].Split(":")[1]);

                yield return new WaitUntil(new Func<bool>(() => Time.time - time > start_time));

                foreach (int row in rows)
                {
                    GameObject tile = Instantiate(sustainTilePrefab);
                    SustainTileController controller = tile.GetComponent<SustainTileController>();
                    SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
                    Vector2 initialSize = sr.size;
                    sr.sprite = sustainTileSprites[row];
                    //sr.size = new Vector2(initialSize.x, initialSize.y * (end_time - start_time) * currSpeed);
                    sr.size = new Vector2(initialSize.x, initialSize.y + (end_time - start_time) * currSpeed);
                    controller.bottomCircleCenterOffset = (sr.size.y - initialSize.y) / 2f;
                    tile.transform.position = new Vector2(rowPos[row], 6 + controller.bottomCircleCenterOffset);
                    controller.speed = currSpeed;
                    tile.SetActive(true);
                }
            }
            else
            {
                start_time = float.Parse(line.Split(" - ")[1]);

                yield return new WaitUntil(new Func<bool>(() => Time.time - time > start_time));

                foreach (int row in rows)
                {
                    SpawnTile(row);
                }
            }
        }

        yield return new WaitForSeconds(4);

        if(ButtonController.score > fullMoonScore)
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
        

        endGameAction.DynamicInvoke();
    }

    private IEnumerator WaitStartMusic()
    {
        yield return new WaitForSeconds(9.6564f / currSpeed + 0.5f + 1f/currSpeed); // Going to kill myself
        GameObject.Find("Main Camera").GetComponent<AudioSource>().PlayOneShot(introMusic, 0.25f);
    }

    private IEnumerator Tutorial()
    {
        dialogueOpen = true;
        dialogueRunner.StartDialogue("Intro");

        yield return new WaitUntil(() => !dialogueOpen);

        fullMoonScore = 250;
        yield return StartCoroutine(TutorialScale());

        dialogueOpen = true;
        dialogueRunner.StartDialogue("SustainTile");

        yield return new WaitUntil(() => !dialogueOpen);

        fullMoonScore = 500;
        yield return StartCoroutine(TutorialSustainTile());

        dialogueOpen = true;
        dialogueRunner.StartDialogue("GoodJob");

        yield return new WaitUntil(() => !dialogueOpen);

        GameManager.hasDonePianoTutorial = true;
    }

    private IEnumerator PlayScaleMusic()
    {
        yield return new WaitForSeconds(9.6564f / currSpeed); // Going to kill myself
        GameObject.Find("Main Camera").GetComponent<AudioSource>().PlayOneShot(scaleMusic, 0.25f);
    }

    private IEnumerator PlaySustainMusic()
    {
        yield return new WaitForSeconds(9.6564f / currSpeed);
        GameObject.Find("Main Camera").GetComponent<AudioSource>().PlayOneShot(sustainMusic, 0.25f);
    }

    private IEnumerator TutorialScale()
    {
        ButtonController.score = 0;
        moonCover.position = new Vector2(moonCover.position.x, moonDownPosition);
        // Start music
        StartCoroutine(PlayScaleMusic());

        // Spawn tiles

        for (int i = 0; i <= 4; i++)
        {
            SpawnTile(i);
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Tile").Count() == 0);
        yield return new WaitForSeconds(1f);

        if (ButtonController.score < fullMoonScore)
        {
            print(ButtonController.score);
            dialogueOpen = true;
            dialogueRunner.StartDialogue("FailScale");

            yield return new WaitUntil(() => !dialogueOpen);
            yield return StartCoroutine(TutorialScale());
        }
    }

    private IEnumerator TutorialSustainTile()
    {
        ButtonController.score = 0;
        moonCover.position = new Vector2(moonCover.position.x, moonDownPosition);

        StartCoroutine(PlaySustainMusic());

        GameObject tile = Instantiate(sustainTilePrefab);
        SustainTileController controller = tile.GetComponent<SustainTileController>();
        SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
        Vector2 initialSize = sr.size;
        sr.sprite = sustainTileSprites[2];
        //sr.size = new Vector2(initialSize.x, initialSize.y * (end_time - start_time) * currSpeed);
        sr.size = new Vector2(initialSize.x, initialSize.y + (2) * currSpeed);
        controller.bottomCircleCenterOffset = (sr.size.y - initialSize.y) / 2f;
        tile.transform.position = new Vector2(rowPos[2], 6 + controller.bottomCircleCenterOffset);
        controller.speed = currSpeed;
        tile.SetActive(true);

        yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("SustainTile").Count() == 0);
        yield return new WaitForSeconds(1f);

        if (ButtonController.score < fullMoonScore)
        {
            dialogueOpen = true;
            dialogueRunner.StartDialogue("FailSustain");

            yield return new WaitUntil(() => !dialogueOpen);
            yield return StartCoroutine(TutorialSustainTile());
        }
    }

    public void CloseDialogue()
    {
        dialogueOpen = false;
    }
}

class Games
{
    public static List<string> GetGame(string gameName)
    {
        switch(gameName)
        {
            case "EasyGame":
                return EasyGame;
            case "EasySustainGame":
                return EasySustainGame;
            case "TestGame":
                return TestGame;
            case "SustainTestGame":
                return SustainTestGame;
            case "IntroGame":
                return IntroGame;
            default:
                return new List<string>();
        }
    }

    /* GAME FORMAT
     * Speed = t
     * n1, n2, nn - time
     * n1, n2, nn - start_time:stop_time
     * ...
     * ...
     */

    private static readonly List<string> IntroGame = new List<string>()
    {
        "Speed = 6",
        "1 - 8.0",
        "3 - 8.5",
        "4 - 9.0",
        "2 - 10.0",
        "0 - 11.0",
        "1 - 12.0",
        "2 - 12.5",
        "3 - 13.0",
        "2 - 14.0",
        "4 - 14.5",
        "3 - 15.0:16.5", // C measure 5
        "4 - 17.0",
        "3 - 17.5",
        "2 - 17.75",
        "1 - 18.0",
        "0 - 18.5",
        "1 - 19.0",
        "0 - 20.0", // A Natural
        "2 - 21.0",
        "3 - 21.5",
        "4 - 21.75",
        "3 - 22.0",
        "0 - 22.5",
        "3 - 22.75",
        "3 - 23.0",
        "2 - 23.5",
        "0 - 25.0", // Measure 10 (After 2nd quarter rest)
        "1 - 26.0",
        "2 - 27.0",
        "3 - 28.5",
        "4 - 28.875",
        "4 - 30.0",
        "3 - 30.5",
        "4 - 31.0:33.0", // C Whole note
        "3 - 33.5",
        "1 - 34.0",
        "2 - 34.5",
        "0 - 35.0",
        "2 - 35.5",
        "1 - 36.0",
        "0 - 36.5",
        "1 - 37.0",
        "2 - 38.0",
        "4 - 39.0", // Start of 16th note measure
        "3 - 39.25",
        "2 - 39.5",
        "1 - 39.75",
        "2 - 40.0",
        "3 - 41.5",
        "1 - 42.0",
        "2 - 42.5",
        "0 - 43.0:45.0", // Low C Whole note
        "1 - 45.5",
        "2 - 46.0",
        "3 - 46.5",
        "4 - 47.0:49.0", // Mid C Whole note
        "1 - 49.5",
        "0 - 50.0",
        "2 - 50.5",
        "3 - 51.0",
        "1 - 52.5",
        "2 - 53.0",
        "3 - 54.0",
        "4 - 55.0:57.0", // Highest G Whole note
        "3 - 57.0",
        "1 - 58.0",
        "0 - 59.0",
        "2 - 60.0",
        "3 - 60.5",
        "4 - 61.0",
        "2 - 62.0",
        "4 - 62.5",
        "3 - 63.0", // Dotted High C
        "4 - 65.0",
        "3 - 65.5",
        "2 - 65.75",
        "3 - 66.0",
        "2 - 66.5",
        "1 - 67.0",
        "0 - 68.0",
        "1 - 69.0", // Start of repetition
        "2 - 69.5",
        "3 - 69.75",
        "2 - 70.0",
        "0 - 70.5",
        "2 - 70.75",
        "1 - 71.0",
        "2 - 71.5",
        "3 - 71.75",
        "2 - 72.0",
        "0 - 72.5",
        "2 - 72.75",
        "1 - 73.0",
        "2 - 73.5",
        "3 - 73.75",
        "2 - 74.0",
        "0 - 74.5",
        "2 - 74.75",
        "2 - 75.0",
        "1 - 75.5",
        "0 - 75.75",
        "1 - 76.0:77.0"
    };

    private static readonly List<string> EasyGame = new List<string>() {
        "Speed = 10",
        "0 - 0",
        "1 - 0.25",
        "2 - 0.5",
        "3 - 0.75",
        "4 - 1",
        "3 - 1.25",
        "2 - 1.5",
        "1 - 1.75",
        "0 - 2",
        "1 - 2.25",
        "2 - 2.5",
        "3 - 2.75",
        "4 - 3",
        "3 - 3.25",
        "2 - 3.5",
        "1 - 3.75",
        "0 - 4"
    };

    private static readonly List<string> EasySustainGame = new List<string>() {
        "Speed = 10",
        "0 - 0:1",
        "1 - 0.25",
        "2 - 0.5",
        "3 - 0.75",
        "4 - 1:2",
        "3 - 1.25",
        "2 - 1.5",
        "1 - 1.75",
        "0 - 2:3",
        "1 - 2.25",
        "2 - 2.5",
        "3 - 2.75",
        "4 - 3",
        "3 - 3.25",
        "2 - 3.5",
        "1 - 3.75:4.75",
        "0 - 4"
    };

    private static readonly List<string> TestGame = new List<string> {
        "Speed = 5",
        "2 - 0:2",
        "3 - 2",
        "1 - 2.25",
    };

    private static readonly List<string> SustainTestGame = new List<string> {
        "Speed = 5",
        "0 - 0:1",
        "1 - 1:2",
        "2 - 2:3",
        "3 - 3:4",
        "4 - 4:5",
    };
}