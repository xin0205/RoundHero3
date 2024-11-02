using System;
using System.Collections.Generic;
using UGFExtensions.Await;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class TreasureFormData
    {
        public int RandomSeed;
    }
    
    public class TreasureForm : UGuiForm
    {
        private BattleEventData battleEventData;

        [SerializeField]
        private List<BattleEventItem> BattleEventItems;
        
        private TreasureFormData treasureFormData;

        private int curAcquireIdx;
        private int curItemIdx;
        private int acquireIdx;
        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            
            treasureFormData = (TreasureFormData)userData;
            if (treasureFormData == null)
            {
                Log.Warning("TreasureFormData is null.");
                return;
            }

            battleEventData = BattleEventManager.Instance.GenerateTreasureEvent(treasureFormData.RandomSeed);

            //text.text = battleEventData.BattleEventExpressionType + "-" + battleEventData.BattleEvent;

            for (int i = 0; i <  BattleEventItems.Count; i++)
            {
                BattleEventItems[i].Init(i, battleEventData.BattleEventItemDatas[i], OnClickItem);
            }

            
        }

        private void NextAcquireItem()
        {
            curAcquireIdx += 1;
            if (curAcquireIdx >= acquireIdx)
            {
                Close();
                return;
            }
            
            var battleEventItemDatas = battleEventData.BattleEventItemDatas[curItemIdx];
            var battleEventItemData = battleEventItemDatas[curAcquireIdx];

            switch (battleEventItemData.EventType)
            {
                case EEventType.Card_Remove:
                    RemoveCard();
                    break;
                case EEventType.Card_Change:
                    ChangeCard();
                    break;
                case EEventType.Card_Copy:
                    CopyCard();
                    break;
                case EEventType.Random_UnitCard:
                case EEventType.Random_TacticCard:
                case EEventType.Random_Fune:
                case EEventType.Random_Bless:
                    ShowSelectAcquireForm();
                    break;
                case EEventType.Appoint_UnitCard:
                case EEventType.Appoint_TacticCard:
                case EEventType.Appoint_Fune:
                case EEventType.Appoint_Bless:
                case EEventType.AddCoin:
                case EEventType.AddHeroMaxHP:
                case EEventType.AddHeroCurHP:
                case EEventType.NegativeCard:
                case EEventType.SubCoin:
                case EEventType.SubHeroMaxHP:
                case EEventType.SubHeroCurHP:
                    AcquireItem();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnClickItem(int itemIdx)
        {
            curItemIdx = itemIdx;
            var battleEventItemDatas = battleEventData.BattleEventItemDatas[curItemIdx];
            acquireIdx = battleEventItemDatas.Count;

            curAcquireIdx = -1;
            NextAcquireItem();

        }
        
        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            BattleMapManager.Instance.NextStep();
        }

        private void AcquireItem()
        {
            var battleEventItemDatas = battleEventData.BattleEventItemDatas[curItemIdx];
            BattleEventManager.Instance.AcquireEventItem(battleEventItemDatas[curAcquireIdx]);
            
            if (Constant.BattleEvent.EventTypeItemTypeMap.ContainsKey(battleEventItemDatas[curAcquireIdx].EventType))
            {
                var acquireFormData = new AcquireFormData()
                {
                    
                    ItemType = Constant.BattleEvent.EventTypeItemTypeMap[battleEventItemDatas[curAcquireIdx].EventType],
                    ClickCloseAction = CheckClose,

                };

                if (Constant.Hero.AttributeItemTypes.Contains(acquireFormData.ItemType))
                {
                    var value = battleEventItemDatas[curAcquireIdx].EventValues[0];
                    acquireFormData.Value = value > 0 ? "+" + value : value.ToString();
                }
                else
                {
                    acquireFormData.ItemID = battleEventItemDatas[curAcquireIdx].EventValues[0];
                }
                
                GameEntry.UI.OpenUIForm(UIFormId.AcquireForm, acquireFormData);
            }

            
        }

        private void ShowSelectAcquireForm()
        {
            var battleEventItemDatas = battleEventData.BattleEventItemDatas[curItemIdx];
            var battleEventItemData = battleEventItemDatas[curAcquireIdx];
            
            var selectAcquireFormParams = new SelectAcquireFormParams();

            foreach (var value in battleEventItemData.EventValues)
            {
                var selectAcquireItemData = new SelectAcquireItemData()
                {
                    ItemType = Constant.BattleEvent.EventTypeItemTypeMap[battleEventItemData.EventType],
                    ItemID = value,
                };
                selectAcquireFormParams.SelectAcquireItemDatas.Add(selectAcquireItemData);
                
            }

            selectAcquireFormParams.OnClick = OnSelectAcquireClick;
            
            
            GameEntry.UI.OpenUIForm(UIFormId.SelectAcquireForm, selectAcquireFormParams);
            
        }

        private void OnSelectAcquireClick(int selectIdx)
        {
            var battleEventItemDatas = battleEventData.BattleEventItemDatas[curItemIdx];
            var battleEventItemData = battleEventItemDatas[curAcquireIdx];
            BattleEventManager.Instance.AcquireEventItem(battleEventItemData, selectIdx);
        }

        private CardsForm cardsForm;
        private async void CopyCard()
        {
            var uiForm = await GameEntry.UI.OpenUIFormAsync(UIFormId.CardsForm, new CardsFormParams()
            {
                Tips = GameEntry.Localization.GetString(Constant.Localization.Tips_CopyCard),
                ShowCardTypes = new List<ECardType>()
                {
                    ECardType.Unit,
                    ECardType.Tactic,
                    ECardType.State,
                },
                OnClickAction = CopyCard,
                IsShowAllFune = false,
                
            });
            cardsForm = uiForm.Logic as CardsForm;
            
        }
        
        private void CopyCard(int cardIdx)
        {
            GameEntry.UI.OpenConfirm(new ConfirmFormParams()
            {
                Message = GameEntry.Localization.GetString(Constant.Localization.Message_ConfirmCopyCard),
                OnConfirm = () =>
                {
                    AcquireItem();
                    GameEntry.UI.CloseUIForm(cardsForm);
                    
                }
            });
            
            
        }

        private async void RemoveCard()
        {
            var uiForm = await GameEntry.UI.OpenUIFormAsync(UIFormId.CardsForm, new CardsFormParams()
            {
                Tips = GameEntry.Localization.GetString(Constant.Localization.Tips_RemoveCard),
                ShowCardTypes = new List<ECardType>()
                {
                    ECardType.Unit,
                    ECardType.Tactic,
                    ECardType.State,
                },
                OnClickAction = RemoveCard,
                IsShowAllFune = false,
                
            });
            cardsForm = uiForm.Logic as CardsForm;
            
        }
        
        private void RemoveCard(int cardIdx)
        {
            GameEntry.UI.OpenConfirm(new ConfirmFormParams()
            {
                Message = GameEntry.Localization.GetString(Constant.Localization.Message_ConfirmRemoveCard),
                OnConfirm = () =>
                {
                    AcquireItem();
                    GameEntry.UI.CloseUIForm(cardsForm);
                    CheckClose();
                }
            });
            
            
        }
        
        private async void ChangeCard()
        {
            var uiForm = await GameEntry.UI.OpenUIFormAsync(UIFormId.CardsForm, new CardsFormParams()
            {
                Tips = GameEntry.Localization.GetString(Constant.Localization.Tips_ChangeCard),
                ShowCardTypes = new List<ECardType>()
                {
                    ECardType.Unit,
                    ECardType.Tactic,
                    ECardType.State,
                },
                OnClickAction = ChangeCard,
                IsShowAllFune = false,
                
            });
            cardsForm = uiForm.Logic as CardsForm;
            
        }
        
        private void ChangeCard(int cardIdx)
        {
            GameEntry.UI.OpenConfirm(new ConfirmFormParams()
            {
                Message = GameEntry.Localization.GetString(Constant.Localization.Message_ConfirmChangeCard),
                OnConfirm = () =>
                {
                    AcquireItem();
                    GameEntry.UI.CloseUIForm(cardsForm);

                }
            });
            
            
        }
        
        private void CheckClose()
        {
     
            NextAcquireItem();
            
        }
    }
}