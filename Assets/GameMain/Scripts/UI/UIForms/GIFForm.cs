using System;
using GifImporter;
using UnityEngine;

namespace RoundHero
{
    public enum EGIFType
    {
        Hero,
        Solider,
        Enemy,
        
    }
    
    [Serializable]
    public class GIFFormData
    {
        public EGIFType ItemType;
        public int Idx;

    }
    
    public class GIFForm : UGuiForm
    {
        private GIFFormData gifFormData;

        [SerializeField] private GifPlayer gifPlayer;
        [SerializeField] private GIFAssets gifAssets;
        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            gifFormData = (GIFFormData)userData;

            var gifStr = gifFormData.ItemType.ToString() + "_" + gifFormData.Idx.ToString();
            gifPlayer.Gif = gifAssets.GifAssetDict[gifStr];
            
        }
    }
}