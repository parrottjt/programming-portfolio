using System.Collections.Generic;
using System.Linq;
using Helpers.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace EndOceanGen
{
    public class EndlessOceanLevelGeneration : MonoBehaviour
    {
        public enum RoomExitDirection
        {
            North,
            East,
            South,
            West,
            None
        }

        Seed _currentSeed;

        [SerializeField] GridSize _gridSize;
        [Range(0, 3)] [SerializeField] int _numberOfExtraPaths;

        [SerializeField] EndlessOceanLevelData _levelData;

        [SerializeField] bool _runDebug;

        [DrawIf("_runDebug", true)] [SerializeField]
        int _numberOfIterations = 25;

        List<((int, int), EndlessOceanRoom)> _currentRoomList;

        GridObject<EndlessOceanRoom> _mapGeneration;
        PathValidation _validation;
        PathCreation _pathCreation;
        PlaceholderRoomReplacement _roomReplacement;
        PathFill _pathFill;

        int _roomLimit;
        LevelRoomTypeGeneration _levelRoomGeneration;

        void Awake()
        {
            LevelGenerationSetup();
            LevelGeneration();
            LevelGenerationCleanup();
        }

        void LevelGenerationSetup()
        {
            _levelData.Setup();

            //todo Find out if this is needed
            _roomLimit = _gridSize.GridDimensions +
                         (_gridSize.GridDimensions * _numberOfExtraPaths) +
                         (_gridSize.GridDimensions / 2);
        }

        void LevelGeneration()
        {
            for (int i = 0; i < (_runDebug ? _numberOfIterations : 1); i++)
            {
                CreateGrid(transform);

                _roomReplacement = new PlaceholderRoomReplacement(_mapGeneration);
                _validation = new PathValidation(_mapGeneration);
                _pathCreation = new PathCreation(_gridSize, _mapGeneration, _validation, _numberOfExtraPaths);

                BuildPathInGrid();

                _pathFill = new PathFill(_mapGeneration, _validation);

                ReplaceRooms();

                print($"Grid Creation Pass {i} Completed");
            }

            print("Done");
        }

        void CreateGrid(Transform objTransform)
        {
            var children = objTransform.GetComponentsInChildren<TextMesh>();
            if (children.Length > 0)
            {
                foreach (var child in children)
                {
                    Destroy(child.gameObject);
                }
            }

            _mapGeneration = new GridObject<EndlessOceanRoom>(_gridSize.GridDimensions, _gridSize.GridDimensions,
                _gridSize.CellSize,
                objTransform.position, objTransform, true, 64);
        }

        void BuildPathInGrid()
        {
            var roomSelector = new PathRoomSelector(_validation);

            foreach (var path in _pathCreation.Paths)
            {
                foreach (var (cell, room) in path)
                {
                    var cellValue = _mapGeneration.GetValue(cell);
                    _mapGeneration.SetValue(cell, cellValue.IsNull() ? room : roomSelector.Combine(cellValue, room));
                }
            }
        }

        void ReplaceRooms()
        {
            ReplaceStaticRooms();

            ReplaceRemainingCells();

            _currentRoomList = RoomsInGrid();
            _currentSeed = SeedGeneration.CreateSeed(_currentRoomList);
        }

        void ReplaceStaticRooms()
        {
            ReplaceEndRoom();

            ReplaceBossRoom();

            ReplaceStartingRoom();
        }

        void ReplaceStartingRoom()
        {
            var cell = _pathCreation.Paths[_pathCreation.Paths.Count - 1].FirstOrDefault().cell;
            _roomReplacement.ReplaceRoom(cell, _levelData.StartingRooms);
        }

        void ReplaceBossRoom()
        {
            var cell = _pathCreation.Paths[0][1].cell;
            _roomReplacement.ReplaceRoom(cell, _levelData.BossRooms);
        }

        void ReplaceEndRoom()
        {
            var cell = _pathCreation.Paths[0][0].cell;
            _roomReplacement.ReplaceRoom(cell, _levelData.EndRooms);
        }

        void ReplaceRemainingCells()
        {
            var cellList = RemainingCellToReplace();
            var levelRoomTypesAvailable = LevelRoomTypesAvailable();
            levelRoomTypesAvailable.Sort();

            _levelRoomGeneration = new LevelRoomTypeGeneration(cellList, levelRoomTypesAvailable);

            foreach (var cellTypePair in _levelRoomGeneration.RoomGenerationDictionary)
            {
                var filterRooms = _levelData.LevelRooms.Where(room => room.RoomType == cellTypePair.Value);
                _roomReplacement.ReplaceRoom(cellTypePair.Key, filterRooms);
            }
        }

        List<EndlessOceanRoom.RoomTypes> LevelRoomTypesAvailable()
        {
            var levelRoomTypesAvailable = new List<EndlessOceanRoom.RoomTypes>();
            foreach (var levelRoom in _levelData.LevelRooms)
            {
                if (levelRoomTypesAvailable.Contains(levelRoom.RoomType) == false)
                {
                    levelRoomTypesAvailable.Add(levelRoom.RoomType);
                }
            }

            return levelRoomTypesAvailable;
        }

        List<(int, int)> RemainingCellToReplace()
        {
            var cellList = new List<(int, int)>();
            for (int index1 = 0; index1 < _pathCreation.Paths.Count - 1; index1++)
            {
                for (int index2 = 0; index2 < _pathCreation.Paths[index1].Count; index2++)
                {
                    if (index1 == 0)
                    {
                        if (index2 == 0 || index2 == 1)
                        {
                            continue;
                        }
                    }

                    cellList.Add(_pathCreation.Paths[index1][index2].cell);
                }
            }

            cellList.AddRange(_pathFill.FilledRooms.Select(cellRoomPair => cellRoomPair.cell));

            return cellList;
        }

        List<((int, int), EndlessOceanRoom)> RoomsInGrid()
        {
            var list = new List<((int, int), EndlessOceanRoom)>();
            for (int x = 0; x < _mapGeneration.Width; x++)
            {
                for (int y = 0; y < _mapGeneration.Height; y++)
                {
                    var cell = (x, y);
                    var cellValue = _mapGeneration.GetValue(cell);
                    if (cellValue.IsNotNull()) list.Add((cell, cellValue));
                }
            }

            return list;
        }

        void LevelGenerationCleanup()
        {
            ForceShopCreation();
            
            var roomExitDirectionsList = new List<((int,int), List<RoomExitDirection>)>();
            foreach (var cellRoomPair in _currentRoomList)
            {
                var (cell, room) = cellRoomPair;
                if(_validation.CellIsCenterCell(cell)) continue;
                var roomExits = room.ActualRoomExitDirections;
                foreach (var direction in _validation.CellDirectionDictionary.Keys)
                {
                    if(room.NumberOfDoors() <= 2) continue;
                    if (_validation.NextCellHasDoorConnecting(cell, direction, out var nextCell) == false)
                    {
                        if (_validation.NextCellInGrid(cell, direction, out _))
                        {
                            roomExits.Remove(direction);
                        }
                    }
                }
                roomExitDirectionsList.Add((cell, roomExits));
            }

            foreach (var (cell, roomExitDirections) in roomExitDirectionsList)
            {
                var room = _mapGeneration.GetValue(cell);
                room.SetRoomExits(roomExitDirections);
            }
        }

        void ForceShopCreation()
        {
            var validCells =
                _validation.ValidCellsInGridWithinTransitionRange((_gridSize.GridDimensions / 2,
                    _gridSize.GridDimensions / 2));

            var filteredCells = new List<(int, int)>();
            foreach (var validCell in validCells)
            {
                var value = _mapGeneration.GetValue(validCell);
                if (value.IsNotNull())
                {
                    if(value.RoomType != EndlessOceanRoom.RoomTypes.Boss) filteredCells.Add(validCell);
                }
            }

            var chosenCell = filteredCells[Random.Range(0, filteredCells.Count)];
            var shopList = _levelData.LevelRooms.
                Where(room => room.RoomType == EndlessOceanRoom.RoomTypes.Shop);
            _roomReplacement.ReplaceRoom(chosenCell, shopList);
        }
    }
}