
using DG.Tweening;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public enum ECardUseType
    {
        Raw, 
        Attack, 
        Move,
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

        private Rect rect;
        private bool isInside;
        private bool isHand;

        //public int RawSiblingIdx;

        
        

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            //GetComponent<Canvas>().sortingLayerName = "OverUISprite";
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            GameEntry.Event.Subscribe(RefreshCardInfoEventArgs.EventId, OnRefreshInfo);
            
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
            var drCard = CardManager.Instance.GetCardTable(BattleCardEntityData.CardIdx);
            
            CardItem.SetCard(BattleCardEntityData.CardData.CardID);
            
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            GameEntry.Event.Unsubscribe(RefreshCardInfoEventArgs.EventId, OnRefreshInfo);
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
                    OnPointerEnter();
                }
            }
            
            if (isInside)
            {
                isInside = rect.Contains(Input.mousePosition);
                if (!isInside)
                {
                    OnPointerExit();
                }
            }
        }
        
        private void OnPointerEnter()
        {
            Log.Debug("Enter");
            if(BattleManager.Instance.BattleState != EBattleState.UseCard)
                return;

            if (GamePlayManager.Instance.GamePlayData.GameMode == EGamMode.PVE)
            {
                if(BattleManager.Instance.CurUnitCamp == EUnitCamp.Enemy)
                    return;
            }
            
            isShow = true;
            ActionGO.SetActive(true);
            transform.localPosition = new Vector3(transform.localPosition.x, BattleController.Instance.HandCardPos.localPosition.y + 50f, 0);
            transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            BattleCardManager.Instance.CurSelectCardIdx = BattleCardEntityData.CardData.CardIdx; 
            BattleCardManager.Instance.RefreshSelectCard();
            
            RefreshCardRect();
            BattleManager.Instance.RefreshEnemyAttackData();
        }
        
        public void OnPointerExit()
        {
            Log.Debug("Exit");
            if(BattleManager.Instance.BattleState != EBattleState.UseCard)
                return;

            if (GamePlayManager.Instance.GamePlayData.GameMode == EGamMode.PVE)
            {
                if(BattleManager.Instance.CurUnitCamp == EUnitCamp.Enemy)
                    return;
            }
            
            
            BattleCardManager.Instance.PointerCardIdx = -1;
            isShow = false;
            ActionGO.SetActive(false);
            transform.localPosition = new Vector3(transform.localPosition.x, BattleController.Instance.HandCardPos.localPosition.y, 0);
            transform.localScale = new Vector3(1f, 1f, 1f);
            if (BattleCardManager.Instance.CurSelectCardIdx == BattleCardEntityData.CardData.CardIdx)
            {
                BattleCardManager.Instance.CurSelectCardIdx = -1;
            }
            BattleCardManager.Instance.RefreshSelectCard();

            RefreshCardRect();
            BattleManager.Instance.RefreshEnemyAttackData();
            
        }

        public void MoveCard(Vector2 pos, float time)
        {
            transform.DOKill(false);
            transform.DOLocalMove(new Vector3(pos.x, pos.y, 0), time);
            RefreshInHandCard(time);
        }
        
        public void AcquireCard(Vector2 pos, float time)
        {
            MoveCard(pos, time);
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, time);
            RefreshInHandCard(time);
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
        
        public void UseCard()
        {
            if (!Input.GetMouseButtonUp(0))
            {
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
            ActionGO.SetActive(false);
            isInside = false;
            isHand = false;
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
            
            transform.DOLocalMove(BattleController.Instance.CenterPos.localPosition, 0.2f);
            //GetComponent<Canvas>().sortingOrder =  1000;

            GameUtility.DelayExcute(0.4f, () =>
            {
                ToPassCard(0.2f);

            });

        }

        public void RemoveCard()
        {
            isHand = false;
            transform.localScale = Vector3.one;
            transform.DOScale(BattleController.Instance.CenterPos.localPosition, 0.25f).OnComplete(() =>
            {
                GameEntry.Entity.HideEntity(this);
            });
        }

        public void ShowInPassCard()
        {
            isHand = false;
            transform.localPosition = BattleController.Instance.PassCardPos.localPosition;
            
            transform.localScale = Vector3.one / 2f;
        }
        
        public void ShowInStandByCard()
        {
            isHand = false;
            transform.localPosition = BattleController.Instance.StandByCardPos.localPosition;
            transform.localScale = Vector3.one / 2f;
        }
        
        public void ShowInConsumeCard()
        {
            isHand = false;
            transform.localPosition = BattleController.Instance.ConsumeCardPos.localPosition;
            transform.localScale = Vector3.one / 2f;
        }
        
        public void ToPassCard(float time)
        {
            isHand = false;
            transform.DOLocalMove(BattleController.Instance.PassCardPos.localPosition, time);
            
            transform.localScale = Vector3.one;
            transform.DOScale(Vector3.zero, time).OnComplete(() =>
            {
                GameEntry.Entity.HideEntity(this);
            });
        }
        
        public void ToStandByCard(float time)
        {
            isHand = false;
            transform.DOLocalMove(BattleController.Instance.StandByCardPos.localPosition, time);
            
            transform.localScale = Vector3.one;
            transform.DOScale(Vector3.zero, time).OnComplete(() =>
            {
                GameEntry.Entity.HideEntity(this);
            });
        }
        
        public void ToConsumeCard(float time)
        {
            isHand = false;
            transform.DOLocalMove(BattleController.Instance.ConsumeCardPos.localPosition, time);
            
            transform.localScale = Vector3.one;
            transform.DOScale(Vector3.zero, time).OnComplete(() =>
            {
                GameEntry.Entity.HideEntity(this);
            });
        }
        
        public void StandByCardToHand(float time)
        {
            transform.localPosition = BattleController.Instance.StandByCardPos.localPosition;
            transform.DOLocalMove(BattleController.Instance.HandCardPos.localPosition, time);
            
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, time);
            
            RefreshInHandCard(time);
        }
        
        public void PassCardToHand(float time)
        {
            transform.localPosition = BattleController.Instance.PassCardPos.localPosition;
            transform.DOLocalMove(BattleController.Instance.HandCardPos.localPosition, time);
            
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, time);
            RefreshInHandCard(time);
        }
        
        public void NewCardToHand(float time)
        {
            transform.localScale = Vector3.one;
            transform.localPosition = BattleController.Instance.CenterPos.localPosition;
            transform.DOLocalMove(BattleController.Instance.HandCardPos.localPosition, time);
            RefreshInHandCard(time);
            
        }
        
        public void NewCardToStandBy(float time)
        {
            isHand = false;
            transform.localScale = Vector3.one;
            transform.localPosition = BattleController.Instance.CenterPos.localPosition;
            transform.DOLocalMove(BattleController.Instance.StandByCardPos.localPosition, time);
            
            transform.DOScale(Vector3.zero, time).OnComplete(() =>
            {
                GameEntry.Entity.HideEntity(this);
            });
            
        }
        
        public void NewCardToPass(float time)
        {
            isHand = false;
            transform.localScale = Vector3.one;
            transform.localPosition = BattleController.Instance.CenterPos.localPosition;
            transform.DOLocalMove(BattleController.Instance.PassCardPos.localPosition, time);
            
            transform.DOScale(Vector3.zero, time).OnComplete(() =>
            {
                GameEntry.Entity.HideEntity(this);
            });
        }


        
        
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
            CardItem.SetCard(BattleCardEntityData.CardData.CardID);
        }

        public void OnRefreshInfo(object sender, GameEventArgs e)
        {
            RefreshInfo();
        }

        public void Move()
        {
            Log.Debug("Move");
            Action();
            BattleCardEntityData.CardData.CardUseType = ECardUseType.Move;
            BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr =
                EBuffID.Spec_MoveUs.ToString();
        }
        
        public void Attack()
        {
            Log.Debug("Attack");
            Action();
            BattleCardEntityData.CardData.CardUseType = ECardUseType.Attack;
            BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr =
                EBuffID.Spec_AttackUs.ToString();
        }

        private void Action()
        {
            
            BattleManager.Instance.BattleState = EBattleState.TacticSelectUnit;
            BattleManager.Instance.TempTriggerData.TriggerBuffData.TriggerBuffType = TriggerBuffType.Card;
            BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx = BattleCardEntityData.CardIdx;
            
        }
    }
}