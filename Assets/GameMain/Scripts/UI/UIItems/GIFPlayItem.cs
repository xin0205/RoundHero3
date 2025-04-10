﻿using System;
using GifImporter;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoundHero
{
    [Serializable]
    public class GIFPlayData
    {
        public EGIFType ItemType;
        public int ID;
        public EShowPosition ShowPosition = EShowPosition.MousePosition;
    }
    
    public class GIFPlayItem : MonoBehaviour
    {
        [SerializeField] private GifPlayer gifPlayer;
        [SerializeField] private GIFAssets gifAssets;

        private GIFPlayData gifPlayData;

        public void SetGIF(GIFPlayData gifPlayData)
        {
            this.gifPlayData = gifPlayData;
            var gifStr = gifPlayData.ItemType.ToString() + "_" + gifPlayData.ID.ToString();
            if (gifAssets.GifAssetDict.ContainsKey(gifStr))
            {
                gifPlayer.Gif = gifAssets.GifAssetDict[gifStr];
            }
            
        }
    }
}