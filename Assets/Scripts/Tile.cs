using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class Tile : ScriptableObject
{

    [SerializeField] private bool isDoor = false;
    [SerializeField] private bool isWall = false;
    [SerializeField] private bool isFloor = false;
    [SerializeField] private bool isSpawn = false;
    [SerializeField] private bool isItemSpawn = false;

    [SerializeField] private Sprite sprite;

    public Sprite Sprite { get => sprite; set => sprite = value; }
}
