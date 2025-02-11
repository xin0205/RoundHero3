using System.Collections.Generic;
using GameFramework;
using UnityEngine;

namespace RoundHero
{
    public class BulletData
    {
        public int ActionUnitID = -1;
        public List<int> MoveGridPosIdxs = new List<int>();
        public GameFrameworkMultiDictionary<int, ITriggerActionData> TriggerActionDataDict = new ();
    }
    
    public class BattleBulletEntityData : EntityData
    {

        public BulletData BulletData;
        
        public void Init(int entityId, Vector3 pos, BulletData bulletData)
        {
            base.Init(entityId, pos);
            BulletData = bulletData;
            
        }
    }
}