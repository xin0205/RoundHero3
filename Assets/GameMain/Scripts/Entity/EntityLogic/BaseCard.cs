using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace RoundHero
{
    public class BaseCard : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI cardName;
        [SerializeField]
        private TextMeshProUGUI desc;
        [SerializeField]
        private TextMeshProUGUI energy;

        public GameObject ActionGO;
        
        private int CardID;

        
        public void SetCardUI(int cardID)
        {
            ActionGO.SetActive(false);
            CardID = cardID;
            

            RefreshCardUI();
        }

        public void RefreshCardUI()
        {
            //var card = CardManager.Instance.GetCard(CardID);
            //var drCard = GameEntry.DataTable.GetCard(card.CardID);
            
            var card = BattleManager.Instance.GetCard(CardID);
            
            
            //var drCard = GameEntry.DataTable.GetCard(card.CardID);
            
            var cardName = "";
            var cardDesc = "";
            GameUtility.GetCardText(card, ref cardName, ref cardDesc);

            this.cardName.text = cardName + (card.UnUse ? "Un" : "");
            desc.text = cardDesc;
            
            
            
            // if (card.CardID == ECardID.HurtUsDamage)
            // {
            //     cardEnergy = BattleUnitManager.Instance.GetUnitCount(EUnitCamp.Us, new List<EUnitCamp>() {EUnitCamp.Us});
            // }
            //
            // if (card.FuneDatas.Contains(EFuneID.AddCurHP) && cardEnergy > 0)
            // {
            //     cardEnergy -= 1;
            // }
            
            var drCard = CardManager.Instance.GetCardTable(CardID);
            var maxHP = BattleUnitManager.Instance.GetUnitHP(CardID);

            var cardEnergy = BattleCardManager.Instance.GetCardEnergy(CardID);
            
            
            var bless = GamePlayManager.Instance.GamePlayData.GetUsefulBless(EBlessID.EachUseCardUnUseEnergy, BattleManager.Instance.CurUnitCamp);
            if (bless != null && bless.Value <= 0 && cardEnergy > 0)
            {
                cardEnergy = 0;
            }

            
            energy.text = cardEnergy < 0 ? "X" : cardEnergy.ToString() + "-" + maxHP;
        }

        
    }
}