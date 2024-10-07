using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Speed info")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float speedMultiplier;
    [Space]
    [SerializeField] private float milestoneIncreaser;
    private float speedMilestone;

    [Header("Movement info")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    private bool canDoubleJump;

    private bool playerUnlocked;

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

        speedMilestone = milestoneIncreaser;
    }

    private void Update()
    {
        CheckCollision();
        AnimatorControllers();

        slideTimeCounter -= Time.deltaTime;
        slideCooldownCounter -= Time.deltaTime;

        if (playerUnlocked)
            Movement();

        if (isGrounded)
            canDoubleJump = true;

        SpeedController();

        CheckForLedge();
        CheckForSlide();
        CheckInput();
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

    #region Ledge Climb Region
    private void CheckForLedge()
    {
        if (ledgeDetected && canGrabLedge) // Check if the player is close to a ledge and can grab it
        {
            canGrabLedge = false;

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
        transform.position = climbOverPosition; // Move the player to the climb over position
        Invoke("AllowLedgeGrab", 1f); // Allow the player to grab the ledge again after 0.5 seconds
    }

    private void AllowLedgeGrab() => canGrabLedge = true;

    private void CheckForSlide()
    {
        if (slideTimeCounter < 0 && !ceillingDetected)
            isSliding = false;
    }
    #endregion

    private void Movement()
    {
        if (wallDetected)
            return;

        if (isSliding)
            rb.velocity = new Vector2(slideSpeed, rb.velocity.y); // Slide the player
        else
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y); // Move the player
    }

    private void CheckInput()
    {
        if (Input.GetButtonDown("Fire2"))
            playerUnlocked = true;


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

    private void AnimatorControllers()
    {
        anim.SetFloat("xVelocity", rb.velocity.x); // Set the x velocity for the run animation
        anim.SetFloat("yVelocity", rb.velocity.y); // Set the y velocity for the jump animation

        anim.SetBool("canDoubleJump", canDoubleJump);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isSliding", isSliding);
        anim.SetBool("canClimb", canClimb);
    }

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
