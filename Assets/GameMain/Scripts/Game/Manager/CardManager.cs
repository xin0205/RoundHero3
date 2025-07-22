using System;
using System.Collections.Generic;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class CardManager : Singleton<CardManager>
    {
        public Dictionary<int, Data_Card> CardDatas => BattlePlayerManager.Instance.PlayerData.CardDatas;
        
        public Dictionary<int, Data_Card> TempCards = new Dictionary<int, Data_Card>();
        
        private int id;
        public int GetIdx()
        {
            return BattlePlayerManager.Instance.PlayerData.CardIdx++;
        }
        
        public int GetTempIdx()
        {
            return BattlePlayerManager.Instance.PlayerData.CardIdx++;
        }
        
        public Data_Card GetCard(int cardIdx)
        {
            if (CardDatas.ContainsKey(cardIdx))
                return CardDatas[cardIdx];
            
            // if(TempCards.ContainsKey(cardID))
            //     return TempCards[cardID];
            
            return null;
        }
        
        public DRCard GetCardTable(int cardIdx)
        {
            var cardData = BattleManager.Instance.GetCard(cardIdx);
            if (cardData == null)
                return null;

            return GameEntry.DataTable.GetCard(cardData.CardID);

        }

        public bool Contain(int cardID, EBuffID buffID)
        {
            return Contain(cardID, buffID.ToString());
        }
        
        public bool Contain(int cardID, string buffIDStr)
        {
            var drCard = GetCardTable(cardID);
            return drCard.BuffIDs.Contains(buffIDStr);
        }

        public int GetHPDelta(int cardID, float value)
        {
            //var drCard = GetCardTable(cardID);
            var card = BattleManager.Instance.GetCard(cardID);

            return (int)(value * (1 + card.UseCardDamageRatio));
        }
        
        public List<BuffData> GetBuffData(int cardIdx)
        {
            var drCard = GetCardTable(cardIdx);
            
            var buffDatas = new List<BuffData>();

            foreach (var buffIDStr in drCard.BuffIDs)
            {
                var buffData = BattleBuffManager.Instance.GetBuffData(buffIDStr);
                buffData.BuffEquipType = EBuffEquipType.Normal;
                buffDatas.Add(buffData);
            }

            var cardData = GetCard(cardIdx);
            foreach (var funeIdx in cardData.FuneIdxs)
            {
                var drBuff = FuneManager.Instance.GetBuffTable(funeIdx);
                foreach (var buffIDStr in drBuff.BuffIDs)
                {
                    var buffData = BattleBuffManager.Instance.GetBuffData(buffIDStr);
                    buffData.BuffEquipType = EBuffEquipType.Fune;
                    buffDatas.Add(buffData);
                }
                
            }

            return buffDatas;
        }
        
        public List<List<float>> GetBuffValues(int soliderIdx, TriggerData preTriggerData = null)
        {
            var soliderUnit = BattleUnitManager.Instance.GetUnitByIdx(soliderIdx) as BattleSoliderEntity;
            //Log.Debug("GetBuffValues:" + BattleUnitManager.Instance.BattleUnitDatas.Count + "-" + BattleUnitManager.Instance.BattleUnitEntities.Count);

            if (soliderUnit == null)
            {
                Log.Debug("AA");
            }
            
            var drCard = CardManager.Instance.GetCardTable(soliderUnit.BattleSoliderEntityData.BattleSoliderData.CardIdx);
            
            var valuelist = new List<List<float>>();

            var idx = 0;
            foreach (var buffID in drCard.BuffIDs)
            {
                var values = new List<float>();
                foreach (var value in drCard.GetValues(idx++))
                {
                    var targetValue = BattleBuffManager.Instance.GetBuffValue(value, soliderIdx, soliderUnit.BattleSoliderEntityData.BattleSoliderData.CardIdx, preTriggerData);
                    values.Add(targetValue);
  
                }

                valuelist.Add(values);
            }
            idx = 0;
            var cardData = GetCard(soliderUnit.BattleSoliderEntityData.BattleSoliderData.CardIdx);
            
            foreach (var funeIdx in cardData.FuneIdxs)
            {
                var drBuff = FuneManager.Instance.GetBuffTable(funeIdx);
                var funeValues = new List<float>();
                foreach (var buffID in drBuff.BuffIDs)
                {
                    foreach (var buffValue in drBuff.GetValues(idx))
                    {
                        var value = BattleBuffManager.Instance.GetBuffValue(buffValue, soliderIdx,
                            soliderUnit.BattleSoliderEntityData.BattleSoliderData.CardIdx, preTriggerData);
                        funeValues.Add(value);

                    }
                    valuelist.Add(funeValues);
                    idx++;
                }

                idx = 0;
                
            }

            return valuelist;
            
            
        }

        

        public float GetBuffValue(int cardID, int buffIdx, int valueIdx)
        {
            var drCard = GetCardTable(cardID);
            
            var idx = 1;
            
            var values = drCard.GetValues(buffIdx);
            string value = String.Empty;

            if (valueIdx < values.Count)
            {
                value = values[valueIdx];
            }

            return BattleBuffManager.Instance.GetBuffValue(value);

        }

        public int GetDamage(int cardID)
        {
            var damage = 0;
            var drBuffs = GetBuffData(cardID);
            var drCard = GetCardTable(cardID);

            var idx = 1;
            foreach (var drBuff in drBuffs)
            {
                var values = drCard.GetValues(idx++);
                var value = BattleBuffManager.Instance.GetBuffValue(values[0]);
     
                if (drBuff.UnitAttribute == EUnitAttribute.HP && value <= 0)
                {
                    damage += (int)value;
                }

                idx++;
            }

            return damage;
        }

    }
}