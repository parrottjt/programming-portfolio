using System.Collections.Generic;
using Helpers.Utils;

namespace EndOceanGen
{
    public class PathFill : PathInfo
    {
        readonly PathValidation _validation;
        readonly PathRoomSelector _roomSelector;

        readonly List<((int, int) cell, EndlessOceanRoom room)> _filledRooms =
            new List<((int, int) cell, EndlessOceanRoom room)>();

        public List<((int, int) cell, EndlessOceanRoom room)> FilledRooms => _filledRooms;
        public PathFill(GridObject<EndlessOceanRoom> grid, PathValidation validation) : base(grid)
        {
            _validation = validation;
            _roomSelector = new PathRoomSelector(_validation);
            
            CheckRoomsForEmptyExits();
        }

        void CheckRoomsForEmptyExits()
        {
            for (int x = 0; x < Grid.Width; x++)
            {
                for (int y = 0; y < Grid.Height; y++)
                {
                    var cell = (x, y);
                    if(cell == Grid.CenterCell()) continue;

                    var room = Grid.GetValue(cell);
                    
                    if(room.IsNull()) continue;
                    if(room.NumberOfDoors() <= 2) continue;
                    
                    CheckRoomExits(cell, room);
                }
            }

            FillEmptyRooms();
        }
        
        void CheckRoomExits((int, int) cell, EndlessOceanRoom room)
        {
            foreach (var exitDirection in room.PossibleExitDirections)
            {
                if (_validation.NextCellIsValid(cell, exitDirection, out var nextCell))
                {
                    var directions = RoomExitDirections(nextCell);
                    _filledRooms.Add((nextCell, _roomSelector.Fill(directions)));
                }
            }
        }
        
        void FillEmptyRooms()
        {
            foreach (var cellRoomPair in _filledRooms)
            {
                Grid.SetValue(cellRoomPair.cell, cellRoomPair.room);
            }
        }

        IEnumerable<EndlessOceanLevelGeneration.RoomExitDirection> RoomExitDirections((int,int) cell)
        {
            var directions = new List<EndlessOceanLevelGeneration.RoomExitDirection>();

            foreach (var direction in _validation.CellDirectionDictionary.Keys)
            {
                if (!_validation.NextCellInGrid(cell, direction, out var cellToCheck)) continue;
                var room = Grid.GetValue(cellToCheck);
                if (room == null) continue;
                if (room.HasDirection(_validation.OppositeDirectionForConnection(direction)))
                {
                    directions.Add(direction);
                }
            }
            
            return directions;
        }
    }
}