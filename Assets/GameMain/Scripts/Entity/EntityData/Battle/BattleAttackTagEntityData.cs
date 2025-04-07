using UnityEngine;

namespace RoundHero
{
    public enum EAttackTagType
    {
        Attack,
        Recover,
        UnitState,
    }
    
    public class BattleAttackTagEntityData : EntityData
    {
        //public int Value;
        public Vector3 StartPos;
        public Vector3 TargetPos;
        public int EntityIdx;
        public bool ShowAttackLine = true;
        public bool ShowAttackPos = true;
        public EAttackTagType AttackTagType;
        public EUnitState UnitState;


        //int value, 
        public void Init(int entityId, Vector3 pos, Vector3 startPos, Vector3 targetPos, EAttackTagType attackTagType,
            EUnitState unitState, int entityIdx = -1, bool showAttackLine = true, bool showAttackPos = true)
        {
            base.Init(entityId, pos);
            this.StartPos = startPos;
            this.TargetPos = targetPos;
            //this.Value = value;
            this.EntityIdx = entityIdx;
            ShowAttackLine = showAttackLine;
            ShowAttackPos = showAttackPos;
        }

    }
}