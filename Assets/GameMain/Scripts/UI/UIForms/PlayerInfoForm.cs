using System.Collections.Generic;
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
            GameEntry.UI.OpenUIForm(UIFormId.CardAndFuneForm);
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
            GameEntry.Event.Subscribe(RefreshBattleUIEventArgs.EventId, OnRefreshBattleUI);
            GameEntry.Event.Fire(null, RefreshPlayerInfoEventArgs.Create());

        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            GameEntry.Event.Unsubscribe(RefreshPlayerInfoEventArgs.EventId, OnRefreshPlayerInfo);
            GameEntry.Event.Unsubscribe(RefreshBattleUIEventArgs.EventId, OnRefreshBattleUI);
        }

        private void OnRefreshPlayerInfo(object sender, GameEventArgs e)
        {
            Refresh();

        }
        
        private void OnRefreshBattleUI(object sender, GameEventArgs e)
        {
            Refresh();

        }

        private async void Refresh()
        {
            coinText.text = BattlePlayerManager.Instance.PlayerData.Coin.ToString();
            hpText.text = BattlePlayerManager.Instance.PlayerData.BattleHero.CurHP + "/" + BattlePlayerManager.Instance.PlayerData.BattleHero.MaxHP;
            var battleHeroData = BattlePlayerManager.Instance.PlayerData.BattleHero;
            heroIcon.sprite = await AssetUtility.GetHeroIcon(battleHeroData.ID);
            
            for (int i = 0; i < heroHPs.Count; i++)
            {
                if (i < battleHeroData.CurHeart)
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
                HeroManager.Instance.BattleHeroData.ID));
            infoFormParams.Desc = Utility.Text.Format(infoFormParams.Desc,
                HeroManager.Instance.BattleHeroData.CurHeart + "/" +
                HeroManager.Instance.BattleHeroData.MaxHeart,
                HeroManager.Instance.BattleHeroData.CurHP + "/" +
                HeroManager.Instance.BattleHeroData.MaxHP);
            
            infoFormParams.Desc += "\n" + GameEntry.Localization.GetString(Utility.Text.Format(Constant.Localization.HeroDesc,
                HeroManager.Instance.BattleHeroData.ID));
        }
    }
}