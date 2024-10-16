using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public class BaseCard : MonoBehaviour
    {
        [SerializeField]
        private Text cardName;
        [SerializeField]
        private Text desc;
        [SerializeField]
        private Text energy;
        
        [SerializeField]
        private Text hp;
        
        [SerializeField]
        private GameObject hpGO;

        [SerializeField] private Image Icon;
        
        private int CardID = -1;

        
        public void SetCardUI(int cardID)
        {

            CardID = cardID;
            

            RefreshCardUI();
        }

        public async void RefreshCardUI()
        {

            //var card = BattleManager.Instance.GetCard(CardID);
            var drCard = GameEntry.DataTable.GetCard(CardID);
            if (drCard.CardType == ECardType.Unit)
            {
                Icon.sprite = await AssetUtility.GetFollowerIcon(CardID);
                hp.text = drCard.HP.ToString();
            }
            else if (drCard.CardType == ECardType.Tactic)
            {
                Icon.sprite = await AssetUtility.GetTacticIcon(CardID);
            }
            

            var cardName = "";
            var cardDesc = "";
            GameUtility.GetCardText(CardID, ref cardName, ref cardDesc);

            // + (card.UnUse ? "Un" : "");
            
            this.cardName.text = cardName;
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
            
            
            //hpGO.SetActive(drCard.CardType == ECardType.Unit);
            energy.text = drCard.Energy < 0 ? "X" : drCard.Energy.ToString();

            //var maxHP = BattleUnitManager.Instance.GetUnitHP(CardID);

            // var cardEnergy = BattleCardManager.Instance.GetCardEnergy(CardID);
            //
            //
            // var bless = GamePlayManager.Instance.GamePlayData.GetUsefulBless(EBlessID.EachUseCardUnUseEnergy, BattleManager.Instance.CurUnitCamp);
            // if (bless != null && bless.Value <= 0 && cardEnergy > 0)
            // {
            //     cardEnergy = 0;
            // }
            //
            // energy.text = cardEnergy < 0 ? "X" : cardEnergy.ToString();


        }

        
    }
}