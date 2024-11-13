using DG.Tweening;
using GameFramework;
using GameFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace RoundHero
{

    public class BattleForm : UGuiForm
    {
        [SerializeField] public Transform StandByCardPos;
        [SerializeField] public Transform PassCardPos;
        [SerializeField] public Transform HandCardPos;
        [SerializeField] public Transform ConsumeCardPos;
        
        [SerializeField] private Text round;
        [SerializeField] private Text standByCardCount;
        [SerializeField] private Text passCardCount;
        [SerializeField] private Text consumeCardCount;
        [SerializeField] private TextMeshProUGUI energy;
        [SerializeField] private TextMeshProUGUI heroHP;
        [SerializeField] private TextMeshProUGUI test;
        [SerializeField] private TextMeshProUGUI coin;
        
        [SerializeField] private EnergyBuffBarUItem energyBuffBarUItem;
        
        [SerializeField] private GameObject tipsNode;

        [SerializeField] private Text tips;
        
        private ProcedureBattle procedureBattle;

        private bool isEndRound = false;
        // private List<A> As = new ();
        //
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            procedureBattle = (ProcedureBattle)userData;
            
            GameEntry.Event.Subscribe(RefreshBattleUIEventArgs.EventId, OnRefreshBattleUI);
            GameEntry.Event.Subscribe(RefreshRoundEventArgs.EventId, OnRefreshRound);
            GameEntry.Event.Subscribe(RefreshActionCampEventArgs.EventId, OnRefreshActionCamp);

            RefreshEnergy();
            
            //energyBuffBarUItem.Init(BattlePlayerManager.Instance.PlayerData.BattleHero);
            tipsNode.SetActive(false);
        }

        private void ShowRoundTips(int round)
        {
            tipsNode.GetComponent<Animation>().Stop();
            tipsNode.GetComponent<Animation>().Play();
            tipsNode.SetActive(true);
            tips.text = GameEntry.Localization.GetLocalizedString(Constant.Localization.Tips_Round, round);
            // GameUtility.DelayExcute(2f, () =>
            // {
            //     tips.text = "";
            //     tipsNode.SetActive(false);
            // });
        }
        
        private void ShowActionTips(bool isUs)
        {
            tipsNode.GetComponent<Animation>().Stop();
            tipsNode.GetComponent<Animation>().Play();
            tipsNode.SetActive(true);
            tips.text = GameEntry.Localization.GetString(isUs
                ? Constant.Localization.Tips_UsAction
                : Constant.Localization.Tips_EnemyAction);
            // GameUtility.DelayExcute(2f, () =>
            // {
            //     tips.text = "";
            //     tipsNode.SetActive(false);
            // });
        }

        // protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        // {
        //     base.OnUpdate(elapseSeconds, realElapseSeconds);
        //     if (isEndRound && BattleManager.Instance.BattleState == EBattleState.UseCard)
        //     {
        //         BattleManager.Instance.EndRound();
        //         isEndRound = false;
        //     }
        // }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            GameEntry.Event.Unsubscribe(RefreshBattleUIEventArgs.EventId, OnRefreshBattleUI);
            GameEntry.Event.Unsubscribe(RefreshRoundEventArgs.EventId, OnRefreshRound);
            GameEntry.Event.Unsubscribe(RefreshActionCampEventArgs.EventId, OnRefreshActionCamp);
        }

        public void OnRefreshBattleUI(object sender, GameEventArgs e)
        {
            RefreshUI();
        }
        
        public void OnRefreshActionCamp(object sender, GameEventArgs e)
        {
            var ne = e as RefreshActionCampEventArgs;
            ShowActionTips(ne.IsUs);
        }
        
        public void OnRefreshRound(object sender, GameEventArgs e)
        {
            ShowRoundTips(BattleManager.Instance.BattleData.Round + 1);
        }

        private void RefreshUI()
        {
            RefreshEnergy();
            RefreshRound();
            RefreshCardCount();
            RefreshHeroHP();
            RefreshCoin();
            RefreshLinks();

        }

        private void RefreshLinks()
        {
            if (BattleAreaManager.Instance.CurPointGridPosIdx != -1)
            {
                var solider =
                    BattleUnitManager.Instance.GetUnitByGridPosIdx(BattleAreaManager.Instance.CurPointGridPosIdx) as BattleSoliderEntity;
                if(solider == null)
                    return;

                var fightSoliderData = BattleFightManager.Instance.GetUnitByID(solider.ID) as Data_BattleSolider;
                var soliderEntity = BattleUnitManager.Instance.GetUnitByID(solider.ID) as BattleSoliderEntity;
                if(fightSoliderData == null)
                    return;

                var card = BattleManager.Instance.GetCard(solider.BattleSoliderEntityData.BattleSoliderData.CardIdx);

                var name = "";
                var desc = "";
                GameUtility.GetCardText(card.CardID, ref name, ref desc);

                test.text = solider.ID + "-" + name + "\n";
                var list = fightSoliderData.Links;
                foreach (var linkSoliderID in list)
                {
                    test.text += linkSoliderID + ",";
                }
                
                test.text += soliderEntity.CurHP +
                             "/" + soliderEntity.MaxHP;
            }
        }

        private void RefreshHeroHP()
        {
            heroHP.text =
                BattlePlayerManager.Instance.PlayerData.BattleHero.Attribute.GetAttribute(EHeroAttribute.CurHeart) + "/" +
                BattlePlayerManager.Instance.PlayerData.BattleHero.Attribute.GetAttribute(EHeroAttribute.MaxHeart) + "-" +
                BattlePlayerManager.Instance.PlayerData.BattleHero.CurHP + "/" +
                BattlePlayerManager.Instance.PlayerData.BattleHero.MaxHP;

            var hpDelta =
                BattleFightManager.Instance.GetTotalDelta(HeroManager.Instance.HeroEntity.ID, EHeroAttribute.CurHP);
            heroHP.text += "   " + ((hpDelta > 0) ? "+" + hpDelta : hpDelta);
        }

        private void RefreshEnergy()
        {
            // energy.text =
            //     BattleHeroManager.Instance.BattleHeroData.Attribute.GetAttribute(EHeroAttribute.CurEnergy) + "/" +
            //     BattleHeroManager.Instance.BattleHeroData.Attribute.GetAttribute(EHeroAttribute.MaxEnergy);
        }
        
        private void RefreshCoin()
        {
            coin.text = HeroManager.Instance.BattleHeroData.Attribute.GetAttribute(EHeroAttribute.Coin) + "";
            
            coin.text += "-" + BattleFightManager.Instance.GetTotalDelta(HeroManager.Instance.HeroEntity.ID, EHeroAttribute.Coin);
        }
        
        private void RefreshRound()
        {
            round.text = Utility.Text.Format(GameEntry.Localization.GetString(Constant.Localization.UI_Round),
                BattleManager.Instance.BattleData.Round + 1);
        }

        private void RefreshCardCount()
        {
            standByCardCount.text = BattlePlayerManager.Instance.BattlePlayerData.StandByCards.Count + "";
            passCardCount.text = BattlePlayerManager.Instance.BattlePlayerData.PassCards.Count + "";
            consumeCardCount.text = BattlePlayerManager.Instance.BattlePlayerData.ConsumeCards.Count + "";
            
        }

        public void EndRound()
        {
            BattleManager.Instance.EndRound();
            ShowActionTips(false);
            //isEndRound = true;
        }
        
        
        
        public void ConfirmUseCard()
        {
            var battleState = BattleManager.Instance.BattleState;
            if (battleState != EBattleState.MoveGrid && battleState != EBattleState.ExchangeSelectGrid)
            {
                return;
            }
 
            BattleManager.Instance.BattleState = EBattleState.UseCard;
            BattleCardManager.Instance.CardEntities[BattleManager.Instance.TempTriggerData.TriggerBuffData.CardID].UseCardAnimation();
            BattleCardManager.Instance.UseCard(BattleManager.Instance.TempTriggerData.TriggerBuffData.CardID);
            
            if (battleState == EBattleState.MoveGrid)
            {
                BattleAreaManager.Instance.ClearMoveGrid();
            }
            else if (battleState == EBattleState.ExchangeSelectGrid)
            {
                BattleAreaManager.Instance.ClearExchangeGrid();
            }

        }

        

        public void ResetMoveGrid()
        {
            if (BattleManager.Instance.BattleState != EBattleState.MoveGrid)
            {
                return;
            }
            
            BattleAreaManager.Instance.ResetMoveGrid();
        }

        public void ConsumeCard()
        {
            BattleCardManager.Instance.ConsumeCardForms();
        }
        
        public void AddNewCard()
        {
            BattleCardManager.Instance.AddNewCardForms();
        }

        public void TestSuccess()
        {
            procedureBattle.EndBattle();
            BattleMapManager.Instance.NextStep();
            
        }
    }
}