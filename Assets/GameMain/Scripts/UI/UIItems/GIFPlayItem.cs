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
        public int ID = -1;
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
            //animationPlayData.GifType.ToString() + 
            var gifStr = "GIF_" + animationPlayData.ID;
            if (gifAssets.GifAssetDict.ContainsKey(gifStr))
            {
                gifPlayer.Gif = gifAssets.GifAssetDict[gifStr];
            }
            
        }
    }
}