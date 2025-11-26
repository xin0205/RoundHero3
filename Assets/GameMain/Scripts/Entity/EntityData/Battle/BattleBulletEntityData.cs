using System.Collections.Generic;
using GameFramework;
using UnityEngine;

namespace RoundHero
{
    public class BulletData
    {
        public int ActionUnitIdx = -1;
        public int EffectUnitIdx = -1;
        public List<int> MoveGridPosIdxs = new List<int>();
        public GameObject ActionUnitGO;
        public GameObject EffectUnitGO;
        public EColor EffectColor;
        public TriggerCollection TriggerCollections =
            new();
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