using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ColorToSell
{
    public Color color;
    public float price;
}

public class UI_Shop : MonoBehaviour
{
    [SerializeField] private GameObject platformColorButton;
    [SerializeField] private Transform platformColorParent;

    [SerializeField] private ColorToSell[] platformColors;

    void Start()
    {
        for (int i = 0; i < platformColors.Length; i++)
        {
            GameObject newButton = Instantiate(platformColorButton, platformColorParent);
        }
    }

    void Update()
    {

    }
}
