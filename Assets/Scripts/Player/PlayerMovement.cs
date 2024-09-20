using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private SpriteRenderer sr;
    private Animator animator;
    private AudioSource audioSource;
    private ParticleSystem ps;

    public float speed;
    public float jumpForce;
    private bool isMoving;

    public bool isGrounded;
    public Transform feetPos;
    public LayerMask groundLayer;

    private Vector2 respawnLocation;

    private float jumpTimeCounter;
    public float jumpTime;
    private bool isJumping;

    public bool hasControl = true;
    public bool canMove = true;
    public bool canJump = true;
    public bool canDash = true;

    private bool isDashing = false;
    private float dashTimeElapsed = 0;
    public float dashVelocity;
    public float dashDuration;
    public float dashCooldown;
    private float dashCooldownRemaining;
    private bool landedAfterDash;

    public float pogoForce;
    public float bumpTime;
    private float bumpTimer;

    public float slowFallGravityScale;
    public float slowFallTime;
    private float slowFallElapsed = 0;
    private bool isSlowFalling = false;
    public bool slowFallUnlocked = false;

    private bool respawning = false;

    public bool developerMode = false;

    private float initialGravityScale;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        // Get rigidbody
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        ps = GetComponentInChildren<ParticleSystem>();
        var rate = ps.emission;
        rate.rateOverTime = 0;
        initialGravityScale = rb.gravityScale;
    }

    private void FixedUpdate()
    {
        //Check if grounded by using an invisible box (its not jank i swear) (dont even worry about testing the length to detect if grounded)
        isGrounded = Physics2D.OverlapBox(feetPos.position, feetPos.gameObject.GetComponent<BoxCollider2D>().size * feetPos.lossyScale, 0, groundLayer);

        if (canMove)
        {
            // Horizontal movement
            rb.velocity = new Vector2(speed * Input.GetAxisRaw("Horizontal"), rb.velocity.y);

        }

        // Make sprite face the right direction
        if(PlayerAttack.canTurn)
        {
            if (rb.velocity.x > 0)
            {
                if(bc.offset.x < 0)
                {
                    var offset = bc.offset;
                    offset.x = -offset.x;
                    bc.offset = offset;
                    transform.GetChild(0).localPosition = new Vector2(offset.x, transform.GetChild(0).localPosition.y);
                }
                sr.flipX = false;
            }
            else if (rb.velocity.x < 0)
            {
                if (bc.offset.x > 0)
                {
                    var offset = bc.offset;
                    offset.x = -offset.x;
                    bc.offset = offset;
                    transform.GetChild(0).localPosition = new Vector2(offset.x, transform.GetChild(0).localPosition.y);
                }
                sr.flipX = true;
            }
        }

        if (developerMode)
        {
            canJump = false;
            canMove = false;
            rb.gravityScale = 0;

            GetComponent<BoxCollider2D>().enabled = false;

            var vel = rb.velocity;
            vel.y = Input.GetAxisRaw("Vertical") * speed;
            vel.x = Input.GetAxisRaw("Horizontal") * speed;
            rb.velocity = vel;
        }
    }

    private void Update()
    {

        isMoving = isGrounded && canMove && Input.GetAxisRaw("Horizontal") != 0;

        animator.SetBool("isMoving", isMoving);
        animator.SetFloat("movingBlend", (isGrounded && canMove && Input.GetAxisRaw("Horizontal") != 0) ? 1 : 0);
        animator.SetBool("isJumping", rb.velocity.y > 0);
        animator.SetBool("isFalling", rb.velocity.y < 0);
        animator.SetBool("isGliding", isSlowFalling && rb.velocity.y <= 0);

        if(isMoving && !audioSource.isPlaying) audioSource.Play();
        else if (!isMoving && audioSource.isPlaying) audioSource.Pause();

        if (canJump)
        {
            //start normal jump
            if (isGrounded && Input.GetButtonDown("Jump"))
            {
                isJumping = true;
                jumpTimeCounter = jumpTime;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                rb.gravityScale = 0;
            }

            //keep jumping
            if (Input.GetButton("Jump") && isJumping)
            {
                if (jumpTimeCounter > 0)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    jumpTimeCounter -= Time.deltaTime;
                }
                else
                {
                    isJumping = false;
                    isSlowFalling = slowFallUnlocked;
                    rb.gravityScale = initialGravityScale;
                }
            }

            //stop jumping
            if (Input.GetButtonUp("Jump") && isJumping)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                isJumping = false;
                rb.gravityScale = initialGravityScale;
            }


            //start slow falling
            if (slowFallUnlocked && !isGrounded && !isJumping && !isSlowFalling && slowFallElapsed == 0 && Input.GetButtonDown("Jump"))
            {
                isSlowFalling = true;
            }

            //keep slow falling
            if (isSlowFalling && Input.GetButton("Jump"))
            {
                if (slowFallElapsed > 0)
                {
                    if (slowFallElapsed < slowFallTime)
                    {
                        slowFallElapsed += Time.deltaTime;
                    }
                    else
                    {
                        isSlowFalling = false;
                        rb.gravityScale = initialGravityScale;
                    }
                }

                if (slowFallElapsed == 0 && rb.velocity.y <= 0)
                {
                    rb.gravityScale = slowFallGravityScale;
                    slowFallElapsed += Time.deltaTime;
                }
            }

            //stop slow falling
            if (Input.GetButtonUp("Jump") && isSlowFalling)
            {
                isSlowFalling = false;
                rb.gravityScale = initialGravityScale;
            }

            if (isGrounded && slowFallElapsed > 0)
            {
                resetSlowFall();
            }
        }

        //start dash
        if (canDash && canMove && !isDashing && (dashCooldownRemaining <= 0) && landedAfterDash && Input.GetButtonDown("Dash"))
        {
            isDashing = true;
            canMove = false;
            isJumping = false;
            canJump = false;
            dashTimeElapsed = 0;
            dashCooldownRemaining = dashCooldown;
            rb.gravityScale = 0;
            landedAfterDash = false;

            var rate = ps.emission;
            rate.rateOverTime = 50;

            int dir;
            if (sr.flipX) dir = -1; //negative x direction => moving left
            else dir = 1; //positive x direction => moving right
            rb.velocity = new Vector2(dashVelocity * dir, 0);
        }

        if (isDashing)
        {
            dashTimeElapsed += Time.deltaTime;
            if (dashTimeElapsed >= dashDuration)
            {
                if (hasControl)
                {
                    canMove = true;
                    canJump = true;
                }
                rb.gravityScale = initialGravityScale;
                isDashing = false;
                var rate = ps.emission;
                rate.rateOverTime = 0;
                resetSlowFall();
            }
        }

        if (dashCooldownRemaining > 0) dashCooldownRemaining -= Time.deltaTime;

        if (!landedAfterDash && !isDashing && isGrounded) landedAfterDash = true;
    }

    private void resetSlowFall()
    {
        slowFallElapsed = 0;
        isSlowFalling = false;
        rb.gravityScale = initialGravityScale;
    }

    /// <summary>
    /// Give player upwards velocity, stop them from jumping if mid jump
    /// </summary>
    public void Pogo()
    {
        var vel = rb.velocity;
        vel.y = jumpForce;
        rb.velocity = vel;
        isJumping = false;
        dashCooldown = 0;

        resetSlowFall();
    }

    public void RevokeControl()
    {
        hasControl = false;
        canJump = false;
        canMove = false;
        rb.gravityScale = initialGravityScale;
        GetComponent<PlayerAttack>().canAttack = false;
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    public void GiveControl()
    {
        hasControl = true;
        canJump = true;
        canMove = true;
        GetComponent<PlayerAttack>().canAttack = true;
    }

    /// <summary>
    /// Start Bump Coroutine (bumps the player a little to the right or left)
    /// </summary>
    /// <param name="dir">direction to bump</param>
    public void Bump(int dir)
    {
        StartCoroutine(BumpCoroutine(dir));
    }

    /// <summary>
    /// Bumps player to left or right
    /// </summary>
    /// <param name="dir">directino to bump</param>
    /// <returns></returns>
    private IEnumerator BumpCoroutine(int dir)
    {
        canMove = false;
        rb.velocity = new Vector2(-dir * speed * 0.75f, rb.velocity.y);
        bumpTimer = bumpTime;
        while (bumpTimer > 0)
        {
            bumpTimer -= Time.deltaTime;
            yield return null;
        }
        canMove = true;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (!respawning && collision.gameObject.tag.Contains("Spike")) StartCoroutine(Respawn());
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("RespawnArea"))
        {
            print("entered a respawn area");
            respawnLocation = collision.gameObject.GetComponent<RespawnAreaController>().respawnTarget.transform.position;
        }
    }

    private IEnumerator Respawn()
    {
        respawning = true;
        Time.timeScale = 0f;
        RevokeControl();
        yield return new WaitForSecondsRealtime(0.5f);
        print("respawned");
        Time.timeScale = 1f;
        rb.transform.position = respawnLocation;
        rb.velocity = Vector2.zero;
        GiveControl();
        respawning = false;
    }

    public void ResetGravityScale()
    {
        rb.gravityScale = initialGravityScale;
    }
}
