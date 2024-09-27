using GameFramework;
using GameFramework.Event;
using TMPro;
using UnityEngine;


namespace RoundHero
{

    public class BattleForm : UGuiForm
    {
        [SerializeField] public Transform StandByCardPos;
        [SerializeField] public Transform PassCardPos;
        [SerializeField] public Transform HandCardPos;
        [SerializeField] public Transform ConsumeCardPos;
        
        [SerializeField] private TextMeshProUGUI round;
        [SerializeField] private TextMeshProUGUI standByCardCount;
        [SerializeField] private TextMeshProUGUI passCardCount;
        [SerializeField] private TextMeshProUGUI consumeCardCount;
        [SerializeField] private TextMeshProUGUI energy;
        [SerializeField] private TextMeshProUGUI heroHP;
        [SerializeField] private TextMeshProUGUI test;
        [SerializeField] private TextMeshProUGUI coin;
        
        [SerializeField] private EnergyBuffBarUItem energyBuffBarUItem;

        private bool isEndRound = false;
        // private List<A> As = new ();
        //
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            GameEntry.Event.Subscribe(RefreshBattleUIEventArgs.EventId, OnRefreshBattleUI);

            RefreshEnergy();
            
            energyBuffBarUItem.Init(BattlePlayerManager.Instance.PlayerData.BattleHero);
            
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
        }

        public void OnRefreshBattleUI(object sender, GameEventArgs e)
        {
            RefreshUI();
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

                var fightSoliderData = FightManager.Instance.GetUnitByID(solider.ID) as Data_BattleSolider;
                var soliderEntity = BattleUnitManager.Instance.GetUnitByID(solider.ID) as BattleSoliderEntity;
                if(fightSoliderData == null)
                    return;

                var card = BattleManager.Instance.GetCard(solider.BattleSoliderEntityData.BattleSoliderData.CardID);

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
                FightManager.Instance.GetTotalDelta(BattleHeroManager.Instance.HeroEntity.ID, EHeroAttribute.CurHP);
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
            coin.text = BattleHeroManager.Instance.BattleHeroData.Attribute.GetAttribute(EHeroAttribute.Coin) + "";
            
            coin.text += "-" + FightManager.Instance.GetTotalDelta(BattleHeroManager.Instance.HeroEntity.ID, EHeroAttribute.Coin);
        }
        
        private void RefreshRound()
        {
            round.text = Utility.Text.Format(GameEntry.Localization.GetString(Constant.Localization.UI_BattleRound),
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
            Close();
            BattleMapManager.Instance.NextStep();
            GamePlayManager.Instance.ProcedureGamePlay.EndBattle();
        }
    }
}