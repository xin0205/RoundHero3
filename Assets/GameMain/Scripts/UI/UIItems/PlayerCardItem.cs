using System;
using UnityEngine;



namespace RoundHero
{
    
    public class PlayerCardItem : MonoBehaviour
    {
        
        private PlayerCardData playerCardData;
        
        [SerializeField] private CardItem CardItem;

        // [SerializeField] private List<Image> FuneIcons = new List<Image>();
        //
        // [SerializeField] private List<GameObject> FuneGOs = new List<GameObject>();
        //
        // [SerializeField] private List<GameObject> FuneDownGOs = new List<GameObject>();

        [SerializeField] private PlayerCardFuneList PlayerCardFuneList;
        [SerializeField] private ExplainTriggerItem explainTriggerItem;

        public Action<int> OnPointEnterAction;
        public Action OnPointExitAction;
        public Action<int> OnClickAction;
        
        public Action<int> OnDropAction;

        
        
        public void Init(Action<int> onPointEnterAction, Action onPointExitAction, Action<int> onClickAction)
        {
            OnPointEnterAction = onPointEnterAction;
            OnPointExitAction = onPointExitAction;
            OnClickAction = onClickAction;
        }
        
        public void SetItemData(PlayerCardData playerCardData, bool isShowAllFune)
        {
            
            this.playerCardData = playerCardData;
            
            var cardData = CardManager.Instance.CardDatas[this.playerCardData.CardIdx];

            //OnDropAction = onPointUpAction;
            var drCard = GameEntry.DataTable.GetCard(cardData.CardID);
            
            explainTriggerItem.ExplainData = new ExplainData()
            {
                ItemType = drCard.CardType == ECardType.Unit ? EItemType.UnitCard : EItemType.TacticCard,
                ItemID = playerCardData.CardID,
                ShowPosition = EShowPosition.MousePosition,
                VideoID = drCard.GIFIdx,
            };

            
            CardItem.SetCard(this.playerCardData.CardID,this.playerCardData.CardIdx);
            PlayerCardFuneList.Init(this.playerCardData.CardIdx, isShowAllFune);

            //PlayerCardFuneList.Refresh();
            
        }

        public void OnPointEnter()
        {
            var drCard = CardManager.Instance.GetCardTable(this.playerCardData.CardIdx);
            // if(drCard.CardType != ECardType.Unit)
            //     return;
            
            OnPointEnterAction?.Invoke(playerCardData.CardIdx);
            

        }

        public void OnClick()
        {
            OnClickAction.Invoke(playerCardData.CardIdx);
        }
        
        public void OnPointExit()
        {
            var drCard = CardManager.Instance.GetCardTable(this.playerCardData.CardIdx);
            if(drCard.CardType != ECardType.Unit)
                return;
            
            OnPointExitAction?.Invoke();
            

        }
        
        public void OnPointUp()
        {
            

            
        }
        
        public void OnDrop()
        {
            OnDropAction?.Invoke(playerCardData.CardIdx);
        }

        
        
    }
}