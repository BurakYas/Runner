using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Transform[] levelPart;
    [SerializeField] private Vector3 nextPartPosition; // The position where the next part of the level will be spawned

    [SerializeField] private float distanceToSpawn;
    [SerializeField] private float distanceToDelete;
    [SerializeField] private Transform player;

    private void Update()
    {
        DeletePlatform();
        GeneratePlatform();
    }

    private void GeneratePlatform()
    {
        while (Vector2.Distance(player.transform.position, nextPartPosition) < distanceToSpawn) // Check if the player is close to the next part of the level
        {
            Transform part = levelPart[Random.Range(0, levelPart.Length)]; // Get a random part of the level to spawn it

            Vector2 newPosition = new Vector2(nextPartPosition.x - part.Find("StartPoint").position.x, 0); // Calculate the new position of the part

            Transform newPart = Instantiate(part, newPosition, transform.rotation, transform); // Spawn the part of the level

            nextPartPosition = newPart.Find("EndPoint").position; // Set the next part position to the end position of the part
        }
    }

    private void DeletePlatform()
    {
       if (transform.childCount > 0)
        {
            Transform partToDelete = transform.GetChild(0); // Get the first part of the level

            if (Vector2.Distance(player.transform.position, partToDelete.transform.position) > distanceToDelete) 
                Destroy(partToDelete.gameObject); 
        }
    } 
}
