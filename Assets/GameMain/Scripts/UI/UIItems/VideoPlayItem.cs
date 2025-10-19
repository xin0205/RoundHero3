using System;
using GifImporter;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Video;

namespace RoundHero
{

    
    public class VideoPlayItem : MonoBehaviour
    {

        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private VideoAssets videoAssets;
        [SerializeField] private Transform battleRightTransform;
        
        private AnimationPlayData animationPlayData;

        public void SetVideo(AnimationPlayData animationPlayData)
        {
            // this.animationPlayData = animationPlayData;
            // if (this.animationPlayData.ID == -1)
            // {
            //     this.gameObject.SetActive(false);
            //     return;
            // }
            //
            // this.gameObject.SetActive(true);
            // //animationPlayData.GifType.ToString() + 
            // var gifStr = "GIF_" + animationPlayData.ID.ToString();
            // if (videoAssets.VideoAssetDict.ContainsKey(gifStr))
            // {
            //     videoPlayer.clip = videoAssets.VideoAssetDict[gifStr];
            // }
            //
            // Vector3 mousePosition = Input.mousePosition;
            //
            // if (animationPlayData.ShowPosition == EShowPosition.MousePosition)
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
            //     transform.position = gifPos;
            // }
            // else if (animationPlayData.ShowPosition == EShowPosition.BattleRight)
            // {
            //     transform.position = battleRightTransform.position;
            // }
            
        }
    }
}