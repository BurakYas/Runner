using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool colorEntierPlatform;
    public Color platformColor;
    public int coins;

    private void Awake()
    {
        instance = this; // Singleton pattern
    }

    public void RestartLevel() => SceneManager.LoadScene(0);
}
