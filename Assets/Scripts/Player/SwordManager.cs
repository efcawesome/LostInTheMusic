using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordManager : MonoBehaviour
{
    [HideInInspector]
    public string direction;
    public int damage;
    private List<Collider2D> colliders = new List<Collider2D>();
    public AudioClip pogoSound;
    private AudioSource audioSource;

    private bool playedSound;

    private GameObject player;

    private void Start()
    {
        player = transform.parent.gameObject;
        audioSource = GameObject.Find("Virtual Camera").GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!colliders.Contains(collision) && !collision.gameObject.tag.Equals("Player")) // Check to make sure the player cant double hit something with the same sword object
        {
            Debug.Log(collision.gameObject.name + " hit!");
            colliders.Add(collision);

            switch (direction) // Do things based on direction of the sword
            {
                case Directions.DOWN:
                    if (collision.gameObject.tag.Contains("Pogoable")) 
                    {
                        player.GetComponent<PlayerMovement>().Pogo();
                        if (!collision.gameObject.tag.Contains("MusicNote") && !playedSound)
                        {
                            playedSound = true;
                            audioSource.PlayOneShot(pogoSound);
                        }
                    }
                    break;
                case Directions.RIGHT:
                    if (!collision.isTrigger || collision.gameObject.tag.Contains("Pogoable")) player.GetComponent<PlayerMovement>().Bump(1);
                    break;
                case Directions.LEFT:
                    if (!collision.isTrigger || collision.gameObject.tag.Contains("Pogoable")) player.GetComponent<PlayerMovement>().Bump(-1);
                    break;
                case Directions.UP:
                    break;
            }

            if (collision.gameObject.TryGetComponent(out DamageableObject d)) // If damageable, damage
            {
                d.Hit(damage);
            }

            if (collision.gameObject.TryGetComponent(out BreakableWall bw)) // If breakable wall, break
            {
                bw.Hit();
            }

            if (collision.gameObject.TryGetComponent(out NoteManager nm)) // If note, trigger isHit
            {
                nm.isHit = true;
                nm.audioSource.PlayOneShot(nm.goodSound);
            }
        }
    }
}
