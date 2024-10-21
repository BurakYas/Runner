using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTrap : Trap
{
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Transform[] movePoints;

    private int currentPointIndex;    

    protected override void Start()
    {
        base.Start();
        transform.position = movePoints[0].position;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoints[currentPointIndex].position, speed * Time.deltaTime); // Move the trap to the next point

        if (Vector2.Distance(transform.position, movePoints[currentPointIndex].position) < 0.1f) // Check if the trap is close to the next point
        {
            currentPointIndex++;

            if (currentPointIndex >= movePoints.Length) // Check if the trap reached the last point
                currentPointIndex = 0;
        }

        if (transform.position.x > movePoints[currentPointIndex].position.x)
            transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
        else
            transform.Rotate(new Vector3(0, 0, -rotationSpeed * Time.deltaTime));
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
}
