using System.Collections.Generic;
using System.Threading.Tasks;
using GameFramework;
using GameFramework.Event;
using SuperScrollView;
using TMPro;
using UGFExtensions.Await;
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
        [SerializeField] private Text heroHP;
        [SerializeField] private Text coreHPDelta;
        [SerializeField] private TextMeshProUGUI test;
        [SerializeField] private TextMeshProUGUI coin;
        
        [SerializeField] private EnergyBuffBarUItem energyBuffBarUItem;
        
        [SerializeField] private GameObject tipsNode;

        [SerializeField] private GameObject endRoundNode;
        [SerializeField] private GameObject reactionNode;
        
        [SerializeField] private Text tips;
        [SerializeField] private Text randomSeed;
        //[SerializeField] private Text coreInfo;
        
        [SerializeField] private Text battleSession;
        [SerializeField] private Text enemyCount;
        
        private ProcedureBattle procedureBattle;
        
        [SerializeField] private InfoTrigger resetActionInfoTrigger;

        private bool isEndRound = false;
        
        
        public LoopGridView hpDeltaDatas;
        
        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            
            hpDeltaDatas.InitGridView(0, OnHPDeltaDatasByRowColumn);
        }
        
        LoopGridViewItem OnHPDeltaDatasByRowColumn(LoopGridView gridView, int itemIndex,int row,int column)
        {
            var item = gridView.NewListViewItem("HPDeltaDataItem");

            var itemScript = item.GetComponent<HPDeltaDataItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
                
            }

            itemScript.SetItemData(
                BattleFightManager.Instance.RoundFightData.HPDeltaDict[PlayerManager.Instance.PlayerData.UnitCamp][itemIndex],
                itemIndex, row, column);
            return item;
        }
        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            procedureBattle = (ProcedureBattle)userData;
            
            GameEntry.Event.Subscribe(RefreshBattleUIEventArgs.EventId, OnRefreshBattleUI);
            GameEntry.Event.Subscribe(RefreshRoundEventArgs.EventId, OnRefreshRound);
            GameEntry.Event.Subscribe(RefreshActionCampEventArgs.EventId, OnRefreshActionCamp);
            //GameEntry.Event.Subscribe(ShowUnitActionUIEventArgs.EventId, OnShowUnitActionUI);
            //GameEntry.Event.Subscribe(SwitchActionCampEventArgs.EventId, OnSwitchActionCamp);
            
            RefreshEnergy();

            resetActionInfoTrigger.SetDescParams(new List<string>()
            {
                BattleManager.Instance.BattleData.ResetActionTimes.ToString(),
                Constant.Battle.ResetActionTimes.ToString()
            });
            
            //energyBuffBarUItem.Init(BattlePlayerManager.Instance.PlayerData.BattleHero);
            tipsNode.SetActive(false);
            endRoundNode.SetActive(false);
            reactionNode.SetActive(false);
            
            randomSeed.text = GamePlayManager.Instance.GamePlayData.RandomSeed.ToString();

            AreaController.Instance.UICore = heroHP.gameObject;
            AreaController.Instance.BattleFormRoot = this.gameObject;
            
            //AreaController.Instance.Canvas = this.GetComponent<Canvas>();

            if (GamePlayManager.Instance.GamePlayData.IsTutorialBattle)
            {
                BattleManager.Instance.TutorialStep = ETutorialStep.Start;
                GameEntry.Event.Fire(null, RefreshTutorialEventArgs.Create());
            }

            battleSession.gameObject.SetActive(false);
            enemyCount.gameObject.SetActive(false);
            
            if (GamePlayManager.Instance.GamePlayData.PVEType == EPVEType.Battle)
            {
                battleSession.gameObject.SetActive(true);

                battleSession.text = GameEntry.Localization.GetLocalizedString(Constant.Localization.Tips_BattleSession,
                    GamePlayManager.Instance.GamePlayData.BattleModeProduce.Session + 1, Constant.BattleMode.MaxBattleCount);
                
            }


            ShowHPDeltaData(false);
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

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            hpDeltaDatas.gameObject.SetActive(Input.GetKey(KeyCode.W));
            if (Input.GetKeyDown(KeyCode.W))
            {
            
                hpDeltaDatas.SetListItemCount(BattleFightManager.Instance.RoundFightData
                    .HPDeltaDict[PlayerManager.Instance.PlayerData.UnitCamp].Count);
                hpDeltaDatas.RefreshAllShownItem();
            }
        }

        public void ShowHPDeltaData(bool isShow)
        {
            hpDeltaDatas.gameObject.SetActive(isShow);
            if (isShow)
            {

                hpDeltaDatas.SetListItemCount(BattleFightManager.Instance.RoundFightData
                    .HPDeltaDict[PlayerManager.Instance.PlayerData.UnitCamp].Count);
                hpDeltaDatas.RefreshAllShownItem();
            }
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            
            base.OnClose(isShutdown, userData);
            GameEntry.Event.Unsubscribe(RefreshBattleUIEventArgs.EventId, OnRefreshBattleUI);
            GameEntry.Event.Unsubscribe(RefreshRoundEventArgs.EventId, OnRefreshRound);
            GameEntry.Event.Unsubscribe(RefreshActionCampEventArgs.EventId, OnRefreshActionCamp);
           //GameEntry.Event.Unsubscribe(ShowUnitActionUIEventArgs.EventId, OnShowUnitActionUI);
            //GameEntry.Event.Unsubscribe(SwitchActionCampEventArgs.EventId, OnSwitchActionCamp);
        }

        public void OnRefreshBattleUI(object sender, GameEventArgs e)
        {
            RefreshUI();
            hpDeltaDatas.SetListItemCount(BattleFightManager.Instance.RoundFightData
                                .HPDeltaDict[PlayerManager.Instance.PlayerData.UnitCamp].Count);
                            hpDeltaDatas.RefreshAllShownItem();
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
        
        // public void OnShowUnitActionUI(object sender, GameEventArgs e)
        // {
        //     var ne = e as ShowUnitActionUIEventArgs;
        //     
        //     Vector3 screenPosition = Camera.main.WorldToScreenPoint(ne.UnitPosition);
        //
        //     // 检查是否在摄像机的视野内
        //     if (screenPosition.z > 0)
        //     {
        //         // 将屏幕坐标转换为UI坐标（假定屏幕左下角为原点）
        //         RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)AreaController.Instance.Canvas.transform, screenPosition, Camera.main, out Vector2 localPosition);
        //         // 现在localPosition包含了相对于Canvas的UI坐标
        //         
        //     }
        // }
        
        public void OnRefreshActionCamp(object sender, GameEventArgs e)
        {
            var ne = e as RefreshActionCampEventArgs;
            //ShowActionTips(ne.IsUs);

            endRoundNode.SetActive(ne.IsUs);
            reactionNode.SetActive(ne.IsUs);
        }
        
        public void OnRefreshRound(object sender, GameEventArgs e)
        {
            //ShowRoundTips(BattleManager.Instance.BattleData.Round + 1);
        }

        private void RefreshUI()
        {
            RefreshEnergy();
            //RefreshRound();
            RefreshCardCount();
            RefreshHeroHP();
            //RefreshCoin();
            //RefreshLinks();
            //coreInfo.text = HeroManager.Instance.BattleHeroData.CurHP + "/" + HeroManager.Instance.BattleHeroData.CacheHPDelta;
            resetActionInfoTrigger.SetDescParams(new List<string>()
            {
                BattleManager.Instance.BattleData.ResetActionTimes.ToString(),
                Constant.Battle.ResetActionTimes.ToString()
            });
            
            if (GamePlayManager.Instance.GamePlayData.PVEType == EPVEType.Battle)
            {
                var _enemyCount = 0;
                foreach (var kv in BattleEnemyManager.Instance.EnemyGenerateData.RoundGenerateUnitCount)
                {
                    _enemyCount += kv.Value;
                }
                
                enemyCount.gameObject.SetActive(true);
                enemyCount.text = GameEntry.Localization.GetLocalizedString(Constant.Localization.Tips_EnemyCount,
                    _enemyCount);

            }
        }

        // private void RefreshLinks()
        // {
        //     if (BattleAreaManager.Instance.CurPointGridPosIdx != -1)
        //     {
        //         var solider =
        //             BattleUnitManager.Instance.GetUnitByGridPosIdx(BattleAreaManager.Instance.CurPointGridPosIdx) as BattleSoliderEntity;
        //         if(solider == null)
        //             return;
        //
        //         var fightSoliderData = BattleFightManager.Instance.GetUnitByIdx(solider.UnitIdx) as Data_BattleSolider;
        //         var soliderEntity = BattleUnitManager.Instance.GetUnitByIdx(solider.UnitIdx) as BattleSoliderEntity;
        //         if(fightSoliderData == null)
        //             return;
        //
        //         var card = BattleManager.Instance.GetCard(solider.BattleSoliderEntityData.BattleSoliderData.CardIdx);
        //
        //         var name = "";
        //         var desc = "";
        //         GameUtility.GetCardText(card.CardID, ref name, ref desc);
        //
        //         test.text = solider.UnitIdx + "-" + name + "\n";
        //         var list = fightSoliderData.Links;
        //         foreach (var linkSoliderID in list)
        //         {
        //             test.text += linkSoliderID + ",";
        //         }
        //         
        //         test.text += soliderEntity.CurHP +
        //                      "/" + soliderEntity.MaxHP;
        //     }
        // }

        private void RefreshHeroHP()
        {
            //BattlePlayerManager.Instance.PlayerData.BattleHero.Attribute.GetAttribute(EHeroAttribute.CurHeart) + "/" +
            //BattlePlayerManager.Instance.PlayerData.BattleHero.Attribute.GetAttribute(EHeroAttribute.MaxHeart) + "-" +
            heroHP.text =
                BattlePlayerManager.Instance.PlayerData.BattleHero.CurHP + "/" +
                BattlePlayerManager.Instance.PlayerData.BattleHero.MaxHP;

            var hpDelta = BattleFightManager.Instance.PlayerData.BattleHero.CurHP -
                          HeroManager.Instance.BattleHeroData.CurHP;
                          
            coreHPDelta.text = "   " + ((hpDelta > 0) ? "+" + hpDelta : hpDelta);
        }

        private void RefreshEnergy()
        {
            // energy.text =
            //     BattleHeroManager.Instance.BattleHeroData.Attribute.GetAttribute(EHeroAttribute.CurEnergy) + "/" +
            //     BattleHeroManager.Instance.BattleHeroData.Attribute.GetAttribute(EHeroAttribute.MaxEnergy);
        }
        
        private void RefreshCoin()
        {
            // coin.text = HeroManager.Instance.BattleHeroData.Attribute.GetAttribute(EHeroAttribute.Coin) + "";
            //
            // coin.text += "-" + BattleFightManager.Instance.GetTotalDelta(HeroManager.Instance.BattleHeroData.Idx, EHeroAttribute.Coin);
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
            if (TutorialManager.Instance.SwitchStep(ETutorialStep.EndRound) == ETutorialState.UnMatch)
            {
                return;
            }
            
            if(BattleManager.Instance.BattleState != EBattleState.UseCard)
                return;
            
            BattleManager.Instance.RecordLastActionBattleData();
            BattleManager.Instance.EndRound();
            //ShowActionTips(false);
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
            var cardIdx = BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx;
            
            var cardEntity = BattleCardManager.Instance.GetCardEntity(cardIdx);
            BattleCardManager.Instance.UseCard(cardIdx);
            //符文，打出，回到抽牌顶端 需要在UseCard之后
            cardEntity.UseCardAnimation();

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
            if(BattleManager.Instance.BattleState != EBattleState.UseCard)
                return;
            
            if (TutorialManager.Instance.IsTutorial() && !TutorialManager.Instance.CheckTutorialEnd())
                return;
            
            if (GamePlayManager.Instance.GamePlayData.PVEType == EPVEType.Battle)
            {
                procedureBattle.EndBattleMode(EBattleResult.Success);
            }
            if (GamePlayManager.Instance.GamePlayData.PVEType == EPVEType.Test)
            {
                procedureBattle.EndBattle(EBattleResult.Success);
            }

            BattleMapManager.Instance.NextStep();
            
        }
        
        public void ExitBattle()
        {
            // if(BattleManager.Instance.BattleState != EBattleState.UseCard)
            //     return;
            
            // if (TutorialManager.Instance.IsTutorial() && !TutorialManager.Instance.CheckTutorialEnd())
            //     return;
            
            GameEntry.UI.OpenConfirm(new ConfirmFormParams()
            {
                Message = GameEntry.Localization.GetString(Constant.Localization.Message_ConfirmExitBattle),
                OnConfirm = () =>
                {
                    BattleManager.Instance.EndBattle(EBattleResult.Empty);

                },
                
            });
            
           
            
            
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

        public void ShowActionSort(bool showActionSort)
        {
            BattleEnemyManager.Instance.ShowActionSort(showActionSort);
        }

        public async void ResetAction()
        {
            if (TutorialManager.Instance.IsTutorial())
            {
                return;

            }

            if (GamePlayManager.Instance.GamePlayData.BattleActionDataList.Count == 0)
            {
                GameEntry.UI.OpenMessage(GameEntry.Localization.GetString(Constant.Localization.Message_UnResetAction));
                return;
            }
            
            if (GamePlayManager.Instance.GamePlayData.BattleData.ResetActionTimes <= 0)
            {
                GameEntry.UI.OpenMessage(GameEntry.Localization.GetString(Constant.Localization.Message_ResetActionTimesNotEnough));
                return;
            }
            
            GamePlayManager.Instance.GamePlayData.LastRoundBattleData.Clear();

            BattleManager.Instance.Destory();
            BattleManager.Instance.Subscribe();
            await ResetArea();

        }
        
        public async Task ResetArea()
        {
            if(GamePlayManager.Instance.GamePlayData.BattleActionDataList.Count <= 0)
                return;

            var battleActionData =
                GamePlayManager.Instance.GamePlayData.BattleActionDataList[
                    GamePlayManager.Instance.GamePlayData.BattleActionDataList.Count - 1];
            var battleData = battleActionData.BattleData;

            GamePlayManager.Instance.GamePlayData.PlayerData = battleActionData.PlayerData.Copy();
            
            GamePlayManager.Instance.GamePlayData.ClearPlayerDataList();
            GamePlayManager.Instance.GamePlayData.AddPlayerData(GamePlayManager.Instance.GamePlayData.PlayerData);
            
            //GamePlayManager.Instance.GamePlayData.LastActionPlayerData.Clear();

            battleData.ResetActionTimes -= 1;
            GamePlayManager.Instance.GamePlayData.BattleData = battleData.Copy();
            BattlePlayerManager.Instance.SetCurPlayer(EUnitCamp.Player1);
            await GamePlayManager.Instance.ShowBattleData(battleData);
            GamePlayManager.Instance.GamePlayData.BattleActionDataList.Remove(battleActionData);

            DataManager.Instance.Save();
        }
        
        public void Setting()
        {
            GameEntry.UI.OpenUIFormAsync(UIFormId.SettingForm, this);

        }
    }
}