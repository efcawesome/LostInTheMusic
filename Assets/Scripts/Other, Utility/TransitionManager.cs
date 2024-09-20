using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static bool isTransitioning = false;

    private GameObject player;
    public float transitionSpeed;

    public string transitionDir;
    public string newScene;
    public Vector3 startingPos;

    private string startingScene;

    private void Start()
    {
        startingScene = SceneManager.GetActiveScene().name;
        player = GameObject.Find("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (SceneManager.GetActiveScene().name.Equals(startingScene) && collision.gameObject.name.Equals("Player") && !isTransitioning)
        {
            StartCoroutine(StartSceneTransition());
        }
    }

    private IEnumerator StartSceneTransition()
    {
        DontDestroyOnLoad(this);

        isTransitioning = true;

        player.GetComponent<PlayerMovement>().RevokeControl();
        var vel = player.GetComponent<Rigidbody2D>().velocity;

        switch (transitionDir)
        {
            case Directions.RIGHT:
                vel.x = transitionSpeed;
                if (vel.y > 0) vel.y = 0;

                player.GetComponent<PlayerMovement>().ResetGravityScale();

                while (!(player.transform.position.x > transform.position.x + 5)) {
                    player.GetComponent<Rigidbody2D>().velocity = vel;
                    yield return null;
                }
                break;
            case Directions.LEFT:
                vel.x = -transitionSpeed;
                if (vel.y > 0) vel.y = 0;

                player.GetComponent<PlayerMovement>().ResetGravityScale();

                while (!(player.transform.position.x < transform.position.x - 5))
                {
                    player.GetComponent<Rigidbody2D>().velocity = vel;
                    yield return null;
                }
                break;
            case Directions.UP:
                player.GetComponent<Rigidbody2D>().gravityScale = 0;
                vel.y = transitionSpeed;
                
                while (!(player.transform.position.y > transform.position.y + 5))
                {
                    player.GetComponent<Rigidbody2D>().velocity = vel;
                    yield return null;
                }
                break;
            case Directions.DOWN:
                player.GetComponent<Rigidbody2D>().gravityScale = 0;
                vel.y = -transitionSpeed;
                
                while (!(player.transform.position.y < transform.position.y - 5))
                {
                    player.GetComponent<Rigidbody2D>().velocity = vel;
                    yield return null;
                }
                break;
        }

        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        GameObject.Find("GameManager").GetComponent<GameManager>().StartHideScreen(0.6f);

        SceneManager.LoadScene(newScene);

        player.GetComponent<Rigidbody2D>().gravityScale = 3;
        player.GetComponent<PlayerMovement>().GiveControl();
        player.transform.position = startingPos;

        isTransitioning = false;

        Destroy(gameObject);
    }
}
