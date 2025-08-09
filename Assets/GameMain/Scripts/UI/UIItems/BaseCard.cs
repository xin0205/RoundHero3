using JetBrains.Annotations;
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
        
        [SerializeField] [CanBeNull] private GameObject hpGO;

        [SerializeField] private Image Icon;
        
        private int CardID = -1;
        private int CardIdx = -1;
        
        public void SetCardUI(int cardID, int cardIdx = -1)
        {
            CardID = cardID;
            CardIdx = cardIdx;

            RefreshCardUI();
        }

        public async void RefreshCardUI(string cardName, string cardDesc, int cardID)
        {
            //Icon.sprite = await AssetUtility.GetTacticIcon(cardID);
            this.cardName.text = cardName;
            this.desc.text = cardDesc;
            if (hpGO != null)
            {
                hpGO?.SetActive(false);
            }
            
            RefreshEnergy(-1);
        }

        public void RefreshEnergy(int energy)
        {
            if (energy < 0)
            {
                this.energy.text = "?";
            }
            else
            {
                this.energy.text = energy.ToString();
            }
            
            
        }

        public async void RefreshCardUI()
        {

            //var card = BattleManager.Instance.GetCard(CardID);
            var drCard = GameEntry.DataTable.GetCard(CardID);
            if (drCard.CardType == ECardType.Unit)
            {
                Icon.sprite = await AssetUtility.GetFollowerIcon(CardID);

                var maxHP = BattleCardManager.Instance.GetCardMaxHP(CardID, CardIdx);
                
                hp.text = maxHP.ToString();
                
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

            if (hpGO != null)
            {
                hpGO.SetActive(drCard.CardType == ECardType.Unit);
            }

            var _energy = drCard.Energy;

            if (CardIdx != -1)
            {
                _energy = BattleCardManager.Instance.GetCardEnergy(CardIdx);
            }
            

            energy.text = _energy < 0 ? "X" : _energy.ToString();

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

        public void SetIconVisible(bool isVisible)
        {
            Icon.gameObject.SetActive(isVisible);
        }
    }
}