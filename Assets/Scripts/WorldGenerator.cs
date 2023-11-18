using System;
using System.Collections.Generic;
using Room;
using UnityEngine;
using Random = UnityEngine.Random;

internal struct PlacedRoomInformation
{
    public GameObject Room;
    public int x;
    public int y;
}

public class WorldGenerator : MonoBehaviour
{
    // set high value to avoid out of bounds exceptions
    private const int MaxWorldSize = 100;
    // set start room to 50 because we want to start in the middle of the world
    private const int startRoom = 50;

    [SerializeField] private RoomLayout startRoomLayout;
    [SerializeField] private List<RoomLayout> bossRooms;
    [SerializeField] private List<RoomLayout> rooms;
    [SerializeField] private int numberOfRooms = 10;
    
    private readonly Stack<RoomLayout> _roomsToPlace = new();
    private readonly List<PlacedRoomInformation> _placedRooms = new();
    private readonly GameObject[,] _worldMap = new GameObject[MaxWorldSize, MaxWorldSize];
    
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
            _roomsToPlace.Push(Instantiate(r));
        }
        _roomsToPlace.Push(startRoomLayout);
    }

    private void GenerateWorldMap()
    {
        PlaceGameRooms();
    }

    private void PlaceGameRooms()
    {
        // start room coordinates correspond to middle of camera
        PlaceStartRoom(_roomsToPlace.Pop(), -4, 2);
        while (_roomsToPlace.Count > 0)
        {
            // get room from rooms to place
            GameObject roomGo = InstantiateRoom(_roomsToPlace.Pop());
            
            // get random other room
            PlacedRoomInformation adjacentRoomInfo = _placedRooms[Random.Range(0, _placedRooms.Count)];
            RoomComponent adjacentRoomComponent = adjacentRoomInfo.Room.GetComponent<RoomComponent>() ?? throw new Exception("Room is null");
            
            // get empty space from other room
            Direction placeDirection = adjacentRoomComponent.GetRandomEmptyNeighborDirection();

            // place room in empty space
            adjacentRoomComponent.Neighbors[placeDirection] = roomGo.GetComponent<RoomComponent>();
            roomGo.GetComponent<RoomComponent>().Neighbors[DirectionHelper.GetOppositeDirection(placeDirection)] = adjacentRoomComponent;
            
            var x = adjacentRoomInfo.x;
            var y = adjacentRoomInfo.y;
            
            SetRoomToWorldMap(placeDirection, roomGo, y: ref y, x: ref x);
            SetNeighbors(roomGo, x, y);
            SetRoomTransform(adjacentRoomInfo, placeDirection, roomGo);

            roomGo.SetActive(true);
            
            _placedRooms.Add(new PlacedRoomInformation
            {
                Room = roomGo,
                x = x,
                y = y
            });
        }
    }

    private void SetRoomToWorldMap(
        Direction placeDirection, 
        GameObject roomGo, 
        ref int y, 
        ref int x
        )
    {
        switch (placeDirection)
        {
            case Direction.North:
            case Direction.South:
                y = DirectionHelper.GetYValue(y, placeDirection);
                break;
            case Direction.East:
            case Direction.West:
                x = DirectionHelper.GetXValue(x, placeDirection);
                break;
            case Direction.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _worldMap[x, y] = roomGo;
    }

    private static void SetRoomTransform(
        PlacedRoomInformation adjacentRoomInfo, 
        Direction placeDirection,
        GameObject roomGo
        )
    {
        var transformX = adjacentRoomInfo.Room.transform.position.x;
        var transformY = adjacentRoomInfo.Room.transform.position.y;
            
        switch (placeDirection)
        {
            case Direction.North:
            case Direction.South:
                transformY = DirectionHelper.GetYValueTransform(transformY, placeDirection);
                break;
            case Direction.East:
            case Direction.West:
                transformX = DirectionHelper.GetXValueTransform(transformX, placeDirection);
                break;
            case Direction.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
            
        roomGo.transform.position = new Vector3(transformX, transformY, 0);
    }

    private void SetNeighbors(GameObject roomGo, int x, int y)
    {
        RoomComponent roomComponent = roomGo.GetComponent<RoomComponent>() ?? throw new Exception("Room is null");
        roomComponent.Neighbors[Direction.North] = _worldMap[x, y + 1]?.GetComponent<RoomComponent>();
        roomComponent.Neighbors[Direction.East] = _worldMap[x + 1, y]?.GetComponent<RoomComponent>();
        roomComponent.Neighbors[Direction.South] = _worldMap[x, y - 1]?.GetComponent<RoomComponent>();
        roomComponent.Neighbors[Direction.West] = _worldMap[x - 1, y]?.GetComponent<RoomComponent>();
    }

    private void PlaceStartRoom(RoomLayout roomLayout, float transformX, float transformY)
    {
        GameObject roomGo = InstantiateRoom(roomLayout);
        roomGo.AddComponent<RoomComponent>();
        roomGo.GetComponent<RoomComponent>().RoomLayout = roomLayout;
        roomGo.transform.position = new Vector3(transformX, transformY, 0);
        roomGo.SetActive(true);
        _worldMap[startRoom, startRoom] = roomGo;
        _placedRooms.Add(new PlacedRoomInformation { Room = roomGo, x = startRoom, y = startRoom });
    }
    
    private GameObject InstantiateRoom(RoomLayout roomLayout)
    {
        GameObject room = new GameObject(roomLayout.name);
        room.SetActive(false);
        room.AddComponent<RoomComponent>();
        room.GetComponent<RoomComponent>().RoomLayout = roomLayout;
        
        for (int i = 0; i < RoomLayout.RoomHeight; i++)
        {
            for (int j = 0; j < RoomLayout.RoomWidth; j++)
            {
                int currentIndex = i * RoomLayout.RoomWidth + j;
                GameObject tile = new GameObject(roomLayout.Tiles[currentIndex].name)
                {
                    transform =
                    {
                        parent = room.transform,
                        position = new Vector3(j, -i, 0)
                    }
                };
                SpriteRenderer spriteRenderer = tile.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = roomLayout.Tiles[currentIndex].Sprite;
            }
        }
        return room;
    }
    
}

internal static class DirectionHelper
{
    public static Direction GetOppositeDirection(Direction emptyDirection)
    {
        return emptyDirection switch
        {
            Direction.North => Direction.South,
            Direction.East => Direction.West,
            Direction.South => Direction.North,
            Direction.West => Direction.East,
            _ => Direction.None
        };
    }

    public static int GetXValue(int i, Direction placeDirection)
    {
        return placeDirection switch
        {
            Direction.North => i,
            Direction.East => i + 1,
            Direction.South => i,
            Direction.West => i - 1,
            _ => 0
        };
    }

    public static int GetYValue(int i, Direction placeDirection)
    {
        return placeDirection switch
        {
            Direction.North => i + 1,
            Direction.East => i,
            Direction.South => i - 1,
            Direction.West => i,
            _ => 0
        };
    }
    
    public static float GetXValueTransform(float i, Direction placeDirection)
    {
        return placeDirection switch
        {
            Direction.North => i,
            Direction.East => i + RoomLayout.RoomWidth,
            Direction.South => i,
            Direction.West => i - RoomLayout.RoomWidth,
            _ => 0
        };
    }
    
    public static float GetYValueTransform(float i, Direction placeDirection)
    {
        return placeDirection switch
        {
            Direction.North => i + RoomLayout.RoomHeight,
            Direction.East => i,
            Direction.South => i - RoomLayout.RoomHeight,
            Direction.West => i,
            _ => 0
        };
    }
}
