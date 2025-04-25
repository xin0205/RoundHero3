using GameFramework;
using GameFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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
        [SerializeField] private Text heroHP;
        [SerializeField] private TextMeshProUGUI test;
        [SerializeField] private TextMeshProUGUI coin;
        
        [SerializeField] private EnergyBuffBarUItem energyBuffBarUItem;
        
        [SerializeField] private GameObject tipsNode;

        [SerializeField] private GameObject endRoundNode;
        
        [SerializeField] private Text tips;
        [SerializeField] private Text randomSeed;
        //[SerializeField] private Text coreInfo;
        
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
            GameEntry.Event.Subscribe(ShowUnitActionUIEventArgs.EventId, OnShowUnitActionUI);
            //GameEntry.Event.Subscribe(SwitchActionCampEventArgs.EventId, OnSwitchActionCamp);
            
            RefreshEnergy();
            
            //energyBuffBarUItem.Init(BattlePlayerManager.Instance.PlayerData.BattleHero);
            tipsNode.SetActive(false);
            
            randomSeed.text = GamePlayManager.Instance.GamePlayData.RandomSeed.ToString();

            AreaController.Instance.UICore = heroHP.gameObject;
            
            AreaController.Instance.Canvas = this.GetComponent<Canvas>();
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
            GameEntry.Event.Unsubscribe(ShowUnitActionUIEventArgs.EventId, OnShowUnitActionUI);
            //GameEntry.Event.Unsubscribe(SwitchActionCampEventArgs.EventId, OnSwitchActionCamp);
        }

        public void OnRefreshBattleUI(object sender, GameEventArgs e)
        {
            RefreshUI();
        }
        
        // public void OnSwitchActionCamp(object sender, GameEventArgs e)
        // {
        //     var ne = e as SwitchActionCampEventArgs;
        //     if (GamePlayManager.Instance.GamePlayData.GameMode == EGamMode.PVE)
        //     {
        //         if (ne.UnitCamp == EUnitCamp.Enemy)
        //         {
        //             endRoundNode.SetActive(false);
        //         }
        //         else
        //         {
        //             endRoundNode.SetActive(true);
        //         }
        //         
        //     }
        //     else
        //     {
        //         if (GamePlayManager.Instance.GamePlayData.PlayerData.UnitCamp == EUnitCamp.Player1)
        //         {
        //             if (ne.UnitCamp == EUnitCamp.Player1)
        //             {
        //                 endRoundNode.SetActive(true);
        //             }
        //             else
        //             {
        //                 endRoundNode.SetActive(false);
        //             }
        //         }
        //         if (GamePlayManager.Instance.GamePlayData.PlayerData.UnitCamp == EUnitCamp.Player2)
        //         {
        //             if (ne.UnitCamp == EUnitCamp.Player2)
        //             {
        //                 endRoundNode.SetActive(true);
        //             }
        //             else
        //             {
        //                 endRoundNode.SetActive(false);
        //             }
        //         }
        //
        //     }
        // }
        
        public void OnShowUnitActionUI(object sender, GameEventArgs e)
        {
            var ne = e as ShowUnitActionUIEventArgs;
            
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(ne.UnitPosition);
 
            // 检查是否在摄像机的视野内
            if (screenPosition.z > 0)
            {
                // 将屏幕坐标转换为UI坐标（假定屏幕左下角为原点）
                RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)AreaController.Instance.Canvas.transform, screenPosition, Camera.main, out Vector2 localPosition);
                // 现在localPosition包含了相对于Canvas的UI坐标
                
            }
        }
        
        public void OnRefreshActionCamp(object sender, GameEventArgs e)
        {
            var ne = e as RefreshActionCampEventArgs;
            ShowActionTips(ne.IsUs);

            endRoundNode.SetActive(ne.IsUs);
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
            //coreInfo.text = HeroManager.Instance.BattleHeroData.CurHP + "/" + HeroManager.Instance.BattleHeroData.CacheHPDelta;
        }

        private void RefreshLinks()
        {
            if (BattleAreaManager.Instance.CurPointGridPosIdx != -1)
            {
                var solider =
                    BattleUnitManager.Instance.GetUnitByGridPosIdx(BattleAreaManager.Instance.CurPointGridPosIdx) as BattleSoliderEntity;
                if(solider == null)
                    return;

                var fightSoliderData = BattleFightManager.Instance.GetUnitByIdx(solider.UnitIdx) as Data_BattleSolider;
                var soliderEntity = BattleUnitManager.Instance.GetUnitByIdx(solider.UnitIdx) as BattleSoliderEntity;
                if(fightSoliderData == null)
                    return;

                var card = BattleManager.Instance.GetCard(solider.BattleSoliderEntityData.BattleSoliderData.CardIdx);

                var name = "";
                var desc = "";
                GameUtility.GetCardText(card.CardID, ref name, ref desc);

                test.text = solider.UnitIdx + "-" + name + "\n";
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
            //BattlePlayerManager.Instance.PlayerData.BattleHero.Attribute.GetAttribute(EHeroAttribute.CurHeart) + "/" +
            //BattlePlayerManager.Instance.PlayerData.BattleHero.Attribute.GetAttribute(EHeroAttribute.MaxHeart) + "-" +
            heroHP.text =
                
                BattlePlayerManager.Instance.PlayerData.BattleHero.CurHP + "/" +
                BattlePlayerManager.Instance.PlayerData.BattleHero.MaxHP;

            var hpDelta = BattleFightManager.Instance.PlayerData.BattleHero.CurHP -
                          HeroManager.Instance.BattleHeroData.CurHP;
                          ;
                ;
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
            
            coin.text += "-" + BattleFightManager.Instance.GetTotalDelta(HeroManager.Instance.BattleHeroData.Idx, EHeroAttribute.Coin);
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
            if (GamePlayManager.Instance.GamePlayData.GameMode == EGamMode.PVE)
            {
                GameEntry.Event.Fire(null, SwitchActionCampEventArgs.Create(EUnitCamp.Enemy));
            }
            else
            {
                if (BattleManager.Instance.CurUnitCamp == EUnitCamp.Player1)
                {
                    GameEntry.Event.Fire(null, SwitchActionCampEventArgs.Create(EUnitCamp.Player2));
                }
                else if (BattleManager.Instance.CurUnitCamp == EUnitCamp.Player2)
                {
                    GameEntry.Event.Fire(null, SwitchActionCampEventArgs.Create(EUnitCamp.Player1));
                }
            }
            
        }
        
        
        
        public void ConfirmUseCard()
        {
            var battleState = BattleManager.Instance.BattleState;
            if (battleState != EBattleState.MoveGrid && battleState != EBattleState.ExchangeSelectGrid)
            {
                return;
            }
 
            BattleManager.Instance.SetBattleState(EBattleState.UseCard);
            BattleCardManager.Instance.CardEntities[BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx].UseCardAnimation();
            BattleCardManager.Instance.UseCard(BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx);
            
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
        
        public void ExitBattleTest()
        {
            BattleManager.Instance.EndBattleTest();
            
            
        }
        
        public void StandByCardTipsInfo(InfoFormParams infoFormParams)
        {
            infoFormParams.Name = GameEntry.Localization.GetString(Constant.Localization.Info_StandByCardName);
            infoFormParams.Desc = GameEntry.Localization.GetString(Constant.Localization.Info_StandByCardDesc);
            
        }
        
        public void PassCardTipsInfo(InfoFormParams infoFormParams)
        {
            infoFormParams.Name = GameEntry.Localization.GetString(Constant.Localization.Info_PassCardName);
            infoFormParams.Desc = GameEntry.Localization.GetString(Constant.Localization.Info_PassCardDesc);
            
        }
        
        public void ConsumeCardTipsInfo(InfoFormParams infoFormParams)
        {
            infoFormParams.Name = GameEntry.Localization.GetString(Constant.Localization.Info_ConsumeCardName);
            infoFormParams.Desc = GameEntry.Localization.GetString(Constant.Localization.Info_ConsumeCardDesc);
            
        }
    }
}