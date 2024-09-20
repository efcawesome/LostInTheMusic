using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Yarn.Unity;

public class BatonPickup : MonoBehaviour
{
    private PlayerMovement pm;
    private PlayerAttack pa;

    public GameObject spirit;
    public DialogueRunner dialogueRunner;
    private bool dialogueOpen = false;

    private bool isRunning = false;

    private Dictionary<GameObject, Coroutine> hoverCors = new Dictionary<GameObject, Coroutine>();

    public TMPro.TMP_Text txt;
    public float hoverDist;
    public float masterSpeed;

    // Start is called before the first frame update
    void Start()
    {
        spirit.SetActive(false);
        pm = GameObject.Find("Player").GetComponent<PlayerMovement>();
        pa = GameObject.Find("Player").GetComponent<PlayerAttack>();
    }

    public void OnOpen()
    {
        if(!isRunning) StartCoroutine(PickUp());
        
    }

    private IEnumerator PickUp()
    {

        isRunning = true;
        GetComponent<SpriteRenderer>().enabled = false;
        StartCoroutine(GameManager.FadeTextOut(GetComponent<InteractableObject>().openPrompt, 5f));
        GetComponent<Light2D>().enabled = false;

        GameObject vcam = GameObject.Find("Virtual Camera");
        vcam.SetActive(false);
        GameObject cam = GameObject.Find("Main Camera");
        Vector3 init_cam_pos = cam.transform.position;
        Vector3 cam_pos = new Vector3(27.91f, -26.12f, -10.77f);
        float t = 0;
        while(Vector3.Distance(cam.transform.position, cam_pos) > 0.005f)
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, cam_pos, t*0.45f);
            t += Time.deltaTime;
            yield return null;
        }

        cam.transform.position = cam_pos;


        // Cutscene
        pm.RevokeControl();
        spirit.SetActive(true);
        spirit.GetComponent<Animator>().Play("SpiritOfTheOrchestraSpawn");

        yield return new WaitForSeconds(16f / 15f);

        dialogueOpen = true;
        dialogueRunner.StartDialogue("HowDareYou");

        yield return new WaitUntil(() => !dialogueOpen);

        List<GameObject> instruments = new List<GameObject>(GameObject.FindGameObjectsWithTag("Instrument"));

        while(instruments.Count > 0)
        {
            int currInd = Random.Range(0, instruments.Count);

            StartCoroutine(FloatInstrument(instruments[currInd]));

            yield return new WaitForSeconds(Random.Range(0.75f, 1.25f));

            instruments.RemoveAt(currInd);
        }

        dialogueOpen = true;
        dialogueRunner.StartDialogue("Spawn");
        yield return new WaitUntil(() => !dialogueOpen);

        // Spirits appear
        instruments = new List<GameObject>(GameObject.FindGameObjectsWithTag("Instrument"));

        while (instruments.Count > 0)
        {
            int currInd = Random.Range(0, instruments.Count);

            StartCoroutine(SpiritAppear(instruments[currInd]));

            instruments.RemoveAt(currInd);
        }

        yield return new WaitForSeconds(2f);

        dialogueOpen = true;
        dialogueRunner.StartDialogue("Fly");
        yield return new WaitUntil(() => !dialogueOpen);

        instruments = new List<GameObject>(GameObject.FindGameObjectsWithTag("Instrument"));

        while(instruments.Count > 0)
        {
            int currInd = Random.Range(0, instruments.Count);

            StartCoroutine(SendInstrumentFlying(instruments[currInd]));

            instruments.RemoveAt(currInd);
        }

        yield return new WaitForSeconds(3f);

        dialogueOpen = true;
        dialogueRunner.StartDialogue("PostSpirits");

        yield return new WaitUntil(() => !dialogueOpen);

        SpriteRenderer spiritSR = spirit.GetComponent<SpriteRenderer>();
        while(spiritSR.color.a > 0)
        {
            var col = spiritSR.color;
            col.a -= Time.deltaTime * 0.5f;
            spiritSR.color = col;
            yield return null;
        }

        Destroy(spirit);

        t = 0;
        while (Vector3.Distance(cam.transform.position, init_cam_pos) > 0.005f)
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, init_cam_pos, t * 0.45f);
            t += Time.deltaTime;
            yield return null;
        }
        vcam.SetActive(true);

        pm.GiveControl();
        pa.hasBaton = true;
        pa.canAttack = true;
        // End Cutscene

        yield return StartCoroutine(GameManager.FadeTextOut(gameObject.GetComponent<InteractableObject>().openPrompt, gameObject.GetComponent<InteractableObject>().fadeOutSpeed));

        Destroy(gameObject);
    }

    private IEnumerator FloatInstrument(GameObject instrument)
    {
        float initY = instrument.transform.position.y;
        Vector2 goalVec = new Vector2(instrument.transform.position.x, initY + hoverDist + Random.Range(-hoverDist / 8, hoverDist / 8));

        float t = 0;

        float speed = masterSpeed + Random.Range(-masterSpeed/4, masterSpeed/4);

        while(instrument.transform.position.y < goalVec.y - 0.025f)
        {
            instrument.transform.position = Vector2.Lerp(instrument.transform.position, goalVec, t*speed);
            t += Time.deltaTime;
            yield return null;
        }

        hoverCors[instrument] = StartCoroutine(HoverInstrument(instrument));
    }

    private IEnumerator HoverInstrument(GameObject instrument)
    {
        float init_y = instrument.transform.position.y;

        float speed = masterSpeed + Random.Range(-masterSpeed / 4, masterSpeed / 4);
        float amp = (Random.Range(0, 2) == 0 ? 1 : -1) * Random.Range(0.2f, 0.5f);
        float t = 0;

        while (true)
        {
            var pos = instrument.transform.position;
            pos.y = init_y + amp * Mathf.Sin(t * speed);
            instrument.transform.position = pos;

            t += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator SendInstrumentFlying(GameObject instrument)
    {
        StopCoroutine(hoverCors[instrument]);

        Vector2 dir = new Vector2((Random.Range(0,2) == 0 ? 1 : -1) * Random.Range(0.05f, 0.1f) * Time.deltaTime * 60f, (Random.Range(0, 2) == 0 ? 1 : -1) * Random.Range(0.05f, 0.1f) * Time.deltaTime * 60f);
        float t = 0;

        while(t < 2)
        {
            instrument.transform.position += (Vector3)(dir);
            t += Time.deltaTime;
            yield return null;
        }

        Destroy(instrument);
    }

    private IEnumerator SpiritAppear(GameObject instrument)
    {
        SpriteRenderer sr = instrument.GetComponent<SpriteRenderer>();
        SpriteRenderer childSR = instrument.transform.childCount > 0 ? instrument.transform.GetChild(0).GetComponent<SpriteRenderer>() : null;

        while(sr.color.a > 0)
        {
            var col = sr.color;
            col.a -= Time.deltaTime;
            sr.color = col;
            
            if(childSR != null)
            {
                var col2 = childSR.color;
                col2.a += Time.deltaTime;
                childSR.color = col2;
            }

            yield return null;
        }

        sr.color = Color.clear;
        if (childSR != null) childSR.color = Color.white;
    }

    public void CloseDialogue()
    {
        dialogueOpen = false;
    }
}
