using System.Collections.Generic;
using Helpers.Utils;
using UnityEngine;

namespace EndOceanGen
{
    public class PlaceholderRoomReplacement : PathInfo
    {
        public PlaceholderRoomReplacement(GridObject<EndlessOceanRoom> grid) : base(grid) { }

        public void ReplaceRoom((int,int) cell, IEnumerable<EndlessOceanRoom> possibleReplacements)
        {
            var currentCellValue = Grid.GetValue(cell);
            var replacement = new List<EndlessOceanRoom>();

            if (currentCellValue.IsNull())
            {
                Debug.Log($"Cell is null at {cell}\n Check cell input cell should NEVER be null!");
                return;
            }
            
            foreach (var possibleReplacement in possibleReplacements)
            {
                if (HasSameExits(currentCellValue, possibleReplacement))
                {
                    replacement.Add(possibleReplacement);
                }
            }

            if (replacement.Count != 0)
            {
                var selectedRoom = replacement[Random.Range(0, replacement.Count)];
                selectedRoom.SetRoomExits(currentCellValue.PossibleExitDirections);
                Grid.SetValue(cell, selectedRoom);
            }
            else
            {
                Debug.Log($"{cell} can not find replacement for {currentCellValue}");
            }
        }

        bool HasSameExits(EndlessOceanRoom room, EndlessOceanRoom replacementRoom)
        {
            return replacementRoom.RoomHasExitDirections(room.PossibleExitDirections);
        }
    }
}
