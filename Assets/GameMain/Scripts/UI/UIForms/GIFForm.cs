using System;
using GifImporter;
using UnityEngine;
using UnityEngine.Serialization;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public enum EGIFType
    {
        Hero,
        Solider,
        Enemy,
        Tactic,
        
    }
    
    [Serializable]
    public class GIFFormData
    {
        public AnimationPlayData animationPlayData;
        
    }
    
    public class GIFForm : UGuiForm
    {
        private GIFFormData gifFormData;

        [SerializeField] private GIFPlayItem gifPlayItem;
        
        [SerializeField] private Transform battleLeftTransform;
        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            gifFormData = (GIFFormData)userData;

            gifPlayItem.SetGIF(gifFormData.animationPlayData);

            Vector3 mousePosition = Input.mousePosition;
            
            // if (gifFormData.animationPlayData.ShowPosition == EShowPosition.MousePosition)
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
            //     gifPlayItem.transform.position = gifPos;
            // }
            // else if (gifFormData.animationPlayData.ShowPosition == EShowPosition.Right)
            // {
            //     gifPlayItem.transform.position = battleLeftTransform.position;
            // }
            
        }
    }
}