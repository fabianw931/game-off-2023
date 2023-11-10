using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Room : ScriptableObject
{
    public const int ROOM_HEIGHT = 5;
    public const int ROOM_WIDTH = 7;

    [HideInInspector, SerializeField] private Tile[] tiles = new Tile[ROOM_HEIGHT * ROOM_WIDTH];

    public int wiegross = ROOM_HEIGHT * ROOM_WIDTH;
    public Tile[] Tiles { get => tiles; }

    public Tile[,] GetTilesMultiDimensional()
    {
        Tile[,] tilesMultiDimensional = new Tile[ROOM_HEIGHT, ROOM_WIDTH];
        int index = 0;
        for (int i = 0; i < tilesMultiDimensional.GetLength(0); i++)
        {
            for (int j = 0; j < tilesMultiDimensional.GetLength(1); j++)
            {
                tilesMultiDimensional[i, j] = tiles[index];
                index++;
            }
        }
        return tilesMultiDimensional;
    }
}
