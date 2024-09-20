using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TreasureBossController : MonoBehaviour
{
    private int phase = 1;
    public GameObject music_projectile;

    private Rigidbody2D rb;

    private bool attackRunning = false;

    public float speed;

    public float attack_time;
    private float attack_timer;

    private bool hasStarted = false;

    private int health = 3;
    public List<Vector3> orbPos;
    public GameObject orbPrefab;
    private int orbsHit = 0;

    private const float rightSidePos = 12.24f;
    private const float initPos = 2.51f;
    private const float leftSidePos = 2.51f - (12.24f - 2.51f);

    public float moveConst;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        attack_timer = attack_time;

        StartCoroutine(StartBossfight());
    }

    private IEnumerator StartBossfight()
    {
        // Do camera stuff or smth

        hasStarted = true;
        StartCoroutine(SpawnOrbs());
        yield return null;
    }

    void Update()
    {
        attack_timer -= Time.deltaTime * phase;
        if (hasStarted && !attackRunning && attack_timer < 0)
        {
            StartCoroutine(DoAttack());
        }
    }

    private IEnumerator DoAttack()
    {
        attackRunning = true;
        switch(Random.Range(5, 10))
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                yield return StartCoroutine(SpiralProj());
                break;
            case 6:
                yield return StartCoroutine(RingProj());
                break;
            case 7:
                yield return StartCoroutine(Dash());
                break;
            case 8:
                yield return StartCoroutine(SpearheadProjAttack());
                break;
            default:
                yield return StartCoroutine(ShootAtPlayer());
                break;
        }
        attackRunning = false;
        attack_timer = attack_time + Random.Range(-attack_time*0.25f, attack_time * 0.25f);
    }

    private IEnumerator SpearheadProjAttack()
    {
        for(float i = 0; i <= 1; i += 0.25f)
        {
            GameObject proj = Instantiate(music_projectile, transform.position, Quaternion.Euler(0, 0, Mathf.Rad2Deg*Mathf.Atan((1 - i) / i)));
            proj.SetActive(true);
            proj.GetComponent<Rigidbody2D>().velocity = new Vector2(i, 1 - i) * speed;

            GameObject proj1 = Instantiate(music_projectile, transform.position, Quaternion.Euler(0, 0, Mathf.Rad2Deg*Mathf.Atan((1 - i) / -i)));
            proj1.SetActive(true);
            proj1.GetComponent<Rigidbody2D>().velocity = new Vector2(-i, 1 - i) * speed;

            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator Dash()
    {
        Coroutine ssp = StartCoroutine(SpawnStationaryProj(0.1f)); // Good naming conventions

        float t = 0;

        if(Random.Range(0, 2) == 0)
        {
            while (transform.position.x < rightSidePos - 0.01f)
            {
                transform.position = Vector2.Lerp(transform.position, new Vector2(rightSidePos, transform.position.y), t * moveConst);
                t += Time.deltaTime;
                yield return null;
            }

            transform.position = new Vector2(rightSidePos, transform.position.y);
        }
        else
        {
            while(transform.position.x > leftSidePos + 0.01f)
            {
                transform.position = Vector2.Lerp(transform.position, new Vector2(leftSidePos, transform.position.y), t * moveConst);
                t += Time.deltaTime;
                yield return null;
            }

            transform.position = new Vector2(leftSidePos, transform.position.y);
        }

        t = 0;

        while(Mathf.Abs(transform.position.x - initPos) > 0.01f)
        {
            transform.position = Vector2.Lerp(transform.position, new Vector2(initPos, transform.position.y), t * moveConst);
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector2(initPos, transform.position.y);

        StopCoroutine(ssp);
    }

    private IEnumerator SpawnStationaryProj(float interval)
    {
        while(true)
        {
            yield return new WaitForSeconds(interval);
            GameObject proj = Instantiate(music_projectile, transform.position, Quaternion.identity);
            proj.SetActive(true);

            GameObject proj1 = Instantiate(music_projectile, transform.position, Quaternion.identity);
            proj1.SetActive(true);
            proj1.GetComponent<Rigidbody2D>().velocity = Vector2.up * speed * 2;
        }
    }

    private IEnumerator SpiralProj()
    {
        for(float i = 0; i <= 2*Mathf.PI; i += Mathf.PI/4)
        {
            GameObject proj = Instantiate(music_projectile, transform.position, Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((1 - i) / i)));
            proj.SetActive(true);
            proj.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(i), Mathf.Sin(i)) * speed;

            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator RingProj()
    {
        for (float i = 0; i <= 2 * Mathf.PI; i += Mathf.PI / 4)
        {
            GameObject proj = Instantiate(music_projectile, transform.position, Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((1 - i) / i)));
            proj.SetActive(true);
            proj.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(i), Mathf.Sin(i)) * speed;
        }

        yield return null;
    }

    private IEnumerator ShootAtPlayer()
    {
        GameObject player = GameObject.Find("Player");
        for(int i = 0; i < 8; i++)
        {
            float ang = Vector3.Angle(transform.position, player.transform.position);
            GameObject proj = Instantiate(music_projectile, transform.position, Quaternion.Euler(0, 0, ang));
            proj.SetActive(true);
            proj.GetComponent<Rigidbody2D>().velocity = (player.transform.position - transform.position).normalized * speed; // Shoot towards player
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator SpawnOrbs()
    {
        yield return new WaitForSeconds(5f);

        while(orbsHit < health)
        {
            GameObject orb = Instantiate(orbPrefab);
            orb.SetActive(true);
            orb.transform.position = orbPos[orbsHit];

            yield return new WaitUntil(() => orb == null);

            // Indicate health down

            orbsHit++;
            yield return new WaitForSeconds(5f);
        }

        hasStarted = false;

        // Death animation
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        while(sr.color.a > 0)
        {
            var col = sr.color;
            col.a -= 4 * Time.deltaTime;
            sr.color = col;

            yield return null;
        }

        Destroy(gameObject);
    }
}
