using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Yarn.Unity;

public class ShootController : MonoBehaviour
{
    public GameObject proj;
    public float noteLifetime = 5.0f;

    private int noteAmount;
    private int notesSpawned;

    private List<GameObject> notes = new List<GameObject>();

    private bool validRun = true;
    private bool startingAbsorb = false;

    private GameObject targetNote;

    public UnityEvent shootEvent;
    public UnityEvent stopEvent;

    private bool started = false;

    public float triggerDistance;

    public Image targetNoteImage;
    public GameObject banner;

    public GameObject[] deathInstruments;
    public float fadeOutSpeed;

    public AudioClip victorySound;
    public int instrumentIndex;

    private void Start()
    {
        if (GameManager.hasDefeated.Contains(gameObject.name)) Destroy(gameObject);
        banner.SetActive(false);

        noteAmount = GetComponent<ViolinEnemy>().noteAmount;
    }

    // Update is called once per frame
    void Update()
    {
        if (!startingAbsorb) {
            if (!started && Vector2.Distance(transform.position, GameObject.Find("Player").transform.position) < triggerDistance)
            {
                banner.SetActive(true);
                shootEvent.Invoke();
                started = true;
            }

            // Check for illegal hits
            for (int i = 0; i < notes.Count; i++) // Wrong note hit
            {
                if (notes[i].GetComponent<NoteManager>().isHit || notes[i].GetComponent<NoteManager>().timeAlive > 5.0f)
                {
                    validRun = false;
                    Destroy(notes[i]);
                    notes.RemoveAt(i);
                    banner.SetActive(false);
                    i--;
                }
            }
            if (targetNote != null && targetNote.GetComponent<NoteManager>().timeAlive > noteLifetime) // Current note alive for too long
            {
                validRun = false;
                Destroy(targetNote);
                banner.SetActive(false);
            }

            targetNoteImage.enabled = validRun;

            // If correct note is hit
            if (targetNote != null && targetNote.GetComponent<NoteManager>().isHit)
            {
                Destroy(targetNote);
                targetNote = null;
                if (notes.Count > 0)
                {
                    targetNote = notes[0];
                    notes.RemoveAt(0);
                    targetNoteImage.sprite = targetNote.GetComponent<SpriteRenderer>().sprite;
                    targetNoteImage.color = targetNote.GetComponent<SpriteRenderer>().color;
                }
            }

            if (notes.Count == 0 && targetNote == null && notesSpawned == noteAmount)
            {
                if (validRun) // If did correctly
                {
                    startingAbsorb = true;
                    banner.SetActive(false);
                    StartCoroutine(Absorb());
                }
                else // Otherwise replay
                {
                    stopEvent.Invoke();
                    validRun = true;
                    targetNote = null;
                    notes.Clear();
                    notesSpawned = 0;
                    started = false;
                }
            }
        }
    }

    public void ShootNote(Vector2 vel, Color col, AudioClip clip)
    {
        GameObject note = Instantiate(proj);
        note.transform.position = transform.position;
        note.SetActive(true);
        note.GetComponent<SpriteRenderer>().color = col;
        note.transform.GetChild(0).GetComponent<Light2D>().color = col;
        note.GetComponent<Rigidbody2D>().velocity = vel;
        note.GetComponent<NoteManager>().goodSound = clip;
        note.GetComponent<NoteManager>().audioSource = GetComponent<AudioSource>();

        if (targetNote == null)
        {
            targetNote = note;
            targetNoteImage.sprite = note.GetComponent<SpriteRenderer>().sprite;
            targetNoteImage.color = col;
        }
        else notes.Add(note);

        notesSpawned++;
    }

    private IEnumerator Absorb()
    {
        banner.SetActive(false);

        GameObject.Find("GameManager").GetComponent<GameManager>().ConductInst();
        GameObject.Find("GameManager").GetComponent<GameManager>().instCaptured[instrumentIndex] = true;
        GameObject.Find("GameManager").GetComponent<GameManager>().PlaySoundNoMusic(victorySound);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        while (sr.color.a > 0f)
        {
            var col = sr.color;
            col.a -= Time.deltaTime * fadeOutSpeed;
            sr.color = col;
            yield return null;
        }

        GameManager.hasDefeated.Add(gameObject.name);

        foreach (GameObject g in deathInstruments)
        {
            GameObject inst = Instantiate(g);
            inst.SetActive(true);
            inst.transform.position = transform.position + Vector3.up * (deathInstruments.Length - 1) * 2;
            yield return new WaitForSeconds(0.5f);
        }

        Destroy(gameObject);
        yield return null;
    }
}
