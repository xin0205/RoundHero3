using System.Collections.Generic;
using GameFramework;
using UnityEngine;

namespace RoundHero
{
    public class TriggerActionData
    {
        public TriggerData TriggerData;
        public MoveUnitData MoveUnitData;
    }
    
    
    public class BattleBulletManager : Singleton<BattleBulletManager>
    {
        public Dictionary<int, Dictionary<int, TriggerActionData>> TriggerActionDatas =
            new ();


        public void AddTriggerData(TriggerData triggerData)
        {
            //var effectUnit = BattleUnitManager.Instance.GetUnitByID(triggerData.EffectUnitID);
            //effectUnit.ActionUnitID = triggerData.ActionUnitID;

            
            if (!TriggerActionDatas.ContainsKey(triggerData.ActionUnitID))
            {
                TriggerActionDatas.Add(triggerData.ActionUnitID, new Dictionary<int, TriggerActionData>());
            } 
            
            TriggerActionData triggerActionData;
            if (!TriggerActionDatas[triggerData.ActionUnitID].ContainsKey(triggerData.EffectUnitID))
            {
                triggerActionData = new TriggerActionData();
                TriggerActionDatas[triggerData.ActionUnitID].Add(triggerData.EffectUnitID, triggerActionData);
            }
            else
            {
                triggerActionData = TriggerActionDatas[triggerData.ActionUnitID][triggerData.EffectUnitID];
            }
            
            
            triggerActionData.TriggerData = triggerData.Copy();
            TriggerActionDatas[triggerData.ActionUnitID][triggerData.EffectUnitID] = triggerActionData;
        }
        
        public void AddMoveActionData(int actionUnitID, MoveData moveData)
        {
            //var effectUnitEntity = BattleUnitManager.Instance.GetUnitByID(moveActionData.ActionUnitID);
            //effectUnit.ActionUnitID = triggerData.ActionUnitID;

            foreach (var kv in moveData.MoveUnitDatas)
            {
                if (!TriggerActionDatas.ContainsKey(kv.Key))
                {
                
                    TriggerActionDatas.Add(kv.Key, new Dictionary<int, TriggerActionData>());
                }
                
                TriggerActionData triggerActionData;
                if (!TriggerActionDatas[actionUnitID].ContainsKey(kv.Key))
                {
                    triggerActionData = new TriggerActionData();
                    TriggerActionDatas[actionUnitID].Add(kv.Key, triggerActionData);
                }
                else
                {
                    triggerActionData = TriggerActionDatas[actionUnitID][kv.Key];
                }

                triggerActionData.MoveUnitData = kv.Value.Copy();
                TriggerActionDatas[actionUnitID][kv.Key] = triggerActionData;
            }
            
        }

        public void UseMoveActionData(int actionUnitID, int effectUnitID)
        {
            if (!TriggerActionDatas.ContainsKey(actionUnitID))
            {
                return;
            }
            
            if (!TriggerActionDatas[actionUnitID].ContainsKey(effectUnitID))
            {
                return;
            }
            
            var moveActionData = TriggerActionDatas[actionUnitID][effectUnitID];
            if (moveActionData.MoveUnitData == null)
            {
                return;
            }
            
            var effectUnitEntity = BattleUnitManager.Instance.GetUnitByID(effectUnitID);

            if (moveActionData.MoveUnitData.UnitActionState == EUnitActionState.Fly)
            {
                effectUnitEntity.Fly(moveActionData.MoveUnitData.MoveActionData);
            }
            else if (moveActionData.MoveUnitData.UnitActionState == EUnitActionState.Run)
            {
                effectUnitEntity.Run(moveActionData.MoveUnitData.MoveActionData);
            }

            
        }

        public void UseTriggerData(int actionUnitID, int effectUnitID)
        {
            if (!TriggerActionDatas.ContainsKey(actionUnitID))
            {
                return;
            }
            
            if (!TriggerActionDatas[actionUnitID].ContainsKey(effectUnitID))
            {
                return;
            }

            BattleFightManager.Instance.TriggerAction(TriggerActionDatas[actionUnitID][effectUnitID].TriggerData);
            

        }

        public void ActionUnitTrigger(int actionUnitID)
        {
            if (!TriggerActionDatas.ContainsKey(actionUnitID))
            {
                return;
            }

            foreach (var kv in TriggerActionDatas[actionUnitID])
            {
                var effectUnit = BattleUnitManager.Instance.GetUnitByID(kv.Key);
                
                UseTriggerData(actionUnitID, effectUnit.ID);
                UseMoveActionData(actionUnitID, effectUnit.ID);
            }
            TriggerActionDatas[actionUnitID].Clear();
            BattleManager.Instance.RefreshView();
        }

        public Dictionary<int, TriggerActionData> GetTriggerDatas(int actionUnitID)
        {
            if (!TriggerActionDatas.ContainsKey(actionUnitID))
            {
                return null;
            }

            return TriggerActionDatas[actionUnitID];
        }
        
        
    }
}