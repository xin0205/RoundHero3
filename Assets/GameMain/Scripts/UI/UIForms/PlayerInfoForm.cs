using GameFramework.Event;
using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public class PlayerInfoForm : UGuiForm
    {
        public Text coinText;
        [SerializeField] private Image heroIcon;
        
        public void ShowCards()
        {
            GameEntry.UI.OpenUIForm(UIFormId.CardsForm);
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
            var drHero = GameEntry.DataTable.GetHero(BattlePlayerManager.Instance.PlayerData.BattleHero.HeroID);
            heroIcon.sprite = await AssetUtility.GetHeroIcon(drHero.Id);
        }
    }
}