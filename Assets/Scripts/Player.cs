using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    private bool isDead;
    [HideInInspector] public bool playerUnlocked;
    [HideInInspector] public bool extraLife;

    [Header("Knockback info")]
    [SerializeField] private Vector2 knockbackDir;
    private bool isKnocked;
    private bool canBeKnocked = true;

    [Header("Move info")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float speedMultiplier;
    private float defaultSpeed;
    [Space]
    [SerializeField] private float milestoneIncreaser;
    private float defaultMilestoneIncrease;
    private float speedMilestone;

    [Header("Jump info")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;    
    private bool canDoubleJump;

    [Header("Slide info")]
    [SerializeField] private float slideSpeed;
    [SerializeField] private float slideTime;
    [SerializeField] private float slideCooldown;
    private float slideCooldownCounter;
    private float slideTimeCounter;
    private bool isSliding;

    [Header("Collision info")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float ceillingCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Vector2 wallCheckSize;
    private bool isGrounded;
    private bool wallDetected;
    private bool ceillingDetected;
    [HideInInspector] public bool ledgeDetected;

    [Header("Ledge info")]
    [SerializeField] private Vector2 offset1;
    [SerializeField] private Vector2 offset2;

    private Vector2 climbBegunPosition;
    private Vector2 climbOverPosition;

    private bool canGrabLedge = true;
    private bool canClimb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        speedMilestone = milestoneIncreaser;
        defaultSpeed = moveSpeed;
        defaultMilestoneIncrease = milestoneIncreaser;
    }

    private void Update()
    {
        CheckCollision();
        AnimatorControllers();

        slideTimeCounter -= Time.deltaTime;
        slideCooldownCounter -= Time.deltaTime;

        extraLife = moveSpeed >= maxSpeed;

        //if (Input.GetKeyDown(KeyCode.K))
        //    Knockback();

        //if (Input.GetKeyDown(KeyCode.L) && !isDead)
        //    StartCoroutine(Die());

        if (isDead)
            return;

        if (isKnocked)
            return;

        if (playerUnlocked)
            SetupMovement();

        if (isGrounded)
            canDoubleJump = true;

        SpeedController();

        CheckForLedge();
        CheckForSlideCancel();
        CheckInput();
    }

    public void Damage()
    {
        if (extraLife)
            Knockback();
        else
            StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        isDead = true;
        canBeKnocked = false;
        rb.velocity = knockbackDir;
        anim.SetBool("isDead", true);

        yield return new WaitForSeconds(1f);
        rb.velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(1f);
        GameManager.instance.RestartLevel();
    }

    #region Knockback
    private IEnumerator Invincibility()
    {
        Color originalColor = sr.color;
        Color darkenColor = new Color(sr.color.r, sr.color.g, sr.color.b, .5f); // Darken the player's color when it gets hit 

        canBeKnocked = false;
        sr.color = darkenColor;
        yield return new WaitForSeconds(.1f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.1f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(.15f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.15f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(.25f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.25f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(.3f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.35f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(.4f);

        sr.color = originalColor;
        canBeKnocked = true;        
    }

    private void Knockback()
    {
        if (!canBeKnocked)
            return;

        StartCoroutine(Invincibility());

        isKnocked = true;
        rb.velocity = knockbackDir;
    }

    private void CancelKnockback() => isKnocked = false;
    #endregion

    #region SpeedControll
    private void SpeedReset()
    {
        moveSpeed = defaultSpeed;
        milestoneIncreaser = defaultMilestoneIncrease;
    }

    private void SpeedController()
    {
        if (moveSpeed == maxSpeed)
            return;

        if (transform.position.x > speedMilestone)
        {
            speedMilestone += milestoneIncreaser;

            moveSpeed += speedMultiplier;
            milestoneIncreaser += speedMilestone; // Increase the milestone

            if (moveSpeed > maxSpeed)
                moveSpeed = maxSpeed;
        }
    }
    #endregion

    #region Ledge Climb Region
    private void CheckForLedge()
    {
        if (ledgeDetected && canGrabLedge) // Check if the player is close to a ledge and can grab it
        {
            canGrabLedge = false;
            rb.gravityScale = 0; // Set the gravity scale to 0 to make the player stay in the air

            Vector2 ledgePosition = GetComponentInChildren<LedgeDetection>().transform.position; // Get the position of the ledge detected

            climbBegunPosition = ledgePosition + offset1; // Set the position where the player will start to climb
            climbOverPosition = ledgePosition + offset2;

            canClimb = true;
        }

        if (canClimb)
            transform.position = climbBegunPosition; // Move the player to the climb begun position
    }

    private void LedgeClimbOver()
    {
        canClimb = false;
        rb.gravityScale = 5;
        transform.position = climbOverPosition; // Move the player to the climb over position
        Invoke("AllowLedgeGrab", 1f); // Allow the player to grab the ledge again after 0.5 seconds
    }

    private void AllowLedgeGrab() => canGrabLedge = true;

    #endregion

    private void CheckForSlideCancel()
    {
        if (slideTimeCounter < 0 && !ceillingDetected)
            isSliding = false;
    }

    private void SetupMovement()
    {
        if (wallDetected)
        {
            SpeedReset();
            return;
        }

        if (isSliding)
            rb.velocity = new Vector2(slideSpeed, rb.velocity.y); // Slide the player
        else
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y); // Move the player
    }

    #region Input
    private void CheckInput()
    {
        //if (Input.GetButtonDown("Fire2"))
        //    playerUnlocked = true;


        if (Input.GetButtonDown("Jump"))
            JumpButton();

        if (Input.GetKeyDown(KeyCode.LeftShift))
            SlideButton();
    }

    private void SlideButton()
    {
        if (rb.velocity.x != 0 && slideCooldownCounter < 0)
        {
            isSliding = true;
            slideTimeCounter = slideTime;
            slideCooldownCounter = slideCooldown; // Set the slide cooldown counter
        }
    }

    private void JumpButton()
    {
        if (isSliding)
            return;

        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        else if (canDoubleJump)
        {
            canDoubleJump = false;
            rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
        }
    }
    #endregion

    #region Animations
    private void AnimatorControllers()
    {
        anim.SetFloat("xVelocity", rb.velocity.x); // Set the x velocity for the run animation
        anim.SetFloat("yVelocity", rb.velocity.y); // Set the y velocity for the jump animation

        anim.SetBool("canDoubleJump", canDoubleJump);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isSliding", isSliding);
        anim.SetBool("canClimb", canClimb);
        anim.SetBool("isKnocked", isKnocked);

        if (rb.velocity.y < -20)
            anim.SetBool("canRoll", true);
    }

    private void RollAnimFinished() => anim.SetBool("canRoll", false); // Set the canRoll parameter to false when the roll animation is finished
    #endregion

    private void CheckCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround); // Check if the player is grounded
        ceillingDetected = Physics2D.Raycast(transform.position, Vector2.up, ceillingCheckDistance, whatIsGround); // Check if the player is touching a ceilling
        wallDetected = Physics2D.BoxCast(wallCheck.position, wallCheckSize, 0, Vector2.zero, 0, whatIsGround); // Check if the player is touching a wall
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance)); // Draw a line to show the ground check distance
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y + ceillingCheckDistance)); // Draw a line to show the ceilling check distance
        Gizmos.DrawWireCube(wallCheck.position, wallCheckSize); // Draw a wire cube to show the wall check size
    }
}
