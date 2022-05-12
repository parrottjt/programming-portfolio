using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace EndOceanGen
{
    public class PathCreation : PathInfo
    {
        readonly Seed _seed;
        readonly GridSize _gridSize;
        readonly PathValidation _validation;
        readonly PathRoomSelector _roomSelector;
        readonly int _numberOfExtraPaths;

        readonly List<List<((int, int), EndlessOceanRoom)>> _paths = new List<List<((int, int), EndlessOceanRoom)>>();
        readonly List<List<((int, int), EndlessOceanLevelGeneration.RoomExitDirection)>> _levelPath;
        
        public List<List<((int, int) cell, EndlessOceanRoom room)>> Paths => _paths;
        
        public PathCreation(GridSize gridSize, GridObject<EndlessOceanRoom> grid, PathValidation validation, int numberOfExtraPaths) : 
            base(grid)
        {
            _gridSize = gridSize;
            _numberOfExtraPaths = numberOfExtraPaths;
            _validation = validation;
            _roomSelector = new PathRoomSelector(_validation);
            _levelPath = new List<List<((int, int), EndlessOceanLevelGeneration.RoomExitDirection)>>();

            CreatePaths();
        }

        void CreatePaths()
        {
            CreatePath(CellPositionToStartPath((_gridSize.GridDimensions - 1, _gridSize.GridDimensions - 1)), true);
            //Side Path - can be set to a loop for extra needed
            for (int i = 0; i < _numberOfExtraPaths; i++)
            {
                CreatePath(CellPositionToStartPath((_gridSize.GridDimensions - 2, _gridSize.GridDimensions - 2)),
                    false);
            }

            PlaceCorrectPlaceholder();
        }

        void CreatePath((int x, int y) currentCell, bool hasBossRoom)
        {
            _levelPath.Add(new LevelPath(Grid, currentCell).Path);
        }

        void PlaceCorrectPlaceholder()
        {
            foreach (var path in _levelPath)
            {
                _paths.Add(PlaceholderRoomsForPath(path));
            }

            _paths.Add(new List<((int, int), EndlessOceanRoom)> { CenterRoom() });
        }

        List<((int, int), EndlessOceanRoom)> PlaceholderRoomsForPath(
            List<((int, int) cell, EndlessOceanLevelGeneration.RoomExitDirection direction)> path)
        {
            var pathRoom = new List<((int, int), EndlessOceanRoom)>();
            for (int i = 0; i < path.Count; i++)
            {
                pathRoom.Add((path[i].cell, _roomSelector.Placeholder(i, path)));
            }

            return pathRoom;
        }

        ((int, int), EndlessOceanRoom) CenterRoom() => (Grid.CenterCell(), _roomSelector.Center(_levelPath));

        (int x, int y) CellPositionToStartPath((int min, int max) transitions)
        {
            var validCells = _validation.ValidCellsInGridWithinTransitionRange(transitions);

            var cellIndex = Random.Range(0, validCells.Count);
            return validCells[cellIndex];
        }
    }
}