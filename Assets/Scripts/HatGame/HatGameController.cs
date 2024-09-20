using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class HatGameController : MonoBehaviour
{
    public string currGame;
    public static int score;

    private bool dialogueOpen = false;
    public DialogueRunner dialogueRunner;

    public List<GameObject> hatPrefabs;
    //public GameObject notePrefab;
    public GameObject quarterNotePrefab;
    public GameObject eighthNotePrefab;
    public GameObject doubleEighthNotePrefab;
    public GameObject barPrefab;
    public GameObject restPrefab;
    public GameObject eighthRestPrefab;
    public AudioClip metronomeClick;
    public AudioClip smartMetronome;
    public UnityAction endAction;

    public float TIME;
    public float START_TIME;
    public float INC_TIME = 0;


    private static float BPM = 120;
    private static float SPB = 60f / BPM; //seconds per beat

    private static float headX = -2.7f;
    private static float hatStartDX = 2.9f;
    private static float hatStartY = 2f;
    private static float hatVelocity = 3;

    //private static float noteFirstX = 2.433f;
    //private static float noteDX = 3.265f - noteFirstX;
    private static float noteFirstX = 2.156f;
    private static float noteDX = (5.514f - noteFirstX)/3.5f;

    private static float noteY = 0.7f;
    private static float quarterNoteY = 0.765f;
    private static float eighthNoteY = 0.765f;
    private static float doubleEighthNoteY = 0.765f;
    private static float restY = 0.488f;
    private static float eighthRestY = 0.411f;

    private static float quarterNoteDX = 5.021f - 4.929f;
    private static float eighthNoteDX = 5.17f - 4.929f;
    private static float doubleEighthNoteDX = 5.298f - 4.929f + 0.03f;
    private static float restDX = 5.046f - 4.929f;
    private static float eighthRestDX = 5.043f - 4.929f;

    //private float barStartX = 2.12f;
    private static float barStartX = noteFirstX - noteDX;
    private static float barStartY = 0.76f;
    //private static float barEndX = 5.619f;
    private static float barEndX = noteFirstX + 3*noteDX;
    private static float barVelocity = noteDX / SPB;

    private static float waitTime = 1 * (4 * SPB);

    private Queue<GameObject> notes = new Queue<GameObject>();
    private Queue<GameObject> hats = new Queue<GameObject>();
    private Queue<GameObject> hatsToDrop = new Queue<GameObject>();

    private int sortingOrder = 10;

    public int fullMoonScore;
    private int initFullMoonScore;
    private int tutorialMoonScore;
    public float moonDownPosition; //2.506
    public float moonUpPosition; //5.851
    public float moonSpeed;

    private Transform moonCover;

    // Start is called before the first frame update
    void Start()
    {
        START_TIME = Time.time;
        initFullMoonScore = fullMoonScore;
        moonCover = GameObject.Find("HatCover").transform;

        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        if(!GameManager.hasDoneHatTutorial)
        {
            yield return StartCoroutine(Tutorial());
            GameManager.hasDoneHatTutorial = true;
        }

        // Start Real Game
        dialogueOpen = true;
        dialogueRunner.StartDialogue("GetReady");

        yield return new WaitUntil(() => !dialogueOpen);

        INC_TIME = 0;
        START_TIME = Time.time;
        score = 0;
        moonCover.position = new Vector2(moonCover.position.x, moonDownPosition);
        fullMoonScore = initFullMoonScore;

        List<List<bool>> game = HatGames.GetGame(currGame);
        StartCoroutine(RunGame(game));
        StartCoroutine(HatThread(game));
        StartCoroutine(Metronome(game));
        StartCoroutine(SmartMetronome(game));
        StartCoroutine(WaitStartMusic());
    }

    private IEnumerator WaitStartMusic()
    {
        yield return new WaitForSecondsRealtime(0.25f);
        GetComponent<AudioSource>().Play();
    }

    private IEnumerator Tutorial()
    {
        dialogueOpen = true;
        dialogueRunner.StartDialogue("StartGame");

        yield return new WaitUntil(() => !dialogueOpen);

        fullMoonScore = 5;
        yield return StartCoroutine(IntroTutorial());

        dialogueOpen = true;
        dialogueRunner.StartDialogue("EightTutorial");

        yield return new WaitUntil(() => !dialogueOpen);

        fullMoonScore = 10;
        yield return StartCoroutine(EigthTutorial());

        dialogueOpen = true;
        dialogueRunner.StartDialogue("CongratulateTutorial");

        yield return new WaitUntil(() => !dialogueOpen);
    }

    private IEnumerator IntroTutorial()
    {
        score = 0;
        moonCover.position = new Vector2(moonCover.position.x, moonDownPosition);
        INC_TIME = 0;
        START_TIME = Time.time;
        List<List<bool>> tutorial = HatGames.GetGame("Tutorial");
        StartCoroutine(HatThread(tutorial));
        StartCoroutine(Metronome(tutorial));
        StartCoroutine(SmartMetronome(tutorial));
        yield return StartCoroutine(RunGame(tutorial)); // Wait for game to finish

        if(score < 5)
        {
            dialogueOpen = true;
            dialogueRunner.StartDialogue("RetryTutorial");

            yield return new WaitUntil(() => !dialogueOpen);

            // Restart
            yield return StartCoroutine(IntroTutorial());
        }
    }

    private IEnumerator EigthTutorial()
    {
        score = 0;
        moonCover.position = new Vector2(moonCover.position.x, moonDownPosition);
        INC_TIME = 0;
        START_TIME = Time.time;
        List<List<bool>> tutorial = HatGames.GetGame("EigthTutorial");
        StartCoroutine(HatThread(tutorial));
        StartCoroutine(Metronome(tutorial));
        StartCoroutine(SmartMetronome(tutorial));
        yield return StartCoroutine(RunGame(tutorial));

        if(score < 10)
        {
            dialogueOpen = true;
            dialogueRunner.StartDialogue("RetryEightTutorial");

            yield return new WaitUntil(() => !dialogueOpen);
            // Restart
            yield return StartCoroutine(EigthTutorial());
        }
    }

    // Update is called once per frame
    void Update()
    {
        TIME = Time.time - START_TIME;
        INC_TIME += Time.deltaTime;

        if (Input.GetButtonDown("Jump"))
        {
            if (hatsToDrop.Count > 0)
            {
                GameObject hat = hatsToDrop.Dequeue();
                hat.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -0.001f);
                hat.GetComponent<Rigidbody2D>().gravityScale = 10;
                hat.GetComponent<PolygonCollider2D>().enabled = true;
            }

        }

        float newY = moonDownPosition + ((moonUpPosition - moonDownPosition) / fullMoonScore) * score;
        if (moonCover.position.y < newY)
        {
            moonCover.position = new Vector2(moonCover.position.x, Mathf.Min(newY, moonCover.position.y + moonSpeed * Time.deltaTime));
        }
    }

    private IEnumerator Metronome(List<List<bool>> measures)
    {
        for (int i = 1; i <= 2 * measures.Count * 4; i++)
        {
            yield return new WaitUntil(new Func<bool>(() => INC_TIME >= SPB * i));
            GameObject.Find("Main Camera").GetComponent<AudioSource>().PlayOneShot(metronomeClick, 1f);
        }
    }

    private IEnumerator SmartMetronome(List<List<bool>> measures)
    {
        int n = 0;
        foreach (List<bool> measure in measures)
        {
            float measureStart = n * 2 * 4 * SPB;
            int i = 2;
            foreach (bool note in measure)
            {
                if (note)
                {
                    yield return new WaitUntil(() => INC_TIME >= measureStart + (0 * 4 * SPB) + (i * SPB / 2f) - 0.05f);
                    GameObject.Find("Main Camera").GetComponent<AudioSource>().PlayOneShot(smartMetronome, 1f);
                }
                i++;
            }
            n++;
        }
    }

        private IEnumerator HatThread(List<List<bool>> measures)
    {
        int n = 0;
        foreach (List<bool> measure in measures)
        {
            float measureStart = n * 2 * 4 * SPB;
            
            //make hats
            float hatStartTime = Time.time;
            int side = -1;
            int i = 2;
            foreach (bool note in measure)
            {
                if (note)
                {
                    //yield return new WaitUntil(new Func<bool>(() => Time.time - hatStartTime > (
                    //            waitTime + SPB + (i * SPB) - (hatStartDX / hatVelocity)
                    //       )));
                    yield return new WaitUntil(new Func<bool>(() => INC_TIME > (
                                measureStart + (1 * 4 * SPB) + (i * SPB / 2f) - (hatStartDX / hatVelocity)
                           )));

                    GameObject hat = Instantiate(hatPrefabs[UnityEngine.Random.Range(0, hatPrefabs.Count)]);
                    hat.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder++;
                    hat.transform.position = new Vector2(headX + (side * hatStartDX), hatStartY);
                    hat.GetComponent<Rigidbody2D>().velocity = Vector2.right * side * -1 * hatVelocity;
                    hat.GetComponent<HatController>().landedCallback = () => { score++; };
                    side *= -1;

                    hats.Enqueue(hat);
                    hatsToDrop.Enqueue(hat);
                }
                i++;
            }

            //wait
            //float waitStartTime = Time.time;
            //yield return new WaitUntil(new Func<bool>(() => Time.time - waitStartTime > waitTime + (hatStartDX / hatVelocity) + SPB));
            //yield return new WaitUntil(new Func<bool>(() => INC_TIME > measureStart + 4 * 4 * SPB));
            yield return new WaitUntil(new Func<bool>(() => INC_TIME > (
                    ((n+1) * 2 * 4 * SPB) + (1 * 4 * SPB) + (2 * SPB / 2f) - (hatStartDX / hatVelocity)
            )));
            //destroy hats
            while (hats.Count > 0) Destroy(hats.Dequeue());
            hatsToDrop = new Queue<GameObject>();
            hats = new Queue<GameObject>();

            n++;
        }
    }

    private IEnumerator RunGame(List<List<bool>> measures)
    {
        int n = 0;
        foreach (List<bool> measure in measures)
        {
            float measureStart = n * 2 * 4 * SPB;
            //make notes
            int i = 0;
            foreach (bool b in measure)
            {
                if (b)
                {
                    if (i % 2 == 0)
                    {
                        if (measure[i + 1])
                        {
                            GameObject note = Instantiate(doubleEighthNotePrefab);
                            note.transform.position = new Vector2(noteFirstX + i * noteDX / 2f + doubleEighthNoteDX, doubleEighthNoteY);
                            notes.Enqueue(note); 
                        }
                        else
                        {
                            GameObject note = Instantiate(quarterNotePrefab);
                            note.transform.position = new Vector2(noteFirstX + i * noteDX / 2f + quarterNoteDX, quarterNoteY);
                            notes.Enqueue(note);
                        }
                    }
                    else
                    {
                        if (!measure[i - 1])
                        {
                            GameObject note = Instantiate(eighthNotePrefab);
                            note.transform.position = new Vector2(noteFirstX + i * noteDX / 2f + eighthNoteDX, eighthNoteY);
                            notes.Enqueue(note);
                        }
                    }
                }
                else
                {
                    if (i % 2 == 0)
                    {
                        if (measure[i + 1])
                        {
                            GameObject rest = Instantiate(eighthRestPrefab);
                            rest.transform.position = new Vector2(noteFirstX + i * noteDX / 2f + eighthRestDX, eighthRestY);
                            notes.Enqueue(rest);
                        }
                        else
                        {
                            GameObject rest = Instantiate(restPrefab);
                            rest.transform.position = new Vector2(noteFirstX + i * noteDX / 2f + restDX, restY);
                            notes.Enqueue(rest);
                        }
                    }
                }
                i++;
            }

            //make bar
            GameObject bar = Instantiate(barPrefab);
            bar.transform.position = new Vector2(barStartX, barStartY);

            //move bar
            bar.GetComponent<Rigidbody2D>().velocity = new Vector2(barVelocity, 0);
            //float barStartTime = Time.time;
            //yield return new WaitUntil(new Func<bool>(() => bar.transform.position.x >= barEndX));
            yield return new WaitUntil(new Func<bool>(() => INC_TIME >= measureStart + 4 * SPB));

            Destroy(bar);
            //bar.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

            //wait
            //float waitStartTime = Time.time;
            //yield return new WaitUntil(new Func<bool>(() => Time.time - waitStartTime > waitTime + (hatStartDX / hatVelocity) + SPB));
            yield return new WaitUntil(new Func<bool>(() => INC_TIME > measureStart + 2 * 4 * SPB));

            //destroy notes
            while (notes.Count > 0) Destroy(notes.Dequeue());
            notes = new Queue<GameObject>();

            n++;
        }

        if (measures.Count > 2)
        {
            yield return new WaitForSeconds(5f);

            if (score > 50)
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
        else yield return new WaitForSeconds(1f);
        
    }

    public void CloseDialogue()
    {
        dialogueOpen = false;
    }
}

class HatGames
{
    public static List<List<bool>> GetGame(string gameName)
    {
        switch(gameName)
        {
            case "DashGame": return ParseGame(DashGame);
            case "Tutorial": return ParseGame(Tutorial);
            case "EigthTutorial": return ParseGame(EigthTutorial);
            default: return new List<List<bool>>();
        }
    }

    private static List<List<bool>> ParseGame(string game)
    {
        List<List<bool>> measures = new List<List<bool>>();
        foreach(string measure in game.Split(" "))
        {
            List<bool> beats = new List<bool>();
            for (int i = 0; i < measure.Length; i++)
            {
                if (measure[i].Equals('.')) 
                     beats.Add(true);
                else beats.Add(false);

            }
            measures.Add(beats);
        }
        return measures;
    }

    private static readonly string DashGame = "..__..__ ._._._._ ._._..._ ..._..._ .__...._ .__..... ._.._.._ .._.._.. ._.._.._ _..__._. ._._.... .__.__._ ......._ ...__...";
    private static readonly string Tutorial = ".___.___ ._._._._";
    private static readonly string EigthTutorial = "..__..__ ........";
}