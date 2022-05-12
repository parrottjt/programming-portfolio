using System.Collections.Generic;
using UnityEngine;

namespace EndOceanGen
{
    public class LevelPath : PathInfo
    {
        readonly PathValidation _validation;
        readonly List<((int, int), EndlessOceanLevelGeneration.RoomExitDirection)> _path;

        public List<((int, int), EndlessOceanLevelGeneration.RoomExitDirection)> Path => _path;

        public LevelPath(GridObject<EndlessOceanRoom> gridObject, (int,int) startingCell) : base(gridObject)
        {
            _validation = new PathValidation(gridObject);
            _path = new List<((int, int), EndlessOceanLevelGeneration.RoomExitDirection)>();

            CreatePath(startingCell);
        }

        void CreatePath((int,int) startingCell)
        {
            CreatePathRoom(startingCell, out var nextCell);
            while (nextCell != (-1,-1))
            {
                CreatePathRoom(nextCell, out nextCell);
            }
        }

        void CreatePathRoom((int,int) startingCell, out (int,int) nextCell)
        {
            var directionList = DirectionsToCenter(startingCell);
            var direction = directionList[Random.Range(0, directionList.Count)];
            
            _path.Add((startingCell, direction));
            
            nextCell = FindNextCellInPath(startingCell, direction);
        }

        List<EndlessOceanLevelGeneration.RoomExitDirection> DirectionsToCenter((int x, int y) cell)
        {
            var directionList = new List<EndlessOceanLevelGeneration.RoomExitDirection>();
            var (x, y) = Grid.CenterCell();
            if (cell.x < x) directionList.Add(EndlessOceanLevelGeneration.RoomExitDirection.East);
            if (cell.x > x) directionList.Add(EndlessOceanLevelGeneration.RoomExitDirection.West);
            if (cell.y < y) directionList.Add(EndlessOceanLevelGeneration.RoomExitDirection.North);
            if (cell.y > y) directionList.Add(EndlessOceanLevelGeneration.RoomExitDirection.South);
            return directionList;
        }

        (int, int) FindNextCellInPath((int, int) currentCell,
            EndlessOceanLevelGeneration.RoomExitDirection direction)
        {
            var nextCell = (-2,-2);
            while (nextCell == (-2,-2))
            {
                if (_validation.NextCellIsValid(currentCell, direction, out var cell))
                {
                    nextCell = cell;
                    Debug.Log($"Cell_{nextCell} , Direction_{direction}");
                }

                if (_validation.CellIsCenterCell(cell))
                {
                    nextCell = (-1, -1);
                }
            }

            return nextCell;
        }
    }
}
