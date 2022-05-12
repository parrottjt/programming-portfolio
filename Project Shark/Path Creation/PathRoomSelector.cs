using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EndOceanGen
{
    public class PathRoomSelector
    {
        readonly PathValidation _validation;

        readonly List<EndlessOceanRoom> _placeholderAssets;
        readonly List<EndlessOceanRoom> _endPlaceholderAssets;
        readonly List<EndlessOceanRoom> _bossPlaceholderAssets;

        public PathRoomSelector(PathValidation validation)
        {
            _validation = validation;
            _placeholderAssets = Resources.LoadAll<EndlessOceanRoom>("GridPlaceholderRooms/").ToList();
            foreach (var asset in _placeholderAssets)
            {
                asset.Setup();
            }
            
            _bossPlaceholderAssets = _placeholderAssets.Where(room => room.NumberOfDoors() == 2).ToList();
            _endPlaceholderAssets = _placeholderAssets.Where(room => room.NumberOfDoors() == 1).ToList();
        }

        public EndlessOceanRoom Placeholder(int i,
            List<((int, int) cell, EndlessOceanLevelGeneration.RoomExitDirection direction)> path)
        {
            var sortedList = PlaceholderAssets(i);

            var refinedList = sortedList.Where(room => room.HasDirection(path[i].direction) &&
                                                       _validation.AllDirectionsProduceCellsOnGrid(path[i].cell,
                                                           room.PossibleExitDirections)).ToList();

            var finalList = i == 0 ? refinedList : new List<EndlessOceanRoom>();
            if (finalList.Count == 0)
            {
                finalList.AddRange(refinedList.Where(room =>
                    room.HasDirection(_validation.OppositeDirectionForConnection(path[i - 1].direction))));
            }

            var pathRoom = finalList.Count == 0 ? null : finalList[Random.Range(0, finalList.Count)];

            return pathRoom;
        }

        public EndlessOceanRoom Center(
            List<List<((int, int) cell, EndlessOceanLevelGeneration.RoomExitDirection direction)>> levelPath)
        {
            var directionList = new List<EndlessOceanLevelGeneration.RoomExitDirection>();
            foreach (var direction in levelPath.Select(path => path[path.Count - 1].direction).Where(direction => directionList.Contains(direction) == false))
            {
                directionList.Add(_validation.OppositeDirectionForConnection(direction));
            }

            var roomList = PlaceholderAssets(2).Where(room => room.NumberOfDoors() == directionList.Count);
            EndlessOceanRoom centerRoom = null;
            foreach (var room in roomList)
            {
                var count = directionList.Count(direction => room.HasDirection(direction));
                if (count == directionList.Count)
                {
                    centerRoom = room;
                }
            }

            return centerRoom;
        }

        public EndlessOceanRoom Fill(IEnumerable<EndlessOceanLevelGeneration.RoomExitDirection> directionsToRooms)
        {
            var cellDirections = directionsToRooms.ToList();
            EndlessOceanRoom room = default;

            var filteredAssets = PlaceholderAssets(2).
                Where(asset => asset.NumberOfDoors() == cellDirections.Count);
            
            foreach (var placeholderAsset in filteredAssets)
            {
                var count = cellDirections.Count(direction => placeholderAsset.HasDirection(direction));

                if (count == cellDirections.Count)
                {
                    room = placeholderAsset;
                }
            }

            return room;
        }

        public EndlessOceanRoom Combine(EndlessOceanRoom originalRoom, EndlessOceanRoom extraRoom)
        {
            var directionList = new List<EndlessOceanLevelGeneration.RoomExitDirection>();
            foreach (var direction in _validation.CellDirectionDictionary.Keys)
            {
                if (originalRoom.HasDirection(direction))
                {
                    directionList.Add(direction);
                }
            }

            foreach (var direction in _validation.CellDirectionDictionary.Keys)
            {
                if (extraRoom.HasDirection(direction) && !directionList.Contains(direction))
                {
                    directionList.Add(direction);
                }
            }

            return Fill(directionList);
        }
        
        IEnumerable<EndlessOceanRoom> PlaceholderAssets(int i)
        {
            switch (i)
            {
                case 0:
                    return _endPlaceholderAssets;
                case 1:
                    return _bossPlaceholderAssets;
                default:
                    return _placeholderAssets;
            }
        }
    }
}