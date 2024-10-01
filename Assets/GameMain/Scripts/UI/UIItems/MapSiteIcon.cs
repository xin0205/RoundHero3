using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public class MapSiteIcon : MonoBehaviour
    {
        [SerializeField]
        public MapSiteSprite MapSiteSprite;
        
        [SerializeField]
        public Image MapSiteImage;

        public EMapSite MapSite;

        [SerializeField] public bool ShowTip;
        
        [SerializeField] public bool TriggerEnter;


        public async void Init(EMapSite mapSite)
        {
            MapSite = mapSite;
            
            MapSiteImage.overrideSprite = await AssetUtility.GetMapSiteIcon(mapSite);
        }

        public void Enter()
        {
            switch (MapSite)
            {
                case EMapSite.NormalBattle:
                    break;
                case EMapSite.EliteBattle:
                    break;
                case EMapSite.BossBattle:
                    break;
                case EMapSite.Store:
                    break;
                case EMapSite.Rest:
                    break;
                case EMapSite.Treasure:
                    break;
                case EMapSite.Random:
                    break;
                case EMapSite.Empty:
                    break;
                default:
                    break;
            }
        }
        

    }
}