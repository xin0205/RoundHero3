
using UnityEngine;

namespace RoundHero
{
    public class BattleCardEntityData : EntityData
    {
        public int CardID;
        public Data_Card CardData;
        
        public void Init(int entityId, Vector2 pos, int cardID)
        {
            base.Init(entityId, pos);
            CardID = cardID;
            CardData = BattleManager.Instance.GetCard(CardID);
        }

        
    }
}