
using System;
using System.Collections.Generic;
using DG.Tweening;
using GameFramework.Event;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;


namespace RoundHero
{
    public enum ECardUseType
    {
        Raw, 
        Attack, 
        Move,
    }

    public enum ECardShowType
    {
        ShowToUnshow,
        UnshowToShow,
        ShowToShow,
    }
    
    public class BattleCardEntity : Entity
    {

        public BattleCardEntityData BattleCardEntityData { get; protected set; }

        [SerializeField]
        private CardItem CardItem;
        
        private int sortingOrder;
        private bool isShow;

        [SerializeField]
        private RectTransform cardRect;
        
        [SerializeField]
        private Canvas canvas;
        
        public GameObject ActionGO;

        [SerializeField]
        private GameObject moveGO;
        
        [SerializeField]
        private GameObject attackGO;
        
        [SerializeField]
        private GameObject moveCheckMark;
        
        [SerializeField]
        private GameObject attackCheckMark;
        
        [SerializeField]
        private InfoTrigger moveInfoTrigger;
        
        [SerializeField]
        private InfoTrigger attackInfoTrigger;

        [SerializeField] private GameObject ConfirmGO;

        
        [SerializeField] private Text ConfirmText;
        
        [SerializeField]
        private VideoTriggerItem videoTriggerItem;

        [SerializeField]
        private MoveGameObject moveGameObject;
        
        [SerializeField]
        private ScaleGameObject scaleGameObject;
        
        private Rect rect;
        private bool isInside;
        private bool isHand;
        private bool isUsing;

        //public int RawSiblingIdx;

        private void OnDisable()
        {
            foreach (var kv in BattleUnitManager.Instance.BattleUnitEntities)
            {
                kv.Value.UnShowTags();
            }
        }


        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            //GetComponent<Canvas>().sortingLayerName = "OverUISprite";
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            GameEntry.Event.Subscribe(RefreshCardInfoEventArgs.EventId, OnRefreshInfo);
            GameEntry.Event.Subscribe(RefreshBattleStateEventArgs.EventId, OnRefreshBattleState);
            
            BattleCardEntityData = userData as BattleCardEntityData;
            if (BattleCardEntityData == null)
            {
                Log.Error("Error BattleCardEntityData");
                return;
            }
            transform.SetParent(BattleController.Instance.Root);
            transform.position = Vector3.one * 100; 
            transform.localScale = Vector3.one;
            //GetComponent<Canvas>().sortingOrder = 1000;
            //var drCard = CardManager.Instance.GetCardTable(BattleCardEntityData.CardIdx);
            
            
            ActionGO.SetActive(false);
            ConfirmGO.SetActive(false);
            //
            //
            // attackCheckMark.SetActive(false);
            // moveCheckMark.SetActive(false);
            
        
            RefreshCardUseTypeInfo();

            videoTriggerItem.VideoFormData.AnimationPlayData.ShowPosition = EShowPosition.BattleLeft;
            var drCard = GameEntry.DataTable.GetCard(BattleCardEntityData.CardData.CardID);
            videoTriggerItem.VideoFormData.AnimationPlayData.GifType = drCard.CardType == ECardType.Unit ? EGIFType.Solider : EGIFType.Tactic;
            videoTriggerItem.VideoFormData.AnimationPlayData.ID = BattleCardEntityData.CardData.CardID;

        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            GameEntry.Event.Unsubscribe(RefreshCardInfoEventArgs.EventId, OnRefreshInfo);
            GameEntry.Event.Unsubscribe(RefreshBattleStateEventArgs.EventId, OnRefreshBattleState);
        }

        private void RefreshCardRect()
        {
            Vector3 topLeft = cardRect.transform.TransformPoint(cardRect.offsetMin);
            Vector3 bottomRight = cardRect.transform.TransformPoint(cardRect.offsetMax);
            //Vector3 bottomLeft = new Vector3(topLeft.x, bottomRight.y, 0);
            //Vector3 topRight = new Vector3(bottomRight.x, topLeft.y, 0);
            
            rect = new Rect(topLeft.x, topLeft.y, bottomRight.x - topLeft.x, bottomRight.y - topLeft.y);
            var screenPoint = RectTransformUtility.WorldToScreenPoint(AreaController.Instance.UICamera, rect.position);
            rect.position = screenPoint;
            rect.width *= 100;
            rect.height *= 100;

        }
        
        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if(BattleManager.Instance.BattleState != EBattleState.UseCard)
                return;
            
            if(!isHand)
                return;
            
            if (!isInside)
            {
                if(BattleCardManager.Instance.PointerCardIdx != -1)
                    return;
                
                isInside = rect.Contains(Input.mousePosition);
                if (isInside)
                {
                    //Log.Debug("inSide:" + BattleCardEntityData.CardIdx);
                    OnPointerEnter();
                }
            }
            
            if (isInside)
            {
                isInside = rect.Contains(Input.mousePosition);
                if (!isInside)
                {
                    //Log.Debug("no inSide:" + BattleCardEntityData.CardIdx);
                    OnPointerExit();
                }
            }
        }
        
        private void OnPointerEnter()
        { 
            if (TutorialManager.Instance.Check_SelectUnitCard(this) == ETutorialState.UnMatch &&
              TutorialManager.Instance.Check_SelectMoveCard(this) == ETutorialState.UnMatch &&
              TutorialManager.Instance.Check_SelectAttackCard(this) == ETutorialState.UnMatch)
            {
                return;
            }

            if (isUsing)
                return;


            //Log.Debug("Enter");
            if(BattleManager.Instance.BattleState != EBattleState.UseCard)
                return;

            if (GamePlayManager.Instance.GamePlayData.GameMode == EGamMode.PVE)
            {
                if(BattleManager.Instance.CurUnitCamp == EUnitCamp.Enemy)
                    return;
            }
            
            //isShow = true;
            ActionGO.SetActive(true);
            //transform.localPosition = new Vector3(transform.localPosition.x, BattleController.Instance.HandCardPos.localPosition.y + 140f, 0);
            ScaleCard(1, 1.2f, 0.01f);
            //transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            BattleCardManager.Instance.PointerCardIdx = BattleCardEntityData.CardData.CardIdx;
            BattleCardManager.Instance.SelectCardIdx = BattleCardEntityData.CardData.CardIdx;
            BattleCardManager.Instance.SelectCardHandOrder = BattleCardEntityData.HandSortingIdx;
            BattleCardManager.Instance.RefreshSelectCard();
            BattleCardManager.Instance.SetCardsPos();

            BattleManager.Instance.TempTriggerData.TriggerType = ETempTriggerType.UseBuff;
            BattleManager.Instance.TempTriggerData.TriggerBuffData.TriggerBuffType = TriggerBuffType.Card;
            BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx = BattleCardEntityData.CardIdx;
            
            RefreshCardRect();
            BattleManager.Instance.RefreshEnemyAttackData();

            foreach (var kv in BattleUnitManager.Instance.BattleUnitEntities)
            {
                
                kv.Value.ShowTacticHurtDisplayValues(kv.Value.UnitIdx);
                
            }
        }
        
        public void OnPointerExit()
        {
            
            if (TutorialManager.Instance.Check_SelectUnitCard(this) == ETutorialState.UnMatch &&
                TutorialManager.Instance.Check_SelectMoveCard(this) == ETutorialState.UnMatch &&
                TutorialManager.Instance.Check_SelectAttackCard(this) == ETutorialState.UnMatch)
            {
                return;
            }
            
            if(BattleManager.Instance.BattleState != EBattleState.UseCard)
                return;

            if (GamePlayManager.Instance.GamePlayData.GameMode == EGamMode.PVE)
            {
                if(BattleManager.Instance.CurUnitCamp == EUnitCamp.Enemy)
                    return;
            }

            
            
            
            BattleCardManager.Instance.UnSelectCard();
            BattleCardManager.Instance.RefreshSelectCard();
            BattleCardManager.Instance.SetCardsPos();
            
            BattleManager.Instance.TempTriggerData.TriggerType = ETempTriggerType.Null;
            BattleManager.Instance.TempTriggerData.TriggerBuffData.TriggerBuffType = TriggerBuffType.Empty;
            BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx = -1;
            
            
            BattleManager.Instance.RefreshEnemyAttackData();
            
            foreach (var kv in BattleUnitManager.Instance.BattleUnitEntities)
            {
                kv.Value.UnShowTags();
            }
            
            
        }

        public void UnSelectCard()
        {
            
            //isShow = false;
            ActionGO.SetActive(false);
            MoveCard(new Vector3(transform.localPosition.x, BattleController.Instance.HandCardPos.localPosition.y, 0),
                0.1f);
            
            //transform.localPosition = new Vector3(transform.localPosition.x, BattleController.Instance.HandCardPos.localPosition.y, 0);
            //transform.localScale = new Vector3(1f, 1f, 1f);
            ScaleCard(-1, 1, 0.01f);
            RefreshCardRect();
            BattleCardEntityData.CardData.CardUseType = ECardUseType.Raw;
            RefreshCardUseTypeInfo();
            moveInfoTrigger.HideInfo();
            attackInfoTrigger.HideInfo();
        }

        private Tween moveTween;
         
        public void MoveCard(Vector2 pos, float time)
        {
            moveGameObject.Move(this.transform.localPosition, new Vector3(pos.x, pos.y, 0), time);
            
            // if (moveTween != null)
            // {
            //     //moveTween.Pause();
            //     moveTween.Kill(false);
            //     moveTween = null;
            // }
            //
            // //transform.DOKill(false);
            // moveTween = transform.DOLocalMove(new Vector3(pos.x, pos.y, 0), time);
            // GameUtility.DelayExcute(time + 0.01f, () =>
            // {
            //     if (moveTween != null)
            //     {
            //         moveTween.Kill(true);
            //     }
            //     
            // });
            RefreshInHandCard(time);
        }
        
        private Tween scaleTween;
        public void ScaleCard(float from, float to, float time)
        {
            Vector3 startScale;
            if (from == -1)
            {
                startScale = transform.localScale; 
            }
            else
            {
                startScale = new Vector3(from, from, from);
            }
            
            scaleGameObject.Scale(startScale, new Vector3(to, to, to), time);
            // from = 1;
            // to = 1;
            // if (scaleTween != null)
            // {
            //     //scaleTween.Pause();
            //     scaleTween.Kill(false);
            //     scaleTween = null;
            // }
            //
            // //transform.DOKill(false);
            // if (from != -1)
            // {
            //     transform.localScale = new Vector3(from, from, from);
            // }
            //
            // scaleTween = transform.DOScale(to, time);
            // GameUtility.DelayExcute(time + 0.01f, () =>
            // {
            //     if (scaleTween != null)
            //     {
            //         scaleTween.Kill(false);
            //         //scaleTween = null;
            //     }
            //     
            // });

        }
        
        public void AcquireCard(Vector2 pos, float time)
        {
            MoveCard(pos, time);
            ScaleCard(0, 1, time);
            // transform.localScale = Vector3.zero;
            // transform.DOScale(Vector3.one, time);
           
            //RefreshInHandCard(time);
        }

        private void RefreshInHandCard(float time)
        {
            GameUtility.DelayExcute(time, () =>
            {
                isHand = true;
                RefreshCardRect();
                //RawSiblingIdx = gameObject.GetComponent<RectTransform>().GetSiblingIndex();
            });
        }
        
        public async void UseCard()
        {
            if(TutorialManager.Instance.Switch_SelectUnitCard(this) == ETutorialState.UnMatch &&
               TutorialManager.Instance.Switch_SelectMoveCard(this) == ETutorialState.UnMatch &&
               TutorialManager.Instance.Switch_SelectAttackCard(this) == ETutorialState.UnMatch)
                return;
            
            if (!Input.GetMouseButtonUp(0))
            {
                return;
            }
            
            if (GamePlayManager.Instance.GamePlayData.GameMode == EGamMode.PVE)
            {
                if(BattleManager.Instance.CurUnitCamp == EUnitCamp.Enemy)
                    return;
            }
            
            if(BattleManager.Instance.BattleState != EBattleState.UseCard)
                return;
            
            
            if(BattleCardEntityData.CardData.UnUse)
                return;

            BattleCardEntityData.CardData.CardUseType = ECardUseType.Raw;
            if(!BattleCardManager.Instance.PreUseCard(BattleCardEntityData.CardIdx))
                return;

            
            UseCardAnimation();
            

        }
        
        public void UseCardAnimation(int gridPosIdx = -1)
        {
            //ActionGO.SetActive(false);
            isInside = false;
            isHand = false;
            isUsing = true;
            
            BattleCardManager.Instance.SelectCardIdx = -1;
            BattleCardManager.Instance.SelectCardHandOrder = -1;
            BattleCardManager.Instance.PointerCardIdx = -1;

            
            
            // switch (BattleCardEntityData.CardData.CardUseType)
            // {
            //     case ECardUseType.Raw:
            //
            //         transform.DOLocalMove(BattleController.Instance.CenterPos.localPosition, 0.2f);
            //         //GetComponent<Canvas>().sortingOrder =  1000;
            //
            //         GameUtility.DelayExcute(0.4f, () =>
            //         {
            //             ToPassCard(0.2f);
            //
            //         });
            //         
            //         break;
            //     case ECardUseType.Attack:
            //         break;
            //     case ECardUseType.Move:
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException();
            // }

            //transform.DOLocalMove(BattleController.Instance.CenterPos.localPosition, 0.2f);

            //MoveCard(BattleController.Instance.CenterPos.localPosition, 0.2f);
            MoveCard(ECardPos.Default, ECardPos.Center, 0.2f);

            //GetComponent<Canvas>().sortingOrder =  1000;

            GameUtility.DelayExcute(0.4f, () =>
            {
                switch (BattleCardEntityData.CardData.CardDestination)
                {
                    case ECardDestination.Pass:
                        //ToPassCard(0.2f);
                        MoveCard(ECardPos.Default, ECardPos.Pass);
                        break;
                    case ECardDestination.Consume:
                        //ToConsumeCard(0.2f);
                        MoveCard(ECardPos.Default, ECardPos.Consume);
                        break;
                    case ECardDestination.StandBy:
                        //ToStandByCard(0.2f);
                        MoveCard(ECardPos.Default, ECardPos.StandBy);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                

            });

        }

        public void RemoveCard()
        {
            isHand = false;
            ScaleCard(-1, 1, 0.01f);
            //transform.localScale = Vector3.one;
            // transform.DOScale(BattleController.Instance.CenterPos.localPosition, 0.25f).OnComplete(() =>
            // {
            //     GameEntry.Entity.HideEntity(this);
            // });
            GameUtility.DelayExcute(0.25f, () =>
            {
                GameEntry.Entity.HideEntity(this);
            });
        }

        public void ShowInPassCard()
        {
            isHand = false;
            // transform.localPosition = BattleController.Instance.PassCardPos.localPosition;
            // transform.localScale = Vector3.one / 2f;
            
            MoveCard(ECardPos.Default, ECardPos.Pass, 0.01f);
            ScaleCard(-1, 0.5f, 0.01f);
        }
        
        public void ShowInStandByCard()
        {
            isHand = false;
            // transform.localPosition = BattleController.Instance.StandByCardPos.localPosition;
            // transform.localScale = Vector3.one / 2f;
            
            MoveCard(ECardPos.Default, ECardPos.StandBy, 0.01f);
            ScaleCard(-1, 0.5f, 0.01f);
        }
        
        public void ShowInConsumeCard()
        {
            isHand = false;
            // transform.localPosition = BattleController.Instance.ConsumeCardPos.localPosition;
            // transform.localScale = Vector3.one / 2f;
            
            MoveCard(ECardPos.Default, ECardPos.Consume, 0.01f);
            ScaleCard(-1, 0.5f, 0.01f);
        }
        
        

        

        public void MoveCard(ECardPos from, ECardPos to, float time = 0.4f)
        {
            if (from != ECardPos.Default)
            {
                transform.localPosition = Constant.Battle.CardPos[from];
            }
            
            MoveCard(Constant.Battle.CardPos[to], time);
            //transform.DOLocalMove(Constant.Battle.CardPos[to], time);

            var fromShow = from == ECardPos.Center || from == ECardPos.Hand;
            var toShow = to == ECardPos.Center || to == ECardPos.Hand;
            
            //transform.localScale = fromShow || !toShow ? Vector3.one : Vector3.zero;
            //transform.DOScale(toShow ? Vector3.one : Vector3.zero, time);
            ScaleCard(fromShow || !toShow ? 1 : 0, toShow ? 1 : 0, time);
            
            if (from == ECardPos.Hand)
            {
                isHand = false;
            }
            
            if (to == ECardPos.Hand)
            {
                RefreshInHandCard(time);
            }

            if (!toShow)
            {
                GameUtility.DelayExcute(time, () =>
                {
                    isUsing = false;
                    GameEntry.Entity.HideEntity(this);
                });
            }

        }
        // public void ToPassCard(float time)
        // {
        //     isHand = false;
        //     transform.DOLocalMove(BattleController.Instance.PassCardPos.localPosition, time);
        //     
        //     transform.localScale = Vector3.one;
        //     transform.DOScale(Vector3.zero, time).OnComplete(() =>
        //     {
        //         GameEntry.Entity.HideEntity(this);
        //     });
        // }
        
        // public void ToStandByCard(float time)
        // {
        //     isHand = false;
        //     transform.DOLocalMove(BattleController.Instance.StandByCardPos.localPosition, time);
        //     
        //     transform.localScale = Vector3.one;
        //     transform.DOScale(Vector3.zero, time).OnComplete(() =>
        //     {
        //         GameEntry.Entity.HideEntity(this);
        //     });
        // }
        
        // public void ToConsumeCard(float time)
        // {
        //     isHand = false;
        //     transform.DOLocalMove(BattleController.Instance.ConsumeCardPos.localPosition, time);
        //     
        //     transform.localScale = Vector3.one;
        //     transform.DOScale(Vector3.zero, time).OnComplete(() =>
        //     {
        //         GameEntry.Entity.HideEntity(this);
        //     });
        // }
        
        // public void StandByCardToHand(float time)
        // {
        //     transform.localPosition = BattleController.Instance.StandByCardPos.localPosition;
        //     transform.DOLocalMove(BattleController.Instance.HandCardPos.localPosition, time);
        //     
        //     transform.localScale = Vector3.zero;
        //     transform.DOScale(Vector3.one, time);
        //     
        //     RefreshInHandCard(time);
        // }
        
        // public void PassCardToHand(float time)
        // {
        //     transform.localPosition = BattleController.Instance.PassCardPos.localPosition;
        //     transform.DOLocalMove(BattleController.Instance.HandCardPos.localPosition, time);
        //     
        //     transform.localScale = Vector3.zero;
        //     transform.DOScale(Vector3.one, time);
        //     RefreshInHandCard(time);
        // }
        
        // public void PassCardToCenter(float time)
        // {
        //     transform.localPosition = BattleController.Instance.PassCardPos.localPosition;
        //     transform.DOLocalMove(BattleController.Instance.CenterPos.localPosition, time);
        //     
        //     transform.localScale = Vector3.zero;
        //     transform.DOScale(Vector3.one, time);
        //     
        //     
        // }
        
        
        // public void StandByToCenter(float time)
        // {
        //     transform.localPosition = BattleController.Instance.StandByCardPos.localPosition;
        //     transform.DOLocalMove(BattleController.Instance.CenterPos.localPosition, time);
        //     
        //     transform.localScale = Vector3.zero;
        //     transform.DOScale(Vector3.one, time);
        // }
        // public void ConsumeCardToHand(float time)
        // {
        //     transform.localPosition = BattleController.Instance.ConsumeCardPos.localPosition;
        //     transform.DOLocalMove(BattleController.Instance.HandCardPos.localPosition, time);
        //     
        //     transform.localScale = Vector3.zero;
        //     transform.DOScale(Vector3.one, time);
        //     
        // }
        

        // public void NewCardToHand(float time)
        // {
        //     transform.localScale = Vector3.one;
        //     transform.localPosition = BattleController.Instance.CenterPos.localPosition;
        //     transform.DOLocalMove(BattleController.Instance.HandCardPos.localPosition, time);
        //     RefreshInHandCard(time);
        //     
        // }
        
        // public void NewCardToStandBy(float time)
        // {
        //     isHand = false;
        //     transform.localScale = Vector3.one;
        //     transform.localPosition = BattleController.Instance.CenterPos.localPosition;
        //     transform.DOLocalMove(BattleController.Instance.StandByCardPos.localPosition, time);
        //     
        //     transform.DOScale(Vector3.zero, time).OnComplete(() =>
        //     {
        //         GameEntry.Entity.HideEntity(this);
        //     });
        //     
        // }
        
        // public void NewCardToPass(float time)
        // {
        //     isHand = false;
        //     transform.localScale = Vector3.one;
        //     transform.localPosition = BattleController.Instance.CenterPos.localPosition;
        //     transform.DOLocalMove(BattleController.Instance.PassCardPos.localPosition, time);
        //     
        //     transform.DOScale(Vector3.zero, time).OnComplete(() =>
        //     {
        //         GameEntry.Entity.HideEntity(this);
        //     });
        // }


        
        
        // public void OnPointerEnter(BaseEventData baseEventData)
        // {
        //     Log.Debug("Enter");
        //     if(BattleManager.Instance.BattleState != EBattleState.UseCard)
        //         return;
        //     
        //     BattleCardManager.Instance.PointerCardID = BattleCardEntityData.CardID;
        //     
        //     isShow = true;
        //     //BaseCard.ActionGO.SetActive(true);
        //     transform.localPosition = new Vector3(transform.localPosition.x, BattleController.Instance.HandCardPos.localPosition.y + 165f, 0);
        //     transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        //     //GetComponent<Canvas>().sortingOrder = sortingOrder + 100;
        //
        //     //var drCard = CardManager.Instance.GetCardTable(BattleCardEntityData.CardID);
        //     // var buffID = drCard.BuffIDs[0];
        //     // var buffData = BattleBuffManager.Instance.GetBuffData(drCard.BuffIDs[0]);
        //     //Wrong
        //     // if (Constant.Card.EffectMultiUnitsBuffIDs.Contains(buffData.DrBuff.Id))
        //     // {
        //     //     BattleManager.Instance.TempTriggerData.TriggerType = ETempUnitType.UseBuff;
        //     //     BattleManager.Instance.TempTriggerData.TriggerBuffData.CardID = BattleCardEntityData.CardID;
        //     //     BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffID = buffData.DrBuff.Id;
        //     //
        //     // }
        //     BattleManager.Instance.Refresh();
        //     
        // }
        
        // public void OnPointerExit(BaseEventData baseEventData)
        // {
        //     Log.Debug("Exit");
        //     if(BattleManager.Instance.BattleState != EBattleState.UseCard)
        //         return;
        //     
        //     
        //     BattleCardManager.Instance.PointerCardID = -1;
        //     isShow = false;
        //     //BaseCard.ActionGO.SetActive(false);
        //     transform.localPosition = new Vector3(transform.localPosition.x, BattleController.Instance.HandCardPos.localPosition.y, 0);
        //     transform.localScale = new Vector3(1f, 1f, 1f);
        //     //GetComponent<Canvas>().sortingOrder -=  100;
        //     
        //     //var drCard = CardManager.Instance.GetCardTable(BattleCardEntityData.CardID);
        //     //var buffID = drCard.BuffIDs[0];
        //     //Wrong
        //     // if (Constant.Card.EffectMultiUnitsBuffIDs.Contains(buffID))
        //     // {
        //     //     BattleManager.Instance.TempTriggerData.TriggerType = ETempUnitType.Null;
        //     //     BattleManager.Instance.TempTriggerData.TriggerBuffData.Clear();
        //     // }
        //     //FightManager.Instance.CalculateEnemyPaths();
        //     BattleManager.Instance.Refresh();
        //     
        // }
        
        public void SetSortingOrder(int sortingOrder, bool force = false)
        {
            //this.canvas.overrideSorting = true;
            this.canvas.sortingOrder = sortingOrder;
            // this.sortingOrder = sortingOrder;
            // if (!isShow || force)
            // {
            //     //GetComponent<Canvas>().sortingOrder = sortingOrder;
            // }
        }

        public void RefreshInfo()
        {
            CardItem.SetCard(BattleCardEntityData.CardData.CardID, BattleCardEntityData.CardData.CardIdx);
        }

        public void OnRefreshInfo(object sender, GameEventArgs e)
        {
            RefreshInfo();
        }
        
        public void OnRefreshBattleState(object sender, GameEventArgs e)
        {
            
            //RefreshCofirm();
        }

        public void RefreshCofirm()
        {
            var isConfirm = BattleCardManager.Instance.SelectCardIdx == BattleCardEntityData.CardData.CardIdx &&
                            ((BattleManager.Instance.BattleState == EBattleState.MoveGrid &&
                              BattleAreaManager.Instance.IsMoveGrid) ||
                             BattleManager.Instance.BattleState == EBattleState.ExchangeSelectGrid);
            
            ConfirmGO.SetActive(isConfirm);
            ActionGO.SetActive(!isConfirm);

            if (isConfirm)
            {
                if (BattleManager.Instance.BattleState == EBattleState.MoveGrid)
                {
                    ConfirmText.text = GameEntry.Localization.GetString(Constant.Localization.Button_ConfrimMove);
                }
                else if (BattleManager.Instance.BattleState == EBattleState.ExchangeSelectGrid)
                {
                    ConfirmText.text =
                        GameEntry.Localization.GetString(Constant.Localization.Button_ConfrimExchangeGrid);
                }
            }
            
            
        }

        public void Move()
        {
            if (TutorialManager.Instance.SwitchStep(ETutorialStep.CardSwitchMove) == ETutorialState.UnMatch)
            {
                return;
            }
            
            if (BattleCardEntityData.CardData.CardUseType == ECardUseType.Move)
            {
                
                BattleCardEntityData.CardData.CardUseType = ECardUseType.Raw;
            }
            else
            {

                //Log.Debug("Move");
                Action();
                BattleCardEntityData.CardData.CardUseType = ECardUseType.Move;
                BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr =
                    EBuffID.Spec_MoveUs.ToString();
            }

            RefreshCardUseTypeInfo();

        }
        
        public void Attack()
        {
            if (TutorialManager.Instance.SwitchStep(ETutorialStep.CardSwitchAttack) == ETutorialState.UnMatch)
            {
                return;
            }
            
            
            if (BattleCardEntityData.CardData.CardUseType == ECardUseType.Attack)
            {
                
                BattleCardEntityData.CardData.CardUseType = ECardUseType.Raw;
                
            }
            else
            {

                Log.Debug("Attack");
                Action();
                BattleCardEntityData.CardData.CardUseType = ECardUseType.Attack;
                BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr =
                    EBuffID.Spec_AttackUs.ToString();
                
            }
            RefreshCardUseTypeInfo();
        }
        

        public void RefreshCardUseTypeInfo()
        {
            if (BattleCardEntityData.CardData.CardID == 0)
            {
                var a = 5;
            }
            switch (BattleCardEntityData.CardData.CardUseType)
            {
                case ECardUseType.Raw:
                    CardItem.SetCard(BattleCardEntityData.CardData.CardID, BattleCardEntityData.CardData.CardIdx);
                    break;
                case ECardUseType.Attack:

                    CardItem.RefreshCardUI(
                        GameEntry.Localization.GetString(Constant.Localization.CardName_Attack),
                        GameEntry.Localization.GetString(Constant.Localization.CardDesc_Attack), 0);
                    break;
                case ECardUseType.Move:
                    CardItem.RefreshCardUI(
                        GameEntry.Localization.GetString(Constant.Localization.CardName_Move),
                        GameEntry.Localization.GetString(Constant.Localization.CardDesc_Move), 0);
                    break;
                default:
                    break;
            }
            
            SelectAction(BattleCardEntityData.CardData.CardUseType);
        }

        private void Action()
        {
            foreach (var kv in BattleUnitManager.Instance.BattleUnitEntities)
            {
                kv.Value.UnShowTags();
            }
            BattleManager.Instance.SetBattleState(EBattleState.TacticSelectUnit);
            BattleManager.Instance.TempTriggerData.TriggerBuffData.TriggerBuffType = TriggerBuffType.Card;
            BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx = BattleCardEntityData.CardIdx;
            
        }
        
        public void RefreshEnergy(int energy)
        {
            CardItem.RefreshEnergy(energy);
        }

        public void SelectAction(ECardUseType cardUseType)
        {
            switch (cardUseType)
            {
                case ECardUseType.Raw:
                    moveCheckMark.SetActive(false);
                    attackCheckMark.SetActive(false);
                    moveGO.SetActive(true);
                    attackGO.SetActive(true);
                    moveGO.GetComponent<Animation>().Play();
                    attackGO.GetComponent<Animation>().Play();
                    break;
                case ECardUseType.Attack:
                    moveCheckMark.SetActive(false);
                    moveGO.SetActive(true);
                    moveGO.GetComponent<Animation>().Play();
                    
                    attackCheckMark.SetActive(true);
                    attackGO.SetActive(false);
                    attackGO.GetComponent<Animation>().Stop();
                    break;
                case ECardUseType.Move:
                    attackCheckMark.SetActive(false);
                    attackGO.SetActive(true);
                    attackGO.GetComponent<Animation>().Play();
                    
                    moveCheckMark.SetActive(true);
                    moveGO.SetActive(false);
                    moveGO.GetComponent<Animation>().Stop();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cardUseType), cardUseType, null);
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

            var moveGridPosIdx = new Dictionary<int, int>();
            if (battleState == EBattleState.MoveGrid)
            {
                
                foreach (var kv in BattleAreaManager.Instance.MoveGrids)
                {
                    moveGridPosIdx.Add(kv.Key, kv.Value.GridPosIdx);
                }
                
                foreach (var kv in BattleAreaManager.Instance.MoveGridPosIdxs)
                {
                    var moveGrid = BattleAreaManager.Instance.MoveGrids[kv.Key];
                    moveGrid.GridPosIdx = kv.Value;
                }
            }

            
            
            BattleCardManager.Instance.UseCard(BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx);
            
            if (battleState == EBattleState.MoveGrid)
            {
                foreach (var kv in BattleAreaManager.Instance.MoveGrids)
                {
                    kv.Value.GridPosIdx = moveGridPosIdx[kv.Key];

                }
            }
            
            if (battleState == EBattleState.MoveGrid)
            {
                BattleAreaManager.Instance.ClearMoveGrid();
            }
            else if (battleState == EBattleState.ExchangeSelectGrid)
            {
                BattleAreaManager.Instance.ClearExchangeGrid();
            }

        }
    }
}