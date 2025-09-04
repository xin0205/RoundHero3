

using System;
using System.Collections.Generic;
using System.Net.Mime;
using UGFExtensions.Await;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace RoundHero
{
    public class BattleModeRewardForm : UGuiForm
    {
        private ProcedureStart procedureBattle;
        
        private List<SelectAcquireItemData> selectAcquireItemDatas = new();
        [SerializeField] private List<SelectAcquireItem> selectAcquireItems;
        private Dictionary<EItemType, int> RewardRatios = new Dictionary<EItemType, int>();
        private SceneEntity startSelectEntity;
        private PlayerInfoForm playerInfoForm;
        private List<int> selectedIdxs = new List<int>();
        [SerializeField] private Text tips;

        private Random random;
        protected override async void OnOpen(object userData)
        {
            base.OnOpen(userData);
            
            procedureBattle = (ProcedureStart)userData;
            
            RewardRatios.Clear();
            selectAcquireItemDatas.Clear();
            selectedIdxs.Clear();
            
            startSelectEntity = await GameEntry.Entity.ShowSceneEntityAsync("StartSelect");
            // var formResult = await GameEntry.UI.OpenUIFormAsync(UIFormId.PlayerInfoForm, this);
            // playerInfoForm = formResult.Logic as PlayerInfoForm;

            var rewardRandomSeed = GamePlayManager.Instance.GamePlayData.BattleModeProduce.RewardRandomSeed;
            random = new Random(rewardRandomSeed);
            
            var ratio = 0;
            foreach (var kv in Constant.Battle.BattleModeRewardRatios)
            {
                ratio += kv.Value;
                RewardRatios.Add(kv.Key, ratio);
            }
            var drBlesses = GameEntry.DataTable.GetDataTable<DRBless>();
            var blessIDs = new List<int>();
            foreach (var drBless in drBlesses)
            {
                bool isContinue = false;
                foreach (var kv in PlayerManager.Instance.PlayerData.BlessDatas)
                {
                    if (kv.Value.BlessID == drBless.BlessID)
                    {
                        isContinue = true;
                        break;
                    }
                        
                }

                if(isContinue)
                    continue;
                
                blessIDs.Add(drBless.Id);
            }
            
            
            for (int i = 0; i < 9; i++)
            {
                var randomType = random.Next(0, 100);
                foreach (var kv in RewardRatios)
                {
                    if (randomType < kv.Value)
                    {
                        var selectAcquireItemData = new SelectAcquireItemData()
                        {
                            ItemType = kv.Key,
                        };
                        selectAcquireItemDatas.Add(selectAcquireItemData);
                        var randomIdx = 0;
                        switch (kv.Key)
                        {
                            case EItemType.UnitCard:
                                var drUnitCards = GameEntry.DataTable.GetCards(ECardType.Unit);
                                randomIdx = random.Next(0, drUnitCards.Length);
                                selectAcquireItemData.ItemID = drUnitCards[randomIdx].Id;
                                break;
                            case EItemType.TacticCard:
                                var drTacticCards = GameEntry.DataTable.GetCards(ECardType.Tactic);
                                randomIdx = random.Next(0, drTacticCards.Length);
                                selectAcquireItemData.ItemID = drTacticCards[randomIdx].Id;
                                break;
                            case EItemType.Bless:
                                
                                randomIdx = random.Next(0, blessIDs.Count);
                                selectAcquireItemData.ItemID = drBlesses[randomIdx].Id;
                                blessIDs.RemoveAt(randomIdx);
                                break;
                            case EItemType.Fune:
                                var drFunes = GameEntry.DataTable.GetBuffs(EBuffType.Fune);
                                randomIdx = random.Next(0, drFunes.Count);
                                selectAcquireItemData.ItemID = drFunes[randomIdx].Id;
                                break;
                            case EItemType.AddMaxHP:
                                break;
                            case EItemType.RemoveCard:
                                break;
                            case EItemType.AddCardFuneSlots:
                                break;

                        }

                        break;

                    }
                }
            }

            RefreshItems();
            SetTips();
        }
        
        public void RefreshItems()
        {
            for (int i = 0; i < selectAcquireItems.Count; i++)
            {
                var selectAcquireItem = selectAcquireItems[i];
                if (i >= selectAcquireItemDatas.Count)
                {
                    selectAcquireItem.gameObject.SetActive(false);
                    continue;
                }
                
                selectAcquireItem.gameObject.SetActive(true);
                selectAcquireItem.SetItemData(selectAcquireItemDatas[i],
                    OnSelectItem, i);
            }

        }

        protected void SetTips()
        {
            tips.text = GameEntry.Localization.GetLocalizedString(Constant.Localization.Tips_BattleModeRewardSelectItem,
                selectedIdxs.Count, Constant.BattleMode.MaxRewardCount);
        }

        public void OpenCardAndFuneForm()
        {
            GameEntry.UI.OpenUIForm(UIFormId.CardAndFuneForm);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            GameEntry.Entity.HideEntity(startSelectEntity);
            //GameEntry.UI.CloseUIForm(playerInfoForm);
        }

        public void OnSelectItem(int selectIdx)
        {
            if (selectedIdxs.Count >= Constant.BattleMode.MaxRewardCount)
            {
                GameEntry.UI.OpenLocalizationMessage(Constant.Localization.Message_RewardMaxCount,
                    Constant.BattleMode.MaxRewardCount);
                return;
            }
                
            
            var selectAcquireItem = selectAcquireItems[selectIdx];
            selectAcquireItem.SetSelect();
            
            var selectAcquireItemData = selectAcquireItemDatas[selectIdx];
            
            var idx = 0;
            switch (selectAcquireItemData.ItemType)
            {
                case EItemType.TacticCard:
                case EItemType.UnitCard:
                    idx = CardManager.Instance.GetIdx();
                    CardManager.Instance.CardDatas.Add(idx, new Data_Card(idx, selectAcquireItemData.ItemID));
                    break;
                case EItemType.Bless:
                    idx = BlessManager.Instance.GetIdx();
                    var drBless = GameEntry.DataTable.GetBless(selectAcquireItemData.ItemID);
                    BlessManager.Instance.BlessDatas.Add(idx, new Data_Bless(idx, drBless.BlessID));
                    GameEntry.Event.Fire(null, RefreshPlayerInfoEventArgs.Create());
                    break;
                case EItemType.Fune:
                    idx = FuneManager.Instance.GetIdx();
                    FuneManager.Instance.FuneDatas.Add(idx, new Data_Fune(idx, selectAcquireItemData.ItemID));
                    BattlePlayerManager.Instance.PlayerData.UnusedFuneIdxs.Add(idx);
                    break;
                
                default:
                    break;
            }

            selectedIdxs.Add(selectIdx);
            SetTips();
        }
        
        
        public void ConfirmReward()
        {
            foreach (var selectAcquireItemData in selectAcquireItemDatas)
            {
                if(!selectAcquireItemData.IsSelected)
                    continue;
                
            }
            
            
            GamePlayManager.Instance.GamePlayData.BattleModeProduce.Session += 1;
            GamePlayManager.Instance.GamePlayData.BattleModeProduce.BattleModeStage = BattleModeStage.Battle;
            
            GameEntry.UI.CloseUIForm(this);
            

            
            GameEntry.Event.Fire(null,
                GamePlayStartGameEventArgs.Create());
        }

        public void Back()
        {
            Close();
            GamePlayManager.Instance.GamePlayData.BattleModeProduce.SelectIdxs = new List<int>(selectedIdxs);
            procedureBattle.Start();
        }
    }
}