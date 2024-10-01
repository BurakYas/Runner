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

    private bool playerUnlocked;

    [Header("Collision info")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        AnimatorControllers();

        if (playerUnlocked)
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y); // Move the player

        ChechCollision();

        CheckInput();
    }

    private void AnimatorControllers()
    {        
        anim.SetBool("isGrounded", isGrounded); // Set the grounded animation
        anim.SetFloat("xVelocity", rb.velocity.x); // Set the x velocity for the run animation
        anim.SetFloat("yVelocity", rb.velocity.y); // Set the y velocity for the jump animation
    }

    private void ChechCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround); // Check if the player is grounded
    }

    private void CheckInput()
    {
        if (Input.GetButtonDown("Fire2"))
            playerUnlocked = true;


        if (Input.GetButtonDown("Jump") && isGrounded)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Jump
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance)); // Draw a line to show the ground check distance
    }
}
