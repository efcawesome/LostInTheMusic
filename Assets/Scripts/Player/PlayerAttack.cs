using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public static bool canTurn = true;

    public bool canAttack = true;
    public bool hasBaton = false;

    public GameObject attackObj;
    public float attackSpeed;
    private float attackSpeedTimer;
    public float attackDuration;
    private float attackDurationTimer;
    private bool isAttacking;
    private GameObject swordClone;
    public List<float> offsets; // [up, down, left, right]
    public int damage;

    public AudioClip swordSwipeClip;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Create a new Sword Object
    /// </summary>
    /// <param name="offsetX">x offset from the player</param>
    /// <param name="offsetY">y offset from the player</param>
    /// <param name="rotation">rotation of the sword swing</param>
    private void CreateSword(float offsetX, float offsetY, float rotation)
    {
        swordClone = Instantiate(attackObj);
        swordClone.transform.SetParent(transform);
        swordClone.transform.localPosition = new Vector3(offsetX, offsetY, 0);
        swordClone.transform.eulerAngles = new Vector3(0, 0, rotation);
        swordClone.GetComponent<SwordManager>().damage = damage;
        swordClone.SetActive(true);
        attackDurationTimer = attackDuration;
        attackSpeedTimer = attackSpeed;
        isAttacking = true;
    }

    void Update()
    {
        canTurn = !isAttacking; // Player sprite cannot turn while attacking
        if(hasBaton && canAttack && Input.GetButtonDown("Attack") && !isAttacking && attackSpeedTimer <= 0) // if attack button pressed and not attacking
        {
            GameObject.Find("Main Camera").GetComponent<AudioSource>().PlayOneShot(swordSwipeClip);
            if (Input.GetAxisRaw("Vertical") > 0) // Attacking up
            {
                CreateSword(0, offsets[0], 90);
                swordClone.GetComponent<SwordManager>().direction = Directions.UP;
                swordClone.GetComponent<SpriteRenderer>().flipY = GetComponent<SpriteRenderer>().flipX;
            }
            else if (Input.GetAxisRaw("Vertical") < 0 && !GetComponent<PlayerMovement>().isGrounded) //Attacking down (cant attack down when on ground)
            {
                CreateSword(0, offsets[1], -90);
                swordClone.GetComponent<SwordManager>().direction = Directions.DOWN;
                swordClone.GetComponent<SpriteRenderer>().flipY = GetComponent<SpriteRenderer>().flipX;
            }
            else if (Input.GetAxisRaw("Horizontal") < 0) // Attacking left
            {
                CreateSword(offsets[2], 0, 0);
                swordClone.GetComponent<SwordManager>().direction = Directions.LEFT;
                swordClone.GetComponent<BoxCollider2D>().offset = new Vector2(-swordClone.GetComponent<BoxCollider2D>().offset.x, swordClone.GetComponent<BoxCollider2D>().offset.y);
                swordClone.GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (Input.GetAxisRaw("Horizontal") > 0) //Attacking right
            {
                CreateSword(offsets[3], 0, 0);
                swordClone.GetComponent<SwordManager>().direction = Directions.RIGHT;
            }
            else // if no direction is held then determine direction based on the way the player is facing
            {
                if(GetComponent<SpriteRenderer>().flipX) // Facing left
                {
                    CreateSword(offsets[2], 0, 0);
                    swordClone.GetComponent<SwordManager>().direction = Directions.LEFT;
                    swordClone.GetComponent<BoxCollider2D>().offset = new Vector2(-swordClone.GetComponent<BoxCollider2D>().offset.x, swordClone.GetComponent<BoxCollider2D>().offset.y);
                    swordClone.GetComponent<SpriteRenderer>().flipX = true;
                }
                else // Facing right
                {
                    CreateSword(offsets[3], 0, 0);
                    swordClone.GetComponent<SwordManager>().direction = Directions.RIGHT;
                }
            }
        }

        if(attackDurationTimer > 0 && isAttacking) // Decrement attack timer
        {
            attackDurationTimer -= Time.deltaTime;
        }
        else if (isAttacking) // If attack timer is less than zero, stop attacking
        {
            Destroy(swordClone);
            isAttacking = false;
        }

        if(attackSpeedTimer > 0) // Decrement timer until player can attack again (probably useless because attacking will be same duration as life of a sword object
        {
            attackSpeedTimer -= Time.deltaTime;
        }

        animator.SetBool("isAttacking", swordClone != null);
    }
}
