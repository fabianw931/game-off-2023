using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Room
{
    public class RoomComponent : MonoBehaviour
    {
        public RoomLayout RoomLayout { get; set; }
        public Dictionary<Direction, RoomLayout> Neighbors { get; } = new();
        
        public Direction GetRandomEmptyNeighborDirection()
        {
            List<Direction> emptyNeighbors = (from neighbor in Neighbors where neighbor.Value == null select neighbor.Key).ToList();
            return emptyNeighbors.Count == 0 ? Direction.None : emptyNeighbors[Random.Range(0, emptyNeighbors.Count)];
        }

    }
}