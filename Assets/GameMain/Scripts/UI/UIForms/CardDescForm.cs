using System;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RoundHero
{
    [Serializable]
    public class CardDescFormData
    {
        public int CardIdx;
    }
    
    public class CardDescForm : UGuiForm
    {
        public CardDescFormData CardDescFormData;

        [SerializeField]
        private GameObject root;

        [SerializeField] private VideoPlayItem videoPlayItem;
        [SerializeField] private ExplainList explainList;
        
        [SerializeField]
        private GameObject funeListGO;
        [SerializeField]
        private GameObject unitStateListGO;

        [SerializeField]
        private Transform pos1;
        [SerializeField]
        private Transform pos2;


        private bool hasDetail;
        private bool hasFune;

        [SerializeField] private List<PlayerCommonItem> funeList = new List<PlayerCommonItem>();
 
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            CardDescFormData = (CardDescFormData)userData;
            
            videoPlayItem.gameObject.SetActive(true);
            
            var items = funeListGO.GetComponentsInChildren<PlayerCommonItem>(true);
            funeList.Clear();
            funeList.AddRange(items);

            hasFune = false;
            hasDetail = false;

            RefreshVideo();
            RefreshDesc();
            RefreshExplain(true);

        }

        private void RefreshVideo()
        {
            var animationPlayData = new AnimationPlayData();
            var drCard = CardManager.Instance.GetCardTable(CardDescFormData.CardIdx);
            animationPlayData.ID = drCard.GIFIdx;
            
            videoPlayItem.gameObject.SetActive(true);
            videoPlayItem.SetVideo(animationPlayData);
        }

        private void RefreshDesc()
        {
            foreach (var playerCommonItem in funeList)
            {
                playerCommonItem.gameObject.SetActive(false);
            }

            var cardData =
                CardManager.Instance.GetCard(CardDescFormData.CardIdx);
 
            var idx = 0;
            foreach (var funeIdx in cardData.FuneIdxs)
            {
                var funeData = FuneManager.Instance.GetFuneData(funeIdx);
                if(idx >= funeList.Count)
                    break;
        
                hasFune = true;
                hasDetail = true;
                funeList[idx].gameObject.SetActive(true);
                funeList[idx].SetItemData(new PlayerCommonItemData()
                {
                    ItemIdx = funeIdx,
                    CommonItemData = new CommonItemData()
                    {
                        ItemType = EItemType.Fune,
                        ItemID = funeData.FuneID,
        
                    }
                }, null, null);
                idx++;
            }
        }
        
        public void RefreshExplain(bool isShowDetail)
        {
            var explainListData = new List<CommonItemData>();
            var drCard = CardManager.Instance.GetCardTable(CardDescFormData.CardIdx);
                    
            explainListData.AddRange(BattleCardManager.Instance.GetCardExplainList(drCard.Id));
                    
            if (isShowDetail)
            {
                var cardData =
                    CardManager.Instance.GetCard(CardDescFormData.CardIdx);
                        
                var idx = 0;
                foreach (var funeIdx in cardData.FuneIdxs)
                {
                    if(idx >= funeList.Count)
                        continue;

                    var funeData = FuneManager.Instance.GetFuneData(funeIdx);
                    explainListData.AddRange(
                        BattleBuffManager.Instance.GetBuffExplainList(funeData.FuneID));;
                        
                    idx++;
                }
            }
            
            explainList.SetData(explainListData);
            
            explainList.gameObject.transform.localPosition = hasFune ? pos1.localPosition : pos2.localPosition;
        }

    }
}