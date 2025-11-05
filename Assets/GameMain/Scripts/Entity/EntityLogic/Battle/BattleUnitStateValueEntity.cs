using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleUnitStateValueEntity : Entity
    {
        public BattleUnitStateValueEntityData BattleUnitStateValueEntityData { get; protected set; }


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

            BattleUnitStateValueEntityData = userData as BattleUnitStateValueEntityData;
            if (BattleUnitStateValueEntityData == null)
            {
                Log.Error("Error UnitStateIconValueEntityData");
                return;
            }


            this.time = 0;
            this.timeEnd = 0;

            var absStartValue = Mathf.Abs(BattleUnitStateValueEntityData.StartValue);
            var absEndValue = Mathf.Abs(BattleUnitStateValueEntityData.EndValue);

            positiveStartValue = "+" + absStartValue;
            positiveEndValue = "+" + absEndValue;

            negativeStartValue = BattleUnitStateValueEntityData.StartValue < 0
                ? "-" + absStartValue.ToString()
                : absStartValue.ToString();
            negativeEndValue = BattleUnitStateValueEntityData.EndValue < 0
                ? "-" + absEndValue.ToString()
                : absEndValue.ToString();

            text.text = BattleUnitStateValueEntityData.StartValue < 0
                ? negativeStartValue
                : BattleUnitStateValueEntityData.StartValue > 0
                    ? positiveStartValue
                    : negativeStartValue;

            text.color = BattleUnitStateValueEntityData.StartValue < 0 ? hurtColor : recoverColor;


            if (BattleUnitStateValueEntityData.FollowParams.IsUIGO)
            {
                startPos = BattleUnitStateValueEntityData.FollowParams.FollowGO.transform.localPosition;
                startPos += (Vector3)BattleUnitStateValueEntityData.FollowParams.DeltaPos;

            }
            else
            {
                startPos = PositionConvert.WorldPointToUILocalPoint(
                    AreaController.Instance.BattleFormRoot.GetComponent<RectTransform>(),
                    BattleUnitStateValueEntityData.FollowParams.FollowGO.transform.localPosition);
                startPos += (Vector3)BattleUnitStateValueEntityData.FollowParams.DeltaPos;
            }

            oriStartPos = startPos;


            if (BattleUnitStateValueEntityData.TargetFollowParams.IsUIGO)
            {
                endPos = BattleUnitStateValueEntityData.TargetFollowParams.FollowGO.transform.localPosition;
                endPos += (Vector3)BattleUnitStateValueEntityData.TargetFollowParams.DeltaPos;
            }
            else
            {
                endPos = PositionConvert.WorldPointToUILocalPoint(
                    AreaController.Instance.BattleFormRoot.GetComponent<RectTransform>(),
                    BattleUnitStateValueEntityData.TargetFollowParams.FollowGO.transform.localPosition);
                endPos += (Vector3)BattleUnitStateValueEntityData.TargetFollowParams.DeltaPos;
            }

            oriEndPos = endPos;

            this.transform.localPosition = startPos;

            Icon.gameObject.SetActive(true);
            Icon.sprite = await AssetUtility.GetUnitStateIcon(BattleUnitStateValueEntityData.UnitState);

            this.Icon.color = new Color(this.Icon.color.r, this.Icon.color.g, this.Icon.color.b, 1f);
            this.text.color = new Color(this.text.color.r, this.text.color.g, this.text.color.b, 1f);


        }

        private float time = 0f;
        private float timeEnd = 0f;
        private float timeShow = 0f;
        private void Update()
        {
            if (!this.gameObject.activeSelf)
                return;

            time += Time.deltaTime;
            if(time < 0)
                return;

            if (BattleUnitStateValueEntityData.FollowParams.FollowGO.IsDestroyed())
            {
                GameEntry.Entity.HideEntity(this);
                return;
            }


            if (BattleUnitStateValueEntityData.TargetFollowParams.FollowGO.IsDestroyed())
            {
                GameEntry.Entity.HideEntity(this);
                return;
            }

            if (!BattleUnitStateValueEntityData.FollowParams.IsUIGO)
            {
                var _startPos = BattleUnitStateValueEntityData.FollowParams.FollowGO == null
                    ? oriStartPos
                    : BattleUnitStateValueEntityData.FollowParams.FollowGO.transform.localPosition;
                startPos = PositionConvert.WorldPointToUILocalPoint(
                    AreaController.Instance.BattleFormRoot.GetComponent<RectTransform>(), _startPos);
                startPos += (Vector3)BattleUnitStateValueEntityData.FollowParams.DeltaPos;
            }

            if (!BattleUnitStateValueEntityData.TargetFollowParams.IsUIGO)
            {
                var _endPos = BattleUnitStateValueEntityData.TargetFollowParams.FollowGO == null
                    ? oriEndPos
                    : BattleUnitStateValueEntityData.TargetFollowParams.FollowGO.transform.localPosition;

                endPos = PositionConvert.WorldPointToUILocalPoint(
                    AreaController.Instance.BattleFormRoot.GetComponent<RectTransform>(), _endPos);
                endPos += (Vector3)BattleUnitStateValueEntityData.TargetFollowParams.DeltaPos;
            }
            
            // if (time <= timeShow)
            // {
            //     this.transform.localPosition = startPos;
            //     return;
            // }

            this.transform.localPosition = Vector2.Lerp(startPos, endPos, (time - timeShow) * 1f);
            
            
            // var colorAlpha = Mathf.Lerp(1f, 0f, (time - timeShow) * 1f);
            // this.Icon.color = new Color(this.Icon.color.r, this.Icon.color.g, this.Icon.color.b, colorAlpha);
            // this.text.color = new Color(this.text.color.r, this.text.color.g, this.text.color.b, colorAlpha);
            
            if (BattleUnitStateValueEntityData.IsAdd)
            {
                if ((time - timeShow) >= 0.5f)
                {
                    text.text = positiveEndValue;
                    text.color = recoverColor;
                }
                else
                {
                    text.text = negativeEndValue;
                    text.color = hurtColor;
                }
  
            }

            
            if (this.transform.localPosition == endPos)
            {
                if (BattleUnitStateValueEntityData.IsLoop)
                {
                    time = -1.5f;
                    this.transform.localPosition = new Vector3(9999, 9999, 9999);
                    // this.Icon.color = new Color(this.Icon.color.r, this.Icon.color.g, this.Icon.color.b, 1f);
                    // this.text.color = new Color(this.text.color.r, this.text.color.g, this.text.color.b, 1f);
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



        public void HideEntity()
        {
            if (BattleUnitStateValueEntityData.IsLoop)
            {
                BattleUnitStateValueEntityData.IsLoop = false;
            }

        }
    }
}