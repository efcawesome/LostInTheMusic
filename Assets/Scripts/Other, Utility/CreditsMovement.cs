using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsMovement : MonoBehaviour
{
    public float moveSpeed;
    public TMP_Text text;
    public TMP_Text hatText;
    public TMP_Text timeText;
    public Image i;

    private void Start()
    {
        i.color = new Color(1, 1, 1, 0);
        hatText.alpha = 0;
        timeText.alpha = 0;
        GameObject.Find("Player").GetComponent<PlayerMovement>().RevokeControl();
        GameObject.Find("Player").GetComponent<Rigidbody2D>().gravityScale = 0f;
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        yield return new WaitForSeconds(1.5f);
        while (i. color.a < 1)
        {
            var col = i.color;
            col.a += Time.deltaTime;
            i.color = col;
            yield return null;
        }

        var c0 = i.color;
        c0.a = 1f;
        i.color = c0;

        while (transform.localPosition.y < 3389)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + moveSpeed * Time.deltaTime * 50 * (Input.GetButton("Jump") ? 3 : 1));
            yield return null;
        }

        yield return new WaitForSeconds(2.5f);

        yield return StartCoroutine(GameManager.FadeTextOut(text, 0.5f));

        while (i.color.a > 0)
        {
            var col = i.color;
            col.a -= Time.deltaTime;
            i.color = col;
            yield return null;
        }

        var c = i.color;
        c.a = 0;
        i.color = c;

        yield return new WaitForSeconds(1f);
        hatText.text = $"Hats Collected:<br>{GameObject.Find("GameManager").GetComponent<GameManager>().HatCount()}/7";
        timeText.text = $"Final Time:<br> {(int)(Mathf.Round(GameManager.endTime - GameManager.initTime)/60)} minutes {Mathf.Round(GameManager.endTime - GameManager.initTime) % 60} seconds";
        StartCoroutine(GameManager.FadeTextIn(hatText, 2f));
        StartCoroutine(GameManager.FadeTextIn(timeText, 2f));
    }
}
