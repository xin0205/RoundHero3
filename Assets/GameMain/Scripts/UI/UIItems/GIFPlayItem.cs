using System;
using GifImporter;
using UnityEngine;

namespace RoundHero
{
    [Serializable]
    public class GIFPlayData
    {
        public EGIFType ItemType;
        public int Idx;

    }
    
    public class GIFPlayItem : MonoBehaviour
    {
        [SerializeField] private GifPlayer gifPlayer;
        [SerializeField] private GIFAssets gifAssets;

        private GIFPlayData gifPlayData;

        public void SetGIF(GIFPlayData gifPlayData)
        {
            this.gifPlayData = gifPlayData;
            var gifStr = gifPlayData.ItemType.ToString() + "_" + gifPlayData.Idx.ToString();
            gifPlayer.Gif = gifAssets.GifAssetDict[gifStr];
        }
    }
}