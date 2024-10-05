using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private GameObject cam;

    [SerializeField] private float parallaxEffect;

    private float length;
    private float xPosition;

    private void Start()
    {
        cam = GameObject.Find("Main Camera");
        length = GetComponent<SpriteRenderer>().bounds.size.x; // Get the length of the background sprite in the x axis
        xPosition = transform.position.x; // Get the initial x position of the background
    }

    private void Update()
    {
        float temp = cam.transform.position.x * (1 - parallaxEffect); // Calculate the distance to move the background
        float distance = cam.transform.position.x * parallaxEffect; // Calculate the distance to move the background
        transform.position = new Vector3(xPosition + distance, transform.position.y); // Move the background

        if (temp > xPosition + length) // Check if the background is out of the camera view
            xPosition += length; // Move the background to the right
    }
}
