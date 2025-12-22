using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class ExplainData
    {
        public EItemType ItemType;
        public int ItemID = -1;
        public EShowPosition ShowPosition = EShowPosition.MousePosition;
        public int VideoID = -1;
        //public AnimationPlayData AnimationPlayData;
    }
    
    public class ExplainForm : UGuiForm
    {
        private ExplainData explainData;

        [SerializeField] private VideoPlayItem videoPlayItem;
        
        // [SerializeField] private VideoPlayer videoPlayer;
        // [SerializeField] private VideoAssets videoAssets;
        [SerializeField] private ExplainList explainList;
        [SerializeField] private GridLayoutGroup explainListGridLayoutGroup;
        
        [SerializeField] private Transform mouseLeftUpPos;
        [SerializeField] private Transform mouseRightUpPos;
        [SerializeField] private Transform mouseLeftUpPosNoVideo;
        [SerializeField] private Transform mouseRightUpPosNoVideo;
        
        [SerializeField] private Transform mouseLeftDownPos;
        [SerializeField] private Transform mouseRightDownPos;
        [SerializeField] private Transform mouseLeftDownPosNoVideo;
        [SerializeField] private Transform mouseRightDownPosNoVideo;
        
        [SerializeField] private Transform downPos;
        
        [SerializeField] private Transform rightTransform;
        [SerializeField] private Transform leftTransform;
        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            explainData = (ExplainData)userData;

            SetData();

        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            UpdatePos();
        }


        public void UpdatePos()
        {
            
            
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 0;
            
            Vector2 uiLocalPos = Vector2.zero;

            if (explainData.ShowPosition == EShowPosition.MousePosition)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    AreaController.Instance.Canvas.GetComponent<RectTransform>(), // UI根节点RectTransform
                    mousePosition,                             // 鼠标屏幕坐标
                    AreaController.Instance.Canvas.worldCamera,                    // Canvas对应的相机（Overlay模式传null）
                    out uiLocalPos                               // 输出的UI本地坐标
                );
                
                //var gifPos = AreaController.Instance.UICamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, mousePosition.z));
                var delta = showVideo ? 250f : 50f;
                if (mousePosition.x < Screen.width / 2)
                {
                    uiLocalPos.x += delta;
                    if (mousePosition.y < Screen.height / 2)
                    {
                        uiLocalPos.y += delta;
                        
                        explainListGridLayoutGroup.startCorner = GridLayoutGroup.Corner.LowerLeft;
                        explainListGridLayoutGroup.childAlignment = TextAnchor.LowerLeft;
                        explainList.transform.localPosition = showVideo ? mouseLeftDownPos.localPosition : mouseLeftDownPosNoVideo.localPosition;

                    }
                    else
                    {
                        uiLocalPos.y -= delta;
                        
                        explainListGridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
                        explainListGridLayoutGroup.childAlignment = TextAnchor.UpperLeft;
                        explainList.transform.localPosition = showVideo ? mouseLeftUpPos.localPosition : mouseLeftUpPosNoVideo.localPosition;
                        
                    }
                    
                }
                else
                {
                    uiLocalPos.x -= delta;
                    if (mousePosition.y < Screen.height / 2)
                    {
                        uiLocalPos.y += delta;

                        explainListGridLayoutGroup.startCorner = GridLayoutGroup.Corner.LowerRight;
                        explainListGridLayoutGroup.childAlignment = TextAnchor.LowerRight;
                        explainList.transform.localPosition = showVideo ? mouseRightDownPos.localPosition : mouseRightDownPosNoVideo.localPosition;
                    }
                    else
                    {
                        uiLocalPos.y -= delta;
                        
                        explainListGridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperRight;
                        explainListGridLayoutGroup.childAlignment = TextAnchor.UpperRight;
                        explainList.transform.localPosition = showVideo ? mouseRightUpPos.localPosition : mouseRightUpPosNoVideo.localPosition;
                    }
                }
            
                
                
            }
            else if (explainData.ShowPosition == EShowPosition.Right)
            {
                uiLocalPos = rightTransform.localPosition;
                
                explainListGridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperRight;
                explainListGridLayoutGroup.childAlignment = TextAnchor.UpperCenter;
                explainList.transform.localPosition = showVideo ? downPos.localPosition : Vector3.zero;
            }
            else if (explainData.ShowPosition == EShowPosition.Left)
            {
                uiLocalPos = leftTransform.localPosition;
                
                explainListGridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
                explainListGridLayoutGroup.childAlignment = TextAnchor.UpperCenter;
                explainList.transform.localPosition = showVideo ? downPos.localPosition : Vector3.zero;
            }
            
            transform.localPosition = uiLocalPos;
        }

        private bool showVideo;
        private void SetData()
        {
            videoPlayItem.SetVideo(new AnimationPlayData()
            {
                ID = explainData.VideoID,
            });

            showVideo = explainData.VideoID != -1;
            
            // showVideo =
            //     (explainData.ItemType == EItemType.UnitCard || explainData.ItemType == EItemType.TacticCard) &&
            //     explainData.VideoID != -1 &&
            //     videoAssets.VideoAssetDict.ContainsKey(gifStr);
            
            // var gifStr = "GIF_" + explainData.VideoID.ToString();
            // showVideo =
            //     (explainData.ItemType == EItemType.UnitCard || explainData.ItemType == EItemType.TacticCard) &&
            //     explainData.VideoID != -1 &&
            //     videoAssets.VideoAssetDict.ContainsKey(gifStr);
            //
            // this.videoPlayer.gameObject.SetActive(showVideo);
            // //animationPlayData.GifType.ToString() + 
            //
            // if (showVideo)
            // {
            //     videoPlayer.clip = videoAssets.VideoAssetDict[gifStr];
            // }
            
            explainList.gameObject.SetActive(false);
            switch (explainData.ItemType)
            {
                case EItemType.UnitCard:
                case EItemType.TacticCard:
                    var cardExplainList = BattleCardManager.Instance.GetCardExplainList(explainData.ItemID);
                    explainList.SetData(cardExplainList);
                    explainList.gameObject.SetActive(true);
                    break;
                case EItemType.Bless:
                    var blessExplainList = BlessManager.Instance.GetBlessExplainList(explainData.ItemID);
                    explainList.SetData(blessExplainList);
                    explainList.gameObject.SetActive(true);
                    break;
                case EItemType.Fune:
                    var buffExplainList = BattleBuffManager.Instance.GetBuffExplainList(explainData.ItemID);
                    explainList.SetData(buffExplainList);
                    explainList.gameObject.SetActive(true);
                    break;
                case EItemType.UnitState:
                    break;
                case EItemType.Enemy:
                    var enemyExplainList = BattleEnemyManager.Instance.GetEnemyExplainList(explainData.ItemID);
                    explainList.SetData(enemyExplainList);
                    explainList.gameObject.SetActive(true);
                    break;
                case EItemType.Coin:
                    break;
                case EItemType.AddMaxHP:
                    break;
                case EItemType.RemoveCard:
                    break;
                case EItemType.AddCardFuneSlot:
                    break;
                case EItemType.ActionTimes:
                case EItemType.PassBePass:
                case EItemType.Pass:
                case EItemType.BePass:
                case EItemType.Buff:
                case EItemType.DeBuff:
                    var newExplainData = GameUtility.GetCommonExplainData(explainData.ItemType.ToString());
                    explainList.SetData(new List<CommonItemData>()
                    {
                        newExplainData,
                    });
                    explainList.gameObject.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

    }
}