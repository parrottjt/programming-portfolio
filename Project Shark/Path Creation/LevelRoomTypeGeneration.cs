using System;
using System.Collections.Generic;
using System.Linq;
using Helpers.Utils;
using Random = UnityEngine.Random;

namespace EndOceanGen
{
    public class LevelRoomTypeGeneration
    {
        Dictionary<(int, int), EndlessOceanRoom.RoomTypes> _roomGenerationDictionary = 
            new Dictionary<(int, int), EndlessOceanRoom.RoomTypes>();
        public Dictionary<(int, int), EndlessOceanRoom.RoomTypes> RoomGenerationDictionary => _roomGenerationDictionary;
        
        public LevelRoomTypeGeneration(List<(int, int)> cellList, IEnumerable<EndlessOceanRoom.RoomTypes> roomTypesAvailable)
        {
            foreach (var cell in cellList)
            {
                if (_roomGenerationDictionary.ContainsKey(cell) == false)
                {
                    _roomGenerationDictionary.Add(cell, EndlessOceanRoom.RoomTypes.Empty);
                }
            }
            
            GenerateRoomTypes(roomTypesAvailable);
        }

        void GenerateRoomTypes(IEnumerable<EndlessOceanRoom.RoomTypes> roomTypesAvailable)
        {
            for (int i = 0; i < _roomGenerationDictionary.Count; i++)
            {
                var key = _roomGenerationDictionary.ElementAt(i).Key;
                _roomGenerationDictionary[key] = SelectRoomType(roomTypesAvailable);
            }
        }
        
        EndlessOceanRoom.RoomTypes SelectRoomType(IEnumerable<EndlessOceanRoom.RoomTypes> roomTypesAvailable)
        {
            var value = Random.Range(0, 300f);
            var weight = 0f;
            foreach (var roomType in roomTypesAvailable)
            {
                weight += RoomTypeWeight(roomType);
                if (value <= weight)
                {
                    return roomType;
                }
            }

            return EndlessOceanRoom.RoomTypes.Enemy;
        }

        float RoomTypeWeight(EndlessOceanRoom.RoomTypes roomType)
        {
            switch (roomType)
            {
               case EndlessOceanRoom.RoomTypes.Enemy:
                   return 1000f;
               case EndlessOceanRoom.RoomTypes.Challenge:
                   return 150f;
               case EndlessOceanRoom.RoomTypes.Treasure:
                   return 25f;
               default:
                   return -50f;
            }
        }
    }
}