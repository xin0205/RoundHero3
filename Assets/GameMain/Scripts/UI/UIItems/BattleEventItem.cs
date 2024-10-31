using System;
using System.Collections.Generic;
using GameFramework;
using TMPro;
using UGFExtensions.Await;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoundHero
{
    public class BattleEventItem : MonoBehaviour
    {
        [SerializeField]
        private Text text;

        private List<BattleEventItemData> battleGameEventItemDatas;

        [SerializeField] private UnityEvent clickEvent;

        public void Init(List<BattleEventItemData> battleGameEventItemDatas)
        {
            this.battleGameEventItemDatas = battleGameEventItemDatas;

            text.text = "";
            var eventValues = new List<string>();
            
            foreach (var battleGameEventItemData in battleGameEventItemDatas)
            {
                eventValues.Clear();
                var eventTypeStr = GameEntry.Localization.GetString(battleGameEventItemData.EventType.ToString());

                foreach (var val in battleGameEventItemData.EventValues)
                {
                    eventValues.Add(val.ToString());
                }
                

                var changeValue = false;
                foreach (var kv in Constant.BattleEvent.ItemTypeEventTypeMap)
                {
                    if (kv.Value.Contains(battleGameEventItemData.EventType))
                    {
                        var name = "";
                        var desc = "";
                        GameUtility.GetItemText(kv.Key, battleGameEventItemData.EventValues[0], ref name, ref desc);
                        eventValues[0] = name;
                        changeValue = true;
                    }
                }

                
                
                text.text += GameEntry.Localization.GetLocalizedStrings(eventTypeStr, eventValues);

                // var names = new List<string>();
                // foreach (var value in battleGameEventItemData.EventValues)
                // {
                //     GameUtility.GetItemText();
                // }
                //
                // text.text += "-";
            }
            
            
        }
        
        private CardsForm cardsForm;

        public async void OnClick()
        {
            GameEntry.UI.OpenConfirm(new ConfirmFormParams()
            {
                Message = GameEntry.Localization.GetString(Constant.Localization.Message_ConfirmAcquire) + "\n" + text.text.ToString(),
                OnConfirm = () =>
                {
                    foreach (var battleGameEventItemData in battleGameEventItemDatas)
                    {
                        switch (battleGameEventItemData.EventType)
                        {
                            case EEventType.Card_Remove:
                                break;
                            case EEventType.Card_Change:
                                break;
                            case EEventType.Card_Copy:
                                var uiForm = await GameEntry.UI.OpenUIFormAsync(UIFormId.CardsForm, new CardsFormParams()
                                {
                                    Tips = GameEntry.Localization.GetString(Constant.Localization.Tips_CardAddFune),
                                    ShowCardTypes = new List<ECardType>()
                                    {
                                        ECardType.Unit,
                                    },
                                    OnClickAction = CopyCard,
                                    IsShowAllFune = false,
                
                                });
                                cardsForm = uiForm.Logic as CardsForm;
                                
                                break;
                            case EEventType.Random_UnitCard:
                                break;
                            case EEventType.Random_TacticCard:
                                break;
                            case EEventType.Random_Fune:
                                break;
                            case EEventType.Random_Bless:
                                break;
                            case EEventType.Appoint_UnitCard:
                                break;
                            case EEventType.Appoint_TacticCard:
                                break;
                            case EEventType.Appoint_Fune:
                                break;
                            case EEventType.Appoint_Bless:
                                break;
                            case EEventType.AddCoin:
                                break;
                            case EEventType.AddHeroMaxHP:
                                break;
                            case EEventType.AddHeroCurHP:
                                break;
                            case EEventType.NegativeCard:
                                break;
                            case EEventType.SubCoin:
                                break;
                            case EEventType.SubHeroMaxHP:
                                break;
                            case EEventType.SubHeroCurHP:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        battleGameEventItemData.EventType
                        
                        BattleEventManager.Instance.AcquireEventItem(battleGameEventItemData);
                    }
                    clickEvent?.Invoke();
                    
                    
                    
                }
            });
            
            
            
        }
        
        public void CopyCard(int cardIdx)
        {
            
            GameEntry.UI.OpenConfirm(new ConfirmFormParams()
            {
                Message = GameEntry.Localization.GetString(Constant.Localization.Message_ConfirmCopyCard),
                OnConfirm = () =>
                {
                    var cardData = CardManager.Instance.GetCard(cardIdx);
                    var newCardIdx = PlayerManager.Instance.PlayerData.CardIdx++;
                    CardManager.Instance.CardDatas.Add(newCardIdx, new Data_Card(newCardIdx, cardData.CardID));
                    
                    GameEntry.UI.CloseUIForm(cardsForm);
                    
                    Close();
                }
            });
            
            
        }
        
        

    }
}