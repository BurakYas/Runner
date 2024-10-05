using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Movement info")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    private bool canDoubleJump;

    private bool playerUnlocked;

    [Header("Slide info")]
    [SerializeField] private float slideSpeed;
    [SerializeField] private float slideTime;
    private float slideTimeCounter;
    private bool isSliding;

    [Header("Collision info")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Vector2 wallCheckSize;
    private bool isGrounded;
    private bool wallDetected;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        ChechCollision();
        AnimatorControllers();

        slideTimeCounter -= Time.deltaTime;

        if (playerUnlocked && !wallDetected)
            Movement();

        if (isGrounded)
            canDoubleJump = true;

        CheckForSlide();
        CheckInput();
    }

    private void CheckForSlide()
    {
        if (slideTimeCounter < 0)
            isSliding = false;
    }

    private void Movement()
    {
        if (isSliding)
            rb.velocity = new Vector2(slideSpeed, rb.velocity.y); // Slide the player
        else
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y); // Move the player
    }

    private void AnimatorControllers()
    {
        anim.SetFloat("xVelocity", rb.velocity.x); // Set the x velocity for the run animation
        anim.SetFloat("yVelocity", rb.velocity.y); // Set the y velocity for the jump animation

        anim.SetBool("canDoubleJump", canDoubleJump);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isSliding", isSliding);
    }

    private void ChechCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround); // Check if the player is grounded
        wallDetected = Physics2D.BoxCast(wallCheck.position, wallCheckSize, 0, Vector2.zero, 0, whatIsGround); // Check if the player is touching a wall
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
        if (rb.velocity.x != 0)
        {
            isSliding = true;
            slideTimeCounter = slideTime;
        }
    }

    private void JumpButton()
    {
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance)); // Draw a line to show the ground check distance
        Gizmos.DrawWireCube(wallCheck.position, wallCheckSize); // Draw a wire cube to show the wall check size
    }
}
