using System;
using UnityEngine;


namespace RoundHero
{

    [Serializable]
    public class VideoFormData
    {
        public AnimationPlayData AnimationPlayData;
        
    }
    
    public class VideoForm : UGuiForm
    {
        private VideoFormData videoFormData;

        [SerializeField] private VideoPlayItem videoPlayItem;
        
        [SerializeField] private Transform battleLeftTransform;
        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            videoFormData = (VideoFormData)userData;

            videoPlayItem.SetVideo(videoFormData.AnimationPlayData);

            Vector3 mousePosition = Input.mousePosition;
            
            // if (videoFormData.AnimationPlayData.ShowPosition == EShowPosition.MousePosition)
            // {
            //     var gifPos = AreaController.Instance.UICamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, mousePosition.z));
            //     var delta = 2f;
            //     if (mousePosition.x < Screen.width / 2)
            //     {
            //         gifPos.x += delta;
            //         if (mousePosition.y < Screen.height / 2)
            //         {
            //             gifPos.y += delta;
            //         }
            //         else
            //         {
            //             gifPos.y -= delta;
            //         }
            //     }
            //     else
            //     {
            //         gifPos.x -= delta;
            //         if (mousePosition.y < Screen.height / 2)
            //         {
            //             gifPos.y += delta;
            //         }
            //         else
            //         {
            //             gifPos.y -= delta;
            //         }
            //     }
            //
            //
            //     videoPlayItem.transform.position = gifPos;
            // }
            // else if (videoFormData.AnimationPlayData.ShowPosition == EShowPosition.Right)
            // {
            //     videoPlayItem.transform.position = battleLeftTransform.position;
            // }
            
        }
    }
}