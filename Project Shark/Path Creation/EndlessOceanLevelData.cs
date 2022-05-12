using System;
using UnityEngine;

namespace EndOceanGen
{
    [Serializable]
    public class EndlessOceanLevelData
    {
        [SerializeField] EndlessOceanRoom[] _startingRooms;
        [SerializeField] EndlessOceanRoom[] _bossRooms;
        [SerializeField] EndlessOceanRoom[] _endRooms;
        [SerializeField] EndlessOceanRoom[] _levelRooms;
        [SerializeField] EndlessOceanRoom[] _shopRooms;

        public EndlessOceanRoom[] StartingRooms => _startingRooms;
        public EndlessOceanRoom[] BossRooms => _bossRooms;
        public EndlessOceanRoom[] EndRooms => _endRooms;
        public EndlessOceanRoom[] LevelRooms => _levelRooms;

        public void Setup()
        {
            SetupAssets(_startingRooms, _bossRooms, _endRooms, _levelRooms, _shopRooms);
        }
        
        void SetupAssets(params EndlessOceanRoom[][] assetsLists)
        {
            foreach (var assetList in assetsLists)
            {
                foreach (var asset in assetList) 
                {
                    asset.Setup();
                }
            }
        }
        
        
    }
}
