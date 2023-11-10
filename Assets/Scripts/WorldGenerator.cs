using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{

    [SerializeField] List<Room> rooms = new List<Room>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (Room room in rooms)
        {
            Tile[,] tiles = room.GetTilesMultiDimensional();
            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    if (tiles[i, j] != null)
                    {
                        GameObject tile = new GameObject(tiles[i, j].name);
                        SpriteRenderer spriteRenderer = tile.AddComponent<SpriteRenderer>();
                        spriteRenderer.sprite = tiles[i, j].Sprite;
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
