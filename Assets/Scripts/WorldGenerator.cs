using System.Collections.Generic;
using Room;
using UnityEngine;

internal struct PlacedRoomInformation
{
    public GameObject Room;
    public int x;
    public int y;
}

public class WorldGenerator : MonoBehaviour
{
    private static int _maxWorldSize;
    
    [SerializeField] private RoomLayout startRoomLayout;
    [SerializeField] private List<RoomLayout> bossRooms;
    [SerializeField] private List<RoomLayout> rooms;
    [SerializeField] private int numberOfRooms = 10;
    
    private readonly Stack<RoomLayout> _roomsToPlace = new();
    private readonly List<PlacedRoomInformation> _placedRooms = new();
    private readonly GameObject[,] _worldMap = new GameObject[_maxWorldSize, _maxWorldSize];
    
    // Start is called before the first frame update
    void Start()
    { 
        InitializeRoomsToPlace();
        GenerateWorldMap();
    }

    private void InitializeRoomsToPlace()
    {
        _roomsToPlace.Push(bossRooms[Random.Range(0, bossRooms.Count)]);
        // start at 2 because we already place start and boss room
        for (int i = 2; i < numberOfRooms; i++)
        {
            int randomIndex = Random.Range(0, rooms.Count);
            RoomLayout r = rooms[randomIndex];
            _roomsToPlace.Push(r);
        }
        _roomsToPlace.Push(startRoomLayout);
    }

    private void GenerateWorldMap()
    { 
        PlaceGameRooms();
    }

    private void PlaceGameRooms()
    {
        PlaceStartRoom();
        while (_roomsToPlace.Count > 1)
        {
            RoomLayout roomLayout = _roomsToPlace.Pop();
            Direction emptyDirection = Direction.None;
            RoomLayout adjacentRoomLayout = null;
            while (emptyDirection == Direction.None)
            {
                adjacentRoomLayout = AdjacentRoom();
                // emptyDirection = adjacentRoomLayout.GetRandomEmptyNeighborDirection();
            }
            // if (adjacentRoomLayout != null) adjacentRoomLayout.Neighbors[emptyDirection] = roomLayout;
            
            InstantiateRoom(roomLayout);

            // get room from rooms to place
            // get random other room
            // get empty space from other room
            // place room in empty space
            // add room to placed rooms
            // add room to world map array
            // set room active
            // set room position
            

        }
        PlaceBossRoom();
    }

    private RoomLayout AdjacentRoom()
    {
        GameObject adjacentRoomGo = _placedRooms[Random.Range(0, _placedRooms.Count)].Room;
        RoomLayout adjacentRoomLayout = adjacentRoomGo.GetComponent<RoomLayout>() ?? throw new System.Exception("Room is null");
        return adjacentRoomLayout;
    }

    private void PlaceBossRoom()
    {
        throw new System.NotImplementedException();
    }

    private void PlaceStartRoom()
    {
        RoomLayout roomLayout = _roomsToPlace.Pop();
        GameObject roomGo = InstantiateRoom(roomLayout);
        roomGo.transform.position = new Vector3(0, 0, 0);
        roomGo.SetActive(true);
        int x = Random.Range(0, _maxWorldSize);
        int y = Random.Range(0, _maxWorldSize);
        _worldMap[x, y] = roomGo;
        _placedRooms.Add(new PlacedRoomInformation { Room = roomGo, x = x, y = y });
    }
    
    private GameObject InstantiateRoom(RoomLayout layout)
    {
        GameObject room = new GameObject(name);
        room.SetActive(false);
        room.AddComponent<RoomComponent>();
        for (int i = 0; i < RoomLayout.RoomHeight; i++)
        {
            for (int j = 0; j < RoomLayout.RoomWidth; j++)
            {
                int currentIndex = (i * RoomLayout.RoomWidth + j);
                GameObject tile = new GameObject(layout.Tiles[currentIndex].name)
                {
                    transform =
                    {
                        parent = room.transform,
                        position = new Vector3(j, -i, 0)
                    }
                };
                SpriteRenderer spriteRenderer = tile.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = layout.Tiles[currentIndex].Sprite;
            }
        }
        return room;
    }
    
}
