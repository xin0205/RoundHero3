using System.Collections.Generic;

namespace RoundHero
{
    public class BattleUnitStateManager : Singleton<BattleUnitStateManager>
    {
        public void RoundEndTrigger()
        {
            AutoSubRoundEndTrigger();
            HurtEachMoveRoundEndTrigger();

        }
        
        public void RoundStartTrigger()
        {
            foreach (var kv in BattleUnitManager.Instance.BattleUnitDatas)
            {
                AddActiveAttack(kv.Value);

            }
        }

        public void AddActiveAttack(Data_BattleUnit unit)
        {
            BattleUnitManager.Instance.GetBuffValue(GamePlayManager.Instance.GamePlayData, unit, out List<BuffValue> triggerBuffDatas);

            var triggerBuffData = triggerBuffDatas.Find(triggerBuffData =>
                triggerBuffData.BuffData.BuffTriggerType == EBuffTriggerType.ActiveAttack);

            if (triggerBuffData != null)
            {
                unit.UnitState.AddState(EUnitState.ActiveAtk);
            }
        }

        public void AutoSubRoundEndTrigger()
        {
            foreach (var kv in BattleUnitManager.Instance.BattleUnitDatas)
            {
                // if (kv.Value.GetStateCount(EUnitState.HurtAddDamage) > 0)
                // {
                //     kv.Value.RemoveState(EUnitState.HurtAddDamage);
                // }
                
                // if (kv.Value.GetStateCount(EUnitState.SubDamage) > 0)
                // {
                //     kv.Value.RemoveState(EUnitState.SubDamage);
                // }
                
                // if (kv.Value.GetStateCount(EUnitState.Dodge) > 0)
                // {
                //     kv.Value.RemoveState(EUnitState.Dodge);
                // }
                
                if (kv.Value.GetStateCount(EUnitState.UnHurt) > 0)
                {
                    kv.Value.RemoveState(EUnitState.UnHurt);
                }
                
                // if (kv.Value.GetStateCount(EUnitState.UnAction) > 0)
                // {
                //     kv.Value.RemoveState(EUnitState.UnAction);
                // }
                
                if (kv.Value.GetStateCount(EUnitState.UnAtk) > 0)
                {
                    kv.Value.RemoveState(EUnitState.UnAtk);
                }
                
                if (kv.Value.GetStateCount(EUnitState.UnRecover) > 0)
                {
                    kv.Value.RemoveState(EUnitState.UnRecover);
                }
                
                if (kv.Value.GetStateCount(EUnitState.UnMove) > 0)
                {
                    kv.Value.RemoveState(EUnitState.UnMove);
                }
                
                if (kv.Value.GetStateCount(EUnitState.UnHurt) > 0)
                {
                    kv.Value.RemoveState(EUnitState.UnHurt);
                }
            }
        }
        
        public void HurtEachMoveRoundEndTrigger()
        {

            foreach (var kv in BattleUnitManager.Instance.BattleUnitDatas)
            {
                if (kv.Value.GetStateCount(EUnitState.HurtEachMove) > 0 && kv.Value.RoundMoveCount <= 0)
                {
                    kv.Value.ChangeState(EUnitState.HurtEachMove);
                }
            }

        }
        
        public void HurtRoundStartMoveTrigger(int passUnitID, int bePassUnitID, List<TriggerData> triggerDatas)
        {
            var passUnit = GameUtility.GetUnitDataByIdx(passUnitID, true);
            var bePassUnit = GameUtility.GetUnitDataByIdx(bePassUnitID, true);

            if(passUnit == null || bePassUnit == null)
                return;
            
            if (passUnit.GetStateCount(EUnitState.HurtRoundStart) > 0)
            {
                var triggerData = BattleFightManager.Instance.Unit_State(triggerDatas, passUnitID, passUnitID, bePassUnitID,
                    EUnitState.HurtRoundStart, 1, ETriggerDataType.RoleState);
                triggerDatas.Add(triggerData);
            }
            if (bePassUnit.GetStateCount(EUnitState.HurtRoundStart) > 0)
            {
                var triggerData = BattleFightManager.Instance.Unit_State(triggerDatas,bePassUnitID, bePassUnitID, passUnitID,
                    EUnitState.HurtRoundStart, -1, ETriggerDataType.RoleState);
                triggerDatas.Add(triggerData);
            }
        }
        
        public void CheckUnitState(int actionUnitID, List<TriggerData> triggerDatas)
        {
   
            //var actionUnitData = GameUtility.GetUnitByID(actionUnitID);
            //var effectUnitData = GameUtility.GetUnitByID(effectUnitID);
            
            // if (actionUnitData.GetStateCount(EUnitState.HurtSubDamage) > 0)
            // {
            //     var hurtSubDamageTriggerData = FightManager.Instance.Unit_State(actionUnitData.ID,
            //         actionUnitData.ID, actionUnitData.ID, EUnitState.HurtSubDamage, 1, ETriggerDataType.RoleState);
            //     FightManager.Instance.SimulateTriggerData(hurtSubDamageTriggerData, triggerDatas);
            //     triggerDatas.Add(hurtSubDamageTriggerData);
            //
            // }
                
            // if (effectUnitData.GetStateCount(EUnitState.AddDamage) > 0)
            // {
            //     var addDamageTriggerData = FightManager.Instance.Unit_State(effectUnitData.ID,
            //         effectUnitData.ID, effectUnitData.ID, EUnitState.AddDamage, -1, ETriggerDataType.RoleState);
            //     FightManager.Instance.SimulateTriggerData(addDamageTriggerData, triggerDatas);
            //     triggerDatas.Add(addDamageTriggerData);
            // }

            // var actionUnitAddDamageCount = actionUnitData.GetStateCount(EUnitState.AddDamage);
            // if (actionUnitAddDamageCount > 0)
            // {
            //     var addDamageTriggerData = FightManager.Instance.Unit_State(triggerDatas, actionUnitData.ID,
            //         actionUnitData.ID, actionUnitData.ID, EUnitState.AddDamage, -actionUnitAddDamageCount, ETriggerDataType.RoleState);
            //     FightManager.Instance.SimulateTriggerData(addDamageTriggerData, triggerDatas);
            //     triggerDatas.Add(addDamageTriggerData);
            // }
            //
            // if (actionUnitData.GetStateCount(EUnitState.HurtSubDamage) > 0)
            // {
            //     var hurtAddDamageTriggerData = FightManager.Instance.Unit_State(triggerDatas, actionUnitData.ID,
            //         actionUnitData.ID, actionUnitData.ID, EUnitState.HurtSubDamage, 1, ETriggerDataType.RoleState);
            //     FightManager.Instance.SimulateTriggerData(hurtAddDamageTriggerData, triggerDatas);
            //     triggerDatas.Add(hurtAddDamageTriggerData);
            // }
            
            
            
            // if (effectUnitData.GetStateCount(EUnitState.SubDamage) > 0)
            // {
            //     var subDamageTriggerData = FightManager.Instance.Unit_State(effectUnitData.ID,
            //         effectUnitData.ID, effectUnitData.ID, EUnitState.SubDamage, -1, ETriggerDataType.RoleState);
            //     FightManager.Instance.SimulateTriggerData(subDamageTriggerData, triggerDatas);
            //     triggerDatas.Add(subDamageTriggerData);
            // }
            
            // if (actionUnitData.GetStateCount(EUnitState.SubDamage) > 0)
            // {
            //     var subDamageTriggerData = FightManager.Instance.Unit_State(actionUnitData.ID,
            //         actionUnitData.ID, actionUnitData.ID, EUnitState.SubDamage, -1, ETriggerDataType.RoleState);
            //     FightManager.Instance.SimulateTriggerData(subDamageTriggerData, triggerDatas);
            //     triggerDatas.Add(subDamageTriggerData);
            // }
        }
    }
}



