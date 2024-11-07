using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Player player;
    public bool colorEntierPlatform;

    [Header("Color Info")]
    public Color platformColor;
    public Color playerColor = Color.white;

    [Header("Score Info")]
    public int coins;
    public float distance;

    private void Awake()
    {
        instance = this; // Singleton pattern
    }

    private void Update()
    {
        if (player.transform.position.x > distance)
            distance = player.transform.position.x;
    }

    public void UnlockPlayer() => player.playerUnlocked = true;
    public void RestartLevel() => SceneManager.LoadScene(0);
}
