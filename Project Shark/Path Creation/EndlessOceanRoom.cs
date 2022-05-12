using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Math = Helpers.Utils.Math;

namespace EndOceanGen
{
    [CreateAssetMenu(menuName = "Game Data/Endless Ocean Rooms/Placeholder Room", fileName = "Placeholder")]
    public class EndlessOceanRoom : ScriptableObject
    {
        [Serializable]
        public struct RoomExit
        {
            [SerializeField] bool north;
            [SerializeField] bool west;
            [SerializeField] bool south;
            [SerializeField] bool east;

            public bool North => north;
            public bool West => west;
            public bool South => south;
            public bool East => east;
        }

        public enum RoomTypes
        {
            Empty,
            Starter,
            Boss,
            End,
            Shop,
            Treasure = 15,
            Challenge = 30,
            Enemy = 50,
        }

        [SerializeField] RoomTypes _roomType;

        [SerializeField] GameObject _roomPrefab;
        [FormerlySerializedAs("_roomExits")] [SerializeField] RoomExit _possibleRoomExits;
        List<EndlessOceanLevelGeneration.RoomExitDirection> _possibleExitDirections;
        
        List<List<EndlessOceanLevelGeneration.RoomExitDirection>> _roomExitLists;
        List<EndlessOceanLevelGeneration.RoomExitDirection> _actualRoomExitDirections;
        
        public GameObject RoomPrefab => _roomPrefab;
        public RoomTypes RoomType => _roomType;
        
        public List<EndlessOceanLevelGeneration.RoomExitDirection> ActualRoomExitDirections => _actualRoomExitDirections;
        public List<EndlessOceanLevelGeneration.RoomExitDirection> PossibleExitDirections => _possibleExitDirections;
        public int NumberOfDoors()
        {
            var value = 0;
            if (_possibleRoomExits.North) value += 1;
            if (_possibleRoomExits.West) value += 1;
            if (_possibleRoomExits.South) value += 1;
            if (_possibleRoomExits.East) value += 1;

            return value;
        }

        public bool HasDirection(EndlessOceanLevelGeneration.RoomExitDirection roomExitDirection)
        {
            return _possibleExitDirections.Contains(roomExitDirection);
        }

        public bool RoomHasExitDirections(List<EndlessOceanLevelGeneration.RoomExitDirection> exitDirections)
        {
            var count = exitDirections.Count(exitDirection => _possibleExitDirections.Contains(exitDirection));

            return count == exitDirections.Count;
        }

        public void Setup()
        {
            _possibleExitDirections = CreateExitDirections();
            CreatePossibleRoomExits();
            SetRoomExits(_possibleExitDirections);
        }
        
        void CreatePossibleRoomExits()
        {
            _roomExitLists = PossibleRoomExits(_possibleExitDirections);
        }

        public void SetRoomExits(List<EndlessOceanLevelGeneration.RoomExitDirection> exitDirections)
        {
            var orderedDirections = exitDirections.OrderBy(direction => direction).ToList();
            _actualRoomExitDirections = orderedDirections;
        }
        
        List<EndlessOceanLevelGeneration.RoomExitDirection> CreateExitDirections()
        {
            var exitDirections = new List<EndlessOceanLevelGeneration.RoomExitDirection>();
            if (_possibleRoomExits.North) exitDirections.Add(EndlessOceanLevelGeneration.RoomExitDirection.North);
            if (_possibleRoomExits.East) exitDirections.Add(EndlessOceanLevelGeneration.RoomExitDirection.East);
            if (_possibleRoomExits.South) exitDirections.Add(EndlessOceanLevelGeneration.RoomExitDirection.South);
            if (_possibleRoomExits.West) exitDirections.Add(EndlessOceanLevelGeneration.RoomExitDirection.West);
            return exitDirections;
        }

        List<List<EndlessOceanLevelGeneration.RoomExitDirection>> PossibleRoomExits(
            List<EndlessOceanLevelGeneration.RoomExitDirection> roomExitDirections)
        {
            var exitDirectionsList = new List<List<EndlessOceanLevelGeneration.RoomExitDirection>>();
            
            //First will always be the list that is return
            exitDirectionsList.Add(roomExitDirections);
            
            var n = roomExitDirections.Count;
            for (int k = roomExitDirections.Count - 1; k >= 2; k--)
            {
                var numberOfCombinations = Math.Factorial(n) / 
                                           (Math.Factorial(k) * Math.Factorial(n - k));
                for (int i = 0; i < numberOfCombinations; i++)
                {
                    for (int j = 0; j < roomExitDirections.Count; j++)
                    {
                        var exitDirections = new List<EndlessOceanLevelGeneration.RoomExitDirection>();
                        for (int l = 0; l < k; l++)
                        {
                            var index = j + l;
                            if (index >= roomExitDirections.Count)
                            {
                                index = 0;
                            }
                            if(exitDirections.Contains(roomExitDirections[index]) == false)
                                exitDirections.Add(roomExitDirections[index]);
                        }
                        if(exitDirectionsList.Contains(exitDirections) == false) 
                            exitDirectionsList.Add(exitDirections.OrderBy(direction => direction).ToList());
                    }
                    
                }
            }
            
            //Last n Lists will always have singular directions 
            foreach (var roomExitDirection in roomExitDirections)
            {
                var lastList = new List<EndlessOceanLevelGeneration.RoomExitDirection> { roomExitDirection };
                exitDirectionsList.Add(lastList);
            }
            
            return exitDirectionsList;
        }
        
        public override string ToString()
        {
            string north = _actualRoomExitDirections.Contains(EndlessOceanLevelGeneration.RoomExitDirection.North) 
                ? "North" : "";
            string west = _actualRoomExitDirections.Contains(EndlessOceanLevelGeneration.RoomExitDirection.West)
                ? "West" : "";
            string south = _actualRoomExitDirections.Contains(EndlessOceanLevelGeneration.RoomExitDirection.South)
                ? "South" : "";
            string east = _actualRoomExitDirections.Contains(EndlessOceanLevelGeneration.RoomExitDirection.East)
                ? "East" : "";
            
            return $"{_roomType.ToString()} has room exits at \n{north} {west} {south} {east}";
        }
    }
}