using System;
using GifImporter;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Video;

namespace RoundHero
{
    [Serializable]
    public class AnimationPlayData
    {
        public EGIFType GifType;
        public int ID;
        public EShowPosition ShowPosition = EShowPosition.MousePosition;
    }
    
    public class GIFPlayItem : MonoBehaviour
    {
        [SerializeField] private GifPlayer gifPlayer;
        [SerializeField] private GIFAssets gifAssets;


        private AnimationPlayData _animationPlayData;

        public void SetGIF(AnimationPlayData animationPlayData)
        {
            this._animationPlayData = animationPlayData;
            var gifStr = animationPlayData.GifType.ToString() + "_" + animationPlayData.ID.ToString();
            if (gifAssets.GifAssetDict.ContainsKey(gifStr))
            {
                gifPlayer.Gif = gifAssets.GifAssetDict[gifStr];
            }
            
        }
    }
}