using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinGenerator : MonoBehaviour
{
    private int amountOfCoins;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int minCoins;
    [SerializeField] private int maxCoins;

    [SerializeField] private SpriteRenderer[] coinImg;

    private void Start()
    {
        for (int i = 0; i < coinImg.Length; i++)
        {
            coinImg[i].sprite = null;
        }

        amountOfCoins = Random.Range(minCoins, maxCoins); // Randomize the amount of coins to spawn
        int additinalOffset = amountOfCoins / 2;

        for (int i = 0; i < amountOfCoins; i++)
        {
            Vector3 offset = new Vector2(i - additinalOffset, 0); // Calculate the offset of the coin
            Instantiate(coinPrefab, transform.position + offset, Quaternion.identity, transform); // Spawn a coin
        }
    }
}
