using UnityEngine;

namespace Room
{
    [CreateAssetMenu]
    public class RoomLayout : ScriptableObject
    {
        public const int RoomHeight = 5;
        public const int RoomWidth = 9;

        [HideInInspector, SerializeField] private Tile.Tile[] tiles = new Tile.Tile[RoomHeight * RoomWidth];
    
        public Tile.Tile[] Tiles { get => tiles; }
        
        public Tile.Tile[,] GetTilesMultiDimensional()
        {
            Tile.Tile[,] tilesMultiDimensional = new Tile.Tile[RoomHeight, RoomWidth];
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

        public bool IsDoor(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return tiles[RoomWidth * (RoomHeight - 1) + RoomWidth / 2].IsDoor;
                case Direction.East:
                    return tiles[RoomWidth * (RoomHeight / 2) + RoomWidth - 1].IsDoor;
                case Direction.South:
                    return tiles[RoomWidth / 2].IsDoor;
                case Direction.West:
                    return tiles[RoomWidth * (RoomHeight / 2)].IsDoor;
                default:
                    return false;
            }   
        }
    }
}
