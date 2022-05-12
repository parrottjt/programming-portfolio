using System;
using System.Collections.Generic;
using Helpers.Utils;
using UnityEngine;

namespace EndOceanGen
{
    public class PathValidation : PathInfo
    {
       public readonly Dictionary<EndlessOceanLevelGeneration.RoomExitDirection, (int x, int y)> CellDirectionDictionary =
                new Dictionary<EndlessOceanLevelGeneration.RoomExitDirection, (int x, int y)>
                {
                    { EndlessOceanLevelGeneration.RoomExitDirection.North, (0, 1) },
                    { EndlessOceanLevelGeneration.RoomExitDirection.East, (1, 0) },
                    { EndlessOceanLevelGeneration.RoomExitDirection.South, (0, -1) },
                    { EndlessOceanLevelGeneration.RoomExitDirection.West, (-1, 0) },
                };
        public PathValidation(GridObject<EndlessOceanRoom> grid) : base(grid) { }

        public bool CellIsCenterCell((int,int) cell) => cell == Grid.CenterCell();

        public bool NextCellInGrid((int,int) cell, EndlessOceanLevelGeneration.RoomExitDirection roomExitDirection, 
            out (int,int) nextCell)
        {
            nextCell = NextCellFromDirection(cell, roomExitDirection);
            return Grid.CellInGrid(nextCell);
        }

        public bool NextCellIsValid((int, int) cell, EndlessOceanLevelGeneration.RoomExitDirection roomExitDirection, 
            out (int,int) nextCell)
        {
            nextCell = NextCellFromDirection(cell, roomExitDirection);
            return Grid.CellValid(nextCell);
        }

        public bool NextCellHasDoorConnecting((int,int) cell, EndlessOceanLevelGeneration.RoomExitDirection roomExitDirection,
            out (int,int) nextCell)
        {
            nextCell = NextCellFromDirection(cell, roomExitDirection);
            var room = Grid.GetValue(nextCell);

            return !room.IsNull() && room.HasDirection(OppositeDirectionForConnection(roomExitDirection));
        }
        
        public EndlessOceanLevelGeneration.RoomExitDirection OppositeDirectionForConnection(
            EndlessOceanLevelGeneration.RoomExitDirection direction)
        {
            switch (direction)
            {
                case EndlessOceanLevelGeneration.RoomExitDirection.North:
                    return EndlessOceanLevelGeneration.RoomExitDirection.South;
                case EndlessOceanLevelGeneration.RoomExitDirection.East:
                    return EndlessOceanLevelGeneration.RoomExitDirection.West;
                case EndlessOceanLevelGeneration.RoomExitDirection.South:
                    return EndlessOceanLevelGeneration.RoomExitDirection.North;
                case EndlessOceanLevelGeneration.RoomExitDirection.West:
                    return EndlessOceanLevelGeneration.RoomExitDirection.East;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public bool AllDirectionsProduceCellsOnGrid((int, int) cell, List<EndlessOceanLevelGeneration.RoomExitDirection> roomExits)
        {
            var value = true;

            foreach (var roomExit in roomExits)
            {
                if (NextCellInGrid(cell, roomExit, out _) == false)
                {
                    value = false;
                    break;
                }
            }
            
            return value;
        }
        
        public List<(int, int)> ValidCellsInGridWithinTransitionRange((int min, int max) transitions)
        {
            var validCells = new List<(int, int)>();
            for (int x = 0; x < Grid.Width; x++)
            {
                for (int y = 0; y < Grid.Height; y++)
                {
                    if (NumberOfTransitionsToGoal(Grid.CenterCell(), (x, y), transitions)
                        && Grid.GetValue((x, y)) == false)
                    {
                        validCells.Add((x, y));
                    }
                }
            }

            return validCells;
        }
        
        (int x, int y) NextCellFromDirection((int x, int y) cell,
            EndlessOceanLevelGeneration.RoomExitDirection roomExitDirection)
        {
            var (x, y) = CellDirectionDictionary[roomExitDirection];
            var nextCellX = cell.x + x;
            var nextCellY = cell.y + y;
            var nextCell = (nextCellX, nextCellY);
            return nextCell;
        }
        
        bool NumberOfTransitionsToGoal((int X, int Y) initial, (int X, int Y) goal, (int, int) allowedTransitionRange)
        {
            var x = Mathf.Abs(goal.X - initial.X);
            var y = Mathf.Abs(goal.Y - initial.Y);
            var transitions = x + y;
            var valid = Conditions.ValueWithinRange(transitions, allowedTransitionRange);
            return valid;
        }
    }
}
