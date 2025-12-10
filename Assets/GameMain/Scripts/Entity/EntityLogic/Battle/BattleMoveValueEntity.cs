using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleMoveValueEntity : Entity
    {
        public BattleMoveValueEntityData BattleMoveValueEntityData { get; protected set; }

        
        [SerializeField] private Text text;
        [SerializeField] private Color hurtColor;
        [SerializeField] private Color recoverColor;

        protected Quaternion cameraQuaternion = Quaternion.identity;
        private Tween textColTween;
        //private Tween moveTween;
        private Tween textStrTween;
        
        private string positiveStartValue;
        private string positiveEndValue;

        private string negativeStartValue;
        private string negativeEndValue;
        
        [SerializeField] private Image Icon;
        private Vector3 startPos = Vector2.zero;
        private Vector3 endPos = Vector2.zero;
        private Vector3 oriStartPos = Vector2.zero;
        private Vector3 oriEndPos = Vector2.zero;


        
        protected override async void OnShow(object userData)
        {
            transform.SetParent(AreaController.Instance.BattleFormRoot.transform);
            
            base.OnShow(userData);

            BattleMoveValueEntityData = userData as BattleMoveValueEntityData;
            if (BattleMoveValueEntityData == null)
            {
                Log.Error("Error BattleMoveValueEntity");
                return;
            }

            
            this.time = 0;
            this.timeEnd = 0;

            var absStartValue = Mathf.Abs(BattleMoveValueEntityData.StartValue);
            var absEndValue = Mathf.Abs(BattleMoveValueEntityData.EndValue);

            if (absStartValue == 0)
            {
                positiveStartValue = "0";
                negativeStartValue = "0";
            }
            else
            {
                positiveStartValue = "+" + absStartValue;
                negativeStartValue = "-" + Mathf.Abs(absStartValue);
            }
            
            if (absEndValue == 0)
            {
                positiveEndValue = "0";
                negativeEndValue = "0";
            }
            else
            {
                positiveEndValue = "+" + absEndValue;
                negativeEndValue = "-" + Mathf.Abs(absEndValue);
            }
           
            
            
            text.text = BattleMoveValueEntityData.StartValue < 0
                ? negativeStartValue
                : BattleMoveValueEntityData.StartValue > 0 ? positiveStartValue: negativeStartValue;

            text.color = BattleMoveValueEntityData.StartValue < 0 ? hurtColor : recoverColor;

            
            if (BattleMoveValueEntityData.FollowParams.IsUIGO)
            {
                startPos = BattleMoveValueEntityData.FollowParams.FollowGO.transform.localPosition;
                startPos += (Vector3)BattleMoveValueEntityData.FollowParams.DeltaPos;
                
            }
            else
            {
                startPos = PositionConvert.WorldPointToUILocalPoint(
                    AreaController.Instance.BattleFormRoot.GetComponent<RectTransform>(), BattleMoveValueEntityData.FollowParams.FollowGO.transform.localPosition);
                startPos += (Vector3)BattleMoveValueEntityData.FollowParams.DeltaPos;
            }

            oriStartPos = startPos;

            
            if (BattleMoveValueEntityData.TargetFollowParams.IsUIGO)
            {
                endPos = BattleMoveValueEntityData.TargetFollowParams.FollowGO.transform.localPosition;
                endPos += (Vector3)BattleMoveValueEntityData.TargetFollowParams.DeltaPos;
            }
            else
            {
                endPos = PositionConvert.WorldPointToUILocalPoint(
                    AreaController.Instance.BattleFormRoot.GetComponent<RectTransform>(), BattleMoveValueEntityData.TargetFollowParams.FollowGO.transform.localPosition);
                endPos += (Vector3)BattleMoveValueEntityData.TargetFollowParams.DeltaPos;
            }

            oriEndPos = endPos;
            
            this.transform.localPosition = startPos;
            
            // this.gameObject.SetActive(false);
            // GameUtility.DelayExcute(0.25f * BattleMoveValueEntityData.ShowValueIdx, () =>
            // {
            //     if (GameEntry.Entity.HasEntity(this.Id))
            //     {
            //         this.gameObject.SetActive(true);
            //     }
            //     
            // });

            Icon.gameObject.SetActive(true);
            if (BattleMoveValueEntityData is BlessIconValueEntityData blessIconValueEntityData)
            {
                Icon.sprite = await AssetUtility.GetBlessIcon(blessIconValueEntityData.BlessID);
            }
            else if (BattleMoveValueEntityData is BattleUnitStateValueEntityData unitStateIconValueEntityData)
            {
                Icon.sprite = await AssetUtility.GetUnitStateIcon(unitStateIconValueEntityData.UnitState);
            }
            else
            {
                Icon.gameObject.SetActive(false);
            }

            

        }

        private float time = 0f;
        private float timeEnd = 0f;
        
        private void Update()
        {
            if(!this.gameObject.activeSelf)
                return;
            
            time += Time.deltaTime;
            if(time < 0)
                return;


            if (BattleMoveValueEntityData.FollowParams.FollowGO.IsDestroyed())
            {
                GameEntry.Entity.HideEntity(this);
                return;
            }
               
            
            if(BattleMoveValueEntityData.TargetFollowParams.FollowGO.IsDestroyed())
            {
                GameEntry.Entity.HideEntity(this);
                return;
            }
            
            if (!BattleMoveValueEntityData.FollowParams.IsUIGO)
            {
                var _startPos = BattleMoveValueEntityData.FollowParams.FollowGO == null
                    ? oriStartPos
                    : BattleMoveValueEntityData.FollowParams.FollowGO.transform.localPosition;
                startPos = PositionConvert.WorldPointToUILocalPoint(
                    AreaController.Instance.BattleFormRoot.GetComponent<RectTransform>(), _startPos);
                startPos += (Vector3)BattleMoveValueEntityData.FollowParams.DeltaPos;
            }
            
            if (!BattleMoveValueEntityData.TargetFollowParams.IsUIGO)
            {
                var _endPos = BattleMoveValueEntityData.TargetFollowParams.FollowGO == null
                    ? oriEndPos
                    : BattleMoveValueEntityData.TargetFollowParams.FollowGO.transform.localPosition;
                
                endPos = PositionConvert.WorldPointToUILocalPoint(
                    AreaController.Instance.BattleFormRoot.GetComponent<RectTransform>(), _endPos);
                endPos += (Vector3)BattleMoveValueEntityData.TargetFollowParams.DeltaPos;
            }

            
            
            this.transform.localPosition = Vector2.Lerp(startPos, endPos, time);
            
            if (BattleMoveValueEntityData.IsAdd)
            {
                if (time >= 0.5f)
                {
                    text.text = positiveEndValue;
                    text.color = recoverColor;
                }
                else
                {
                    // text.text = negativeEndValue;
                    // text.color = hurtColor;
                    text.text = BattleMoveValueEntityData.StartValue < 0
                        ? negativeStartValue
                        : BattleMoveValueEntityData.StartValue > 0 ? positiveStartValue: negativeStartValue;

                    text.color = BattleMoveValueEntityData.StartValue < 0 ? hurtColor : recoverColor;
                }
  
            }
            

            if(this.transform.localPosition == endPos)
            {
                timeEnd += Time.deltaTime;
                if (timeEnd > 0.1f)
                {
                    timeEnd = 0f;
                    if (BattleMoveValueEntityData.IsLoop )
                    {
                        time = -1.5f;
                        this.transform.localPosition = new Vector3(9999, 9999, 9999);
                    }
                    else
                    {
                        if (GameEntry.Entity.HasEntity(this.Id))
                        {
                            GameEntry.Entity.HideEntity(this);
                        }
                    }
                }
                
            }
            
        }

        

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            //Log.Debug("OnHide" + BattleMoveValueEntityData.Id);

            //KillTween();

        }

        public void HideEntity()
        {
            if (BattleMoveValueEntityData.IsLoop)
            {
                GameEntry.Entity.HideEntity(this);
                //BattleMoveValueEntityData.IsLoop = false;
            }
            else
            {
                //GameEntry.Entity.HideEntity(this);
            }
        }

    }
}