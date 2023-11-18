using System;
using System.Collections.Generic;
using Room;
using Unity.VisualScripting;
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
    private const int MaxWorldSize = 10;

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
        PlaceStartRoom(_roomsToPlace.Pop(), -4, 2);
        while (_roomsToPlace.Count > 0)
        {
            // get room from rooms to place
            GameObject roomGo = InstantiateRoom(_roomsToPlace.Pop());
            
            // get random other room
            PlacedRoomInformation adjacentRoomInfo = _placedRooms[Random.Range(0, _placedRooms.Count)];
            RoomComponent adjacentRoomComponent = adjacentRoomInfo.Room.GetComponent<RoomComponent>() ?? throw new System.Exception("Room is null");
            
            // get empty space from other room
            Direction placeDirection = adjacentRoomComponent.GetRandomEmptyNeighborDirection();

            // place room in empty space
            adjacentRoomComponent.Neighbors[placeDirection] = roomGo.GetComponent<RoomComponent>().RoomLayout;
            roomGo.GetComponent<RoomComponent>().Neighbors[DirectionHelper.GetOppositeDirection(placeDirection)] = adjacentRoomComponent.RoomLayout;
            
            
            // add room to world map array
            // only set one of them because shift only one direction
            var x = adjacentRoomInfo.x;
            var y = adjacentRoomInfo.y;
            
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
            
            // set room position
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
            
            // set room active
            roomGo.SetActive(true);
            
            // add room to placed rooms
            _placedRooms.Add(new PlacedRoomInformation
            {
                Room = roomGo,
                x = x,
                y = y
            });
            

            // RoomLayout roomLayout = _roomsToPlace.Pop();
            // InstantiateRoom(roomLayout);
            // Direction emptyDirection = Direction.None;
            // RoomLayout adjacentRoomLayout = null;
            // while (emptyDirection == Direction.None)
            // {
            //     adjacentRoomLayout = AdjacentRoom();
            //     
            //     // emptyDirection = adjacentRoomLayout.GetRandomEmptyNeighborDirection();
            // }

        }
        
        // PlaceBossRoom();
    }

    private RoomLayout AdjacentRoom()
    {
        GameObject adjacentRoomGo = _placedRooms[Random.Range(0, _placedRooms.Count)].Room;
        RoomLayout adjacentRoomLayout = adjacentRoomGo.GetComponent<RoomLayout>() ?? throw new System.Exception("Room is null");
        return adjacentRoomLayout;
    }

    private void PlaceBossRoom()
    {
        // TODO throw new System.NotImplementedException();
    }

    private void PlaceStartRoom(RoomLayout roomLayout, float transformX, float transformY)
    {
        GameObject roomGo = InstantiateRoom(roomLayout);
        roomGo.AddComponent<RoomComponent>();
        roomGo.GetComponent<RoomComponent>().RoomLayout = roomLayout;
        roomGo.transform.position = new Vector3(transformX, transformY, 0);
        roomGo.SetActive(true);
        int x = Random.Range(0, MaxWorldSize);
        int y = Random.Range(0, MaxWorldSize);
        _worldMap[x, y] = roomGo;
        _placedRooms.Add(new PlacedRoomInformation { Room = roomGo, x = x, y = y });
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
                int currentIndex = (i * RoomLayout.RoomWidth + j);
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

internal class DirectionHelper
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
    
    public static int GetUnitsToPlaceInDirection(Direction direction)
    {
        return direction switch
        {
            Direction.North => RoomLayout.RoomHeight,
            Direction.East => RoomLayout.RoomWidth,
            Direction.South => RoomLayout.RoomHeight,
            Direction.West => RoomLayout.RoomWidth,
            _ => 0
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
