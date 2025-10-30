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
            
            var gifStr = "GIF_" + animationPlayData.ID.ToString();
            var showVideo =
                videoAssets.VideoAssetDict.ContainsKey(gifStr);
                
            this.gameObject.SetActive(showVideo);
            //this.videoPlayer.gameObject.SetActive(showVideo);

            if (showVideo)
            {
                videoPlayer.clip = videoAssets.VideoAssetDict[gifStr];
            }
            
            
            
        }
    }
}