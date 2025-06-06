
using System.Collections.Generic;


namespace RoundHero
{
    public partial class BattleCardManager : Singleton<BattleCardManager>
    {
        public void CacheTacticCardData(int cardIdx, EUnitCamp camp, Data_BattleUnit effectUnit)
        {
            var drCard = CardManager.Instance.GetCardTable(cardIdx);
            var card = BattleManager.Instance.GetCard(cardIdx);
            var triggerDatas = new List<TriggerData>();
            foreach (var buffID in drCard.BuffIDs)
            {
                var buffData = BattleBuffManager.Instance.GetBuffData(buffID);
                var values = new List<float>();
                foreach (var value in drCard.Values1)
                {
                    values.Add(GameUtility.GetBuffValue(value, effectUnit != null ? effectUnit.Idx : -1));
                }
                
                
                //BattleBuffManager.Instance.CacheBuffData(buffData, camp, effectUnit, values, 1 + card.UseCardDamageRatio);
                //
                BattleBuffManager.Instance.BuffTrigger(buffData.BuffTriggerType, buffData, values, -1, -1,
                    effectUnit != null ? effectUnit.Idx : -1, triggerDatas, -1, -1);
                BattleFightManager.Instance.RoundFightData.BuffData_Use.ActionDataType = EActionDataType.Tactic;
                
                
            }
            BattleFightManager.Instance.RoundFightData.BuffData_Use.TriggerDatas.Add(cardIdx, triggerDatas);
        }

        

        
       
        

    }
}