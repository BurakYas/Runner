using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeDetection : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Player player;

    private bool canDetected;
    
    //private BoxCollider2D boxCd => GetComponent<BoxCollider2D>();

    private void Update()
    {
        if (canDetected)
            player.ledgeDetected = Physics2D.OverlapCircle(transform.position, radius, whatIsGround); // Check if the player is close to a ledge
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            canDetected = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Collider2D[] colliders = Physics2D.OverlapBoxAll(boxCd.bounds.center, boxCd.size, 0); // Check if the player is on the ground or not to avoid ledge detection when jumping

        //foreach (var hit in colliders)
        //{
        //    if (hit.gameObject.GetComponent<PlatformController>() != null)
        //        return;
        //}

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            canDetected = true;
    }

    private void OnDrawGizmos()
    {        
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
