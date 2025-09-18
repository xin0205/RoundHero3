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
        //public EAttackCastType AttackCastType;
        public BuffValue BuffValue;
        public EAttackCastType AttackCastType;
        public bool IsStatic;

        //int value, 
        public void Init(int entityId, Vector3 pos, Vector3 startPos, Vector3 targetPos, EAttackTagType attackTagType,
            EUnitState unitState, BuffValue buffValue, EAttackCastType attackCastType, int entityIdx = -1,
            bool showAttackLine = true, bool showAttackPos = true, bool isStatic = false)
        {
            base.Init(entityId, pos);
            this.StartPos = startPos;
            this.TargetPos = targetPos;
            //this.Value = value;
            this.EntityIdx = entityIdx;
            ShowAttackLine = showAttackLine;
            ShowAttackPos = showAttackPos;
            BuffValue = buffValue;
            AttackCastType = attackCastType;
            IsStatic = isStatic;
        }

    }
}