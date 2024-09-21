using System.Collections.Generic;
using UnityEngine;

namespace RoundHero
{
    public interface IMoveGrid
    {
        public Vector3 Position { get; set; }
        
        public int GridPosIdx { get; set; }
    }

    // public interface IUnit
    // {
    //     public EUnitActionState UnitActionState { get; set; }
    //
    //     public void SetAction(EUnitActionState actionState)
    //     {
    //         UnitActionState = actionState;
    //        
    //     }
    //     
    //     public int MaxHP { get; }
    //     
    //     public int CurHP { get; set; }
    //
    //     public void Hurt()
    //     {
    //         SetAction(EUnitActionState.Hurt);
    //     }
    //
    //     public void Recover()
    //     {
    //         SetAction(EUnitActionState.Recover);
    //     }
    //     
    //     public void Attack()
    //     {
    //         SetAction(EUnitActionState.Attack);
    //     }
    //
    //     public void MoveAttack()
    //     {
    //         SetAction(EUnitActionState.MoveAttack);
    //     }
    //     
    //     public void MoveHurt()
    //     {
    //         SetAction(EUnitActionState.MoveHurt);
    //     }
    //
    //     public void Dead()
    //     {
    //         SetAction(EUnitActionState.Dead);
    //     }
    //
    //     public void Dodge()
    //     {
    //         SetAction(EUnitActionState.Dodge);
    //     }
    //
    //     public float Move(List<int> moveGridPosIdxs);
    //     
    //     
    //     public int ID { get; set; }
    //
    //     public Data_BattleUnit BattleUnit { get; set; }
    //
    //     public EUnitCamp UnitCamp { get;}
    //     
    //     public EUnitRole UnitRole { get;}
    //
    //     public void ChangeCurHP(int changeHP, bool useDefense = true, bool addHeroHP = true, bool changeHPInstantly = true);
    //
    //     public void UpdatePos(Vector3 pos);
    //     
    //     public void RefreshData();
    //
    //     public void RefreshDamageState();
    //
    //
    // }

}