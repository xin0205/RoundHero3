using System.Collections.Generic;
using GameFramework;
using UnityEngine;

namespace RoundHero
{
    public class BulletData
    {
        public List<int> MoveGridPosIdxs;
        public GameFrameworkMultiDictionary<int, TriggerData> TriggerDataDict = new GameFrameworkMultiDictionary<int, TriggerData>();
    }
    
    public class BattleBulletEntityData : EntityData
    {

        public BulletData BulletData;
        
        public void Init(int entityId, Vector2 pos, BulletData bulletData)
        {
            base.Init(entityId, pos);
            BulletData = bulletData;
            
        }
    }
}