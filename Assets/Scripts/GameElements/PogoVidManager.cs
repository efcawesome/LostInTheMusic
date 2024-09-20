using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PogoVidManager : MonoBehaviour
{
    private PlayerMovement pm;
    private bool running;
    public VideoPlayer vp;
    public GameObject rI;
    public float waitTime = 0.0f;

    private void Start()
    {
        vp.Pause();
        rI.SetActive(false);
        pm = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    public void RunVid()
    {
        if(!running) StartCoroutine(RunVidCo());
    }

    private IEnumerator RunVidCo()
    {
        running = true;
        pm.RevokeControl();
        rI.SetActive(true);

        yield return new WaitForSecondsRealtime(waitTime);

        vp.Prepare();
        yield return new WaitUntil(() => vp.isPrepared);

        while (!vp.isPlaying)
        {
            vp.Play();
            yield return null;
        }
        
        yield return new WaitForSeconds((float)vp.length);
        vp.Pause();
        rI.SetActive(false);
        pm.GiveControl();
        running = false;
    }
}
