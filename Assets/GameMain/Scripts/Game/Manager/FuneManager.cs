using System;
using System.Collections.Generic;
using System.Linq;

namespace RoundHero
{
    public class FuneManager : Singleton<FuneManager>
    {

        public Dictionary<int, Data_Fune> FuneDatas => BattlePlayerManager.Instance.PlayerData.FuneDatas;
        
        public Data_Fune GetFuneData(int funeIdx)
        {
            if (BattlePlayerManager.Instance.PlayerData.FuneDatas.ContainsKey(funeIdx))
            {
                return BattlePlayerManager.Instance.PlayerData.FuneDatas[funeIdx];
            }

            return null;
        }

        public int GetIdx()
        {
            return PlayerManager.Instance.PlayerData.FuneIdx++;
        }
        
        public DRBuff GetBuffTable(int funeID)
        {
            var funeData = GetFuneData(funeID);
            return GameEntry.DataTable.GetBuff(funeData.FuneID);

        }
        
        // public EBuffID GetFuneID(int funeIdx)
        // {
        //     if (BattlePlayerManager.Instance.PlayerData.FuneDatas.ContainsKey(funeID))
        //     {
        //         return BattlePlayerManager.Instance.PlayerData.FuneDatas[funeID].FuneID;
        //     }
        //
        //     return EBuffID.Empty;
        // }
        
        public void CacheUnitUseData(int ownUnitID, int actionUnitID, int cardID, EUnitCamp unitCamp, int gridPosIdx)
        {
            BattleFightManager.Instance.RoundFightData.UseCardDatas.Clear();
            var card = BattleManager.Instance.GetCard(cardID);
            foreach (var funeID in card.FuneIdxs)
            {
                FuneManager.Instance.UseTrigger(funeID, ownUnitID, actionUnitID, unitCamp, gridPosIdx,
                    BattleFightManager.Instance.RoundFightData.UseCardDatas);
            }
            
        }
        
        public void CacheUnitKillData(int ownUnitID, int actionUnitID, int unitID, List<TriggerData> triggerDatas)
        {
            BattleFightManager.Instance.RoundFightData.UseCardDatas.Clear();
            var unit = BattleFightManager.Instance.GetUnitByID(unitID);

            if (unit is Data_BattleSolider solider)
            {
                var card = BattleManager.Instance.GetCard(solider.CardID);
                foreach (var funeID in card.FuneIdxs)
                {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
                    FuneManager.Instance.KillTrigger(funeID, ownUnitID, actionUnitID,
                        triggerDatas);
                }
            }
            
            
            
        }

        public void CacheEachRoundDatas()
        {
            
        }

        public void TriggerUnitUse()
        {
            foreach (var triggerData in BattleFightManager.Instance.RoundFightData.UseCardDatas)
            {
                BattleFightManager.Instance.TriggerAction(triggerData);
            }
            
            //BattleManager.Instance.Refresh();

            //return FightManager.Instance.RoundFightData.UseCardDatas.Count > 0 ? 1 : 0;
        }

        // public TriggerData RoundTrigger(int funeID, int ownUnitID, int actionUnitID, List<TriggerData> triggerDatas)
        // {
        //     var _funeID = FuneManager.Instance.GetFuneID(funeID);
        //     var drFune = FuneManager.Instance.GetFuneTable(funeID);
        //     if (drFune.BuffTriggerType != EBuffTriggerType.Round)
        //         return null;
        //     
        //     switch (_funeID)
        //     {
        //         case EFuneID.EachRound_AddCurHP:
        //             return EachRound_AddCurHP(funeID, ownUnitID, actionUnitID, triggerDatas);
        //
        //     }
        //
        //     return null;
        // }
        
        private TriggerData EachRound_AddCurHP(int funeID, int ownUnitID, int actionUnitID, 
            List<TriggerData> triggerDatas)
        {
            var drFune = FuneManager.Instance.GetBuffTable(funeID);
            var triggerData = BattleFightManager.Instance.BattleRoleAttribute(ownUnitID, actionUnitID,
                actionUnitID, EUnitAttribute.HP, BattleBuffManager.Instance.GetBuffValue(drFune.BuffValues[0]), ETriggerDataSubType.Unit);
            triggerDatas.Add(triggerData);
                
            BattleFightManager.Instance.SimulateTriggerData(triggerData, triggerDatas);

            return triggerData;
        }

        public void KillTrigger(int funeID, int ownUnitID, int actionUnitID, List<TriggerData> triggerDatas)
        {
            //Wrong
            // var _funeID = FuneManager.Instance.GetFuneID(funeID);
            // switch (_funeID)
            // {
            //     case EFuneID.Kill_AddCurHP:
            //         Kill_AddCurHP(funeID, ownUnitID, actionUnitID, triggerDatas);
            //         break;
            //     case EFuneID.Kill_AddHeroCurHP:
            //         Kill_AddHeroCurHP(funeID, ownUnitID, actionUnitID, triggerDatas);
            //         break;
            //     case EFuneID.Kill_AddCoin:
            //         Kill_AddCoin(funeID, ownUnitID, actionUnitID, triggerDatas);
            //         break;
            //     case EFuneID.Kill_ToHand:
            //         Kill_ToHandCards(funeID, ownUnitID, actionUnitID, triggerDatas);
            //         break;
            //     case EFuneID.Kill_RemoveCard:
            //         Kill_RemoveCard(funeID, ownUnitID, actionUnitID, triggerDatas);
            //         break;    
            //         
            // }
        }

        public void UseTrigger(int funeID, int ownUnitID, int actionUnitID, EUnitCamp selfUnitCamp, int gridPosIdx, List<TriggerData> triggerDatas)
        {
            //Wrong
            // var _funeID = FuneManager.Instance.GetFuneID(funeID);
            // switch (_funeID)
            // {
            //    
            //     case EFuneID.Use_ToStandBy:
            //         break;
            //     case EFuneID.Use_AddCurHP_SubCurHP:
            //         Use_AddCurHP_SubCurHP(ownUnitID, actionUnitID, selfUnitCamp, gridPosIdx, triggerDatas);
            //         break;
            //     case EFuneID.Use_CopyToPass:
            //         break;
            //
            //
            // }
        }

        private void Use_AddCurHP_SubCurHP(int ownUnitID, int actionUnitID, EUnitCamp selfUnitCamp, int gridPosIdx, List<TriggerData> triggerDatas)
        {
            //Wrong
            // var drFune = GameEntry.DataTable.GetFune(EFuneID.Use_AddCurHP_SubCurHP);
            //
            // var unitGridPosIdxs = GameUtility.GetRange(gridPosIdx, EActionType.Around, selfUnitCamp, new List<ERelativeCamp>()
            // {
            //     ERelativeCamp.Enemy, ERelativeCamp.Us
            // });
            //
            // var isSubCurHP = false;
            // foreach (var unitGridPosIdx in unitGridPosIdxs)
            // {
            //     var unit = FightManager.Instance.GetUnitByGridPosIdx(unitGridPosIdx);
            //     if(unit == null)
            //         continue;
            //
            //     var value = unit.UnitCamp == selfUnitCamp ? BattleBuffManager.Instance.GetBuffValue(drFune.BuffValues[0]) : BattleBuffManager.Instance.GetBuffValue(drFune.BuffValues[1]);
            //
            //     var triggerData = FightManager.Instance.BattleRoleAttribute(ownUnitID, actionUnitID,
            //         unit.ID, EUnitAttribute.CurHP, value, ETriggerDataSubType.Unit);
            //     triggerDatas.Add(triggerData);
            //     
            //     FightManager.Instance.SimulateTriggerData(triggerData,triggerDatas);
            //     
            //     if (GameUtility.IsSubCurHPTrigger(triggerData))
            //     {
            //         isSubCurHP = true;
            //     }
            // }
            // if (isSubCurHP)
            // {
            //     BattleBuffManager.Instance.AttackTrigger(triggerDatas[triggerDatas.Count - 1], triggerDatas);
            //     BattleUnitStateManager.Instance.CheckUnitState(actionUnitID, triggerDatas);
            // }
        }

        private void Kill_AddCurHP(int funeID, int ownUnitID, int actionUnitID, 
            List<TriggerData> triggerDatas)
        {
            //Wrong
            // var unit = FightManager.Instance.GetUnitByID(actionUnitID);
            // var funeCount = unit.FuneCount(EFuneID.Kill_AddCurHP);
            // var drFune = FuneManager.Instance.GetFuneTable(funeID);
            // var triggerData = FightManager.Instance.BattleRoleAttribute(ownUnitID, actionUnitID,
            //     actionUnitID, EUnitAttribute.CurHP, BattleBuffManager.Instance.GetBuffValue(drFune.BuffValues[0]) * funeCount, ETriggerDataSubType.Unit);
            // triggerDatas.Add(triggerData);
            //     
            // FightManager.Instance.SimulateTriggerData(triggerData, triggerDatas);
        }
        
        private void Kill_AddHeroCurHP(int funeID, int ownUnitID, int actionUnitID, 
            List<TriggerData> triggerDatas)
        {
            var unit = BattleFightManager.Instance.GetUnitByID(actionUnitID);
            var triggerData = BattleFightManager.Instance.BattleRoleAttribute(ownUnitID, actionUnitID,
                BattleFightManager.Instance.PlayerData.BattleHero.ID, EUnitAttribute.HP, unit.MaxHP - unit.CurHP, ETriggerDataSubType.Unit);
            triggerDatas.Add(triggerData);
                
            BattleFightManager.Instance.SimulateTriggerData(triggerData, triggerDatas);
        }
        
        private void Kill_AddCoin(int funeID, int ownUnitID, int actionUnitID, 
            List<TriggerData> triggerDatas)
        {
            var drFune = FuneManager.Instance.GetBuffTable(funeID);

            var triggerData = BattleFightManager.Instance.Unit_HeroAttribute(ownUnitID, actionUnitID,
                BattleFightManager.Instance.PlayerData.BattleHero.ID, EHeroAttribute.Coin, BattleBuffManager.Instance.GetBuffValue(drFune.BuffValues[0]));
            triggerDatas.Add(triggerData);
                
            BattleFightManager.Instance.SimulateTriggerData(triggerData, triggerDatas);
        }
        
        private void Kill_ToHandCards(int funeID, int ownUnitID, int actionUnitID, 
            List<TriggerData> triggerDatas)
        {
            //Wrong
            // var drFune = GameEntry.DataTable.GetFune(EFuneID.Kill_ToHand);
            //
            // var unit = FightManager.Instance.GetUnitByID(actionUnitID);
            // var triggerData = FightManager.Instance.Hero_Card(ownUnitID, actionUnitID, actionUnitID, 
            //     BattleBuffManager.Instance.GetBuffValue(drFune.BuffValues[0]), ECardTriggerType.ToHand);;
            //
            //
            // triggerDatas.Add(triggerData);
            //     
            // FightManager.Instance.SimulateTriggerData(triggerData, triggerDatas);
        }
        
        
        private void Kill_RemoveCard(int funeID, int ownUnitID, int actionUnitID, 
            List<TriggerData> triggerDatas)
        {
            //Wrong
            // var drFune = GameEntry.DataTable.GetFune(EFuneID.Kill_RemoveCard);
            //
            // var unit = FightManager.Instance.GetUnitByID(actionUnitID);
            // var triggerData = FightManager.Instance.Hero_Card(ownUnitID, actionUnitID, actionUnitID, 
            //     BattleBuffManager.Instance.GetBuffValue(drFune.BuffValues[0]), ECardTriggerType.ConsumeCard);;
            //
            //
            // triggerDatas.Add(triggerData);
            //     
            // FightManager.Instance.SimulateTriggerData(triggerData, triggerDatas);
        }
        

        public void FuneAttackTrigger(TriggerData triggerData, List<TriggerData> triggerDatas)
        {
            
        }
    }
}