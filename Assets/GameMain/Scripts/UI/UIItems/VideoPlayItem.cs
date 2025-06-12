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

        private AnimationPlayData _animationPlayData;

        public void SetVideo(AnimationPlayData animationPlayData)
        {
            this._animationPlayData = animationPlayData;
            var gifStr = animationPlayData.GifType.ToString() + "_" + animationPlayData.ID.ToString();
            if (videoAssets.VideoAssetDict.ContainsKey(gifStr))
            {
                videoPlayer.clip = videoAssets.VideoAssetDict[gifStr];
            }
            
        }
    }
}