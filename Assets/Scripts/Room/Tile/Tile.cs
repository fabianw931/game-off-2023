using UnityEngine;

namespace Room.Tile
{
    [CreateAssetMenu]

    public class Tile : ScriptableObject
    {

        [SerializeField] private bool isDoor = false;
        [SerializeField] private bool isWall = false;
        [SerializeField] private bool isFloor = false;
        [SerializeField] private bool isSpawn = false;
        [SerializeField] private bool isItemSpawn = false;

        [SerializeField] private Sprite sprite;

        public bool IsDoor => isDoor;

        public bool IsWall => isWall;

        public bool IsFloor => isFloor;

        public bool IsSpawn => isSpawn;

        public bool IsItemSpawn => isItemSpawn;

        public Sprite Sprite => sprite;
    }
}
