﻿using System.Collections.Generic;
using System.Linq;
using GameFramework;
using GameFramework.Event;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public class PlayerInfoForm : UGuiForm
    {
        [SerializeField] private Text hpText;
        [SerializeField] private Text coinText;
        [SerializeField] private Image heroIcon;
        [SerializeField] private List<GameObject> heroHPs;
        public LoopGridView blessGridView;
        
        private List<int> blessList = new ();
        
        public void ShowCards()
        {
            GameEntry.UI.OpenUIForm(UIFormId.CardsForm);
        }
        
        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            
            blessGridView.InitGridView(0, OnBlessIconGetItemByRowColumn);
            
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            GameEntry.Event.Subscribe(RefreshPlayerInfoEventArgs.EventId, OnRefreshPlayerInfo);
            GameEntry.Event.Fire(null, RefreshPlayerInfoEventArgs.Create());

        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            GameEntry.Event.Unsubscribe(RefreshPlayerInfoEventArgs.EventId, OnRefreshPlayerInfo);
        }

        private async void OnRefreshPlayerInfo(object sender, GameEventArgs e)
        {
            coinText.text = BattlePlayerManager.Instance.PlayerData.Coin.ToString();
            hpText.text = BattlePlayerManager.Instance.PlayerData.BattleHero.CurHP.ToString();
            var drHero = GameEntry.DataTable.GetHero(BattlePlayerManager.Instance.PlayerData.BattleHero.HeroID);
            heroIcon.sprite = await AssetUtility.GetHeroIcon(drHero.Id);
            
            for (int i = 0; i < heroHPs.Count; i++)
            {
                if (i < drHero.Heart)
                {
                    heroHPs[i].SetActive(true);
                }
                else
                {
                    heroHPs[i].SetActive(false);
                }
            }

            blessList = BlessManager.Instance.BlessDatas.Keys.ToList();
            blessGridView.SetListItemCount(blessList.Count);
            blessGridView.RefreshAllShownItem();
            
        }
        
        LoopGridViewItem OnBlessIconGetItemByRowColumn(LoopGridView gridView, int itemIndex,int row,int column)
        {
            
            var blessData = BlessManager.Instance.GetBless(blessList[itemIndex]);
            if (blessData == null)
            {
                return null;
            }

            var item = gridView.NewListViewItem("BlessIconItem");

            var itemScript = item.GetComponent<BlessIconItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            
            itemScript.SetItemData(blessData, itemIndex, row, column);
            return item;
        }
        
        public void HPTipsInfo(InfoFormParams infoFormParams)
        {
            infoFormParams.Name = GameEntry.Localization.GetString(Utility.Text.Format(Constant.Localization.HeroName,
                BattleHeroManager.Instance.BattleHeroData.ID));
            infoFormParams.Desc = Utility.Text.Format(infoFormParams.Desc,
                BattleHeroManager.Instance.BattleHeroData.CurHeart + "/" +
                BattleHeroManager.Instance.BattleHeroData.MaxHeart,
                BattleHeroManager.Instance.BattleHeroData.CurHP + "/" +
                BattleHeroManager.Instance.BattleHeroData.MaxHP);
            
            infoFormParams.Desc += "\n" + GameEntry.Localization.GetString(Utility.Text.Format(Constant.Localization.HeroDesc,
                BattleHeroManager.Instance.BattleHeroData.ID));
        }
    }
}