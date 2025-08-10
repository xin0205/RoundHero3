using UnityEngine;

namespace RoundHero
{
    public enum EBattleIconType
    {
        Collision,
        Empty,
    }
    
    public class BattleIconEntityData : EntityData
    {
        public EBattleIconType BattleIconType = EBattleIconType.Empty;
        //public EUnitState UnitState = EUnitState.Empty;
        public int EntityIdx;
        
        
        public void Init(int entityId, Vector3 pos, EBattleIconType battleIconType, int entityIndex)
        {
            base.Init(entityId, pos);
            this.BattleIconType = battleIconType;
            this.EntityIdx = entityIndex;

        }

    }
}