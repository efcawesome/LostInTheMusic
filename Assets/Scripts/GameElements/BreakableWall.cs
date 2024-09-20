using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BreakableWall : MonoBehaviour
{
    public int totalHits;
    public PolygonCollider2D newCamCollider;
    public PolygonCollider2D singleCamCollider;
    private int hitsLeft;
    public Sprite[] breakingSprites;
    public AudioClip hitClip;
    public AudioClip breakingClip;

    public bool isLeft;

    private void Awake()
    {
        hitsLeft = totalHits;
    }

    public void Hit()
    {
        hitsLeft--;

        if (hitsLeft == 0) BreakWall(true);
        else
        {
            GetComponent<SpriteRenderer>().sprite = breakingSprites[breakingSprites.Length - hitsLeft];
            GameObject.Find("Main Camera").GetComponent<AudioSource>().PlayOneShot(hitClip);
        }
    }

    public void BreakWall(bool playSound)
    {
        if (playSound) GameObject.Find("Main Camera").GetComponent<AudioSource>().PlayOneShot(breakingClip);

        if(singleCamCollider != null)
        {
            if (GameObject.FindGameObjectsWithTag("BreakableWall").Count() > 1) { GameObject.Find("Virtual Camera").GetComponent<Cinemachine.CinemachineConfiner2D>().m_BoundingShape2D = singleCamCollider; print("Other wall"); }
            else { GameObject.Find("Virtual Camera").GetComponent<Cinemachine.CinemachineConfiner2D>().m_BoundingShape2D = newCamCollider; print("No othre wall"); }
        }
        else
        {
            GameObject.Find("Virtual Camera").GetComponent<Cinemachine.CinemachineConfiner2D>().m_BoundingShape2D = newCamCollider;
        }

        //if(!GameManager.wallsBroken.Contains(name)) GameManager.wallsBroken.Add(name);

        if (isLeft) GameManager.leftWallBroken = true;
        else GameManager.rightWallBroken = true;
        Destroy(gameObject);
    }

    public IEnumerator WaitBreakWall()
    {
        yield return new WaitForSeconds(0.1f);
        BreakWall(false);
    }
}
