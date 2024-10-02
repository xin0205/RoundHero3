using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoundHero
{
    public class MapStageRouteItem : MonoBehaviour
    {
        [SerializeField] private List<MapStageStepItem> mapStageStepItems = new ();

        private List<EMapSite> MapSites;

        private int tag;

        private Action<int> TagCallBack;

        private int randomSeed;
        private System.Random random;

        public void Init(Data_MapRoute mapRoute, List<EMapSite> mapSites, int tag, Action<int> tagCallBack)
        {
            gameObject.SetActive(true);
            MapSites = mapSites;
            this.tag = tag;
            TagCallBack = tagCallBack;

            randomSeed = BattleMapManager.Instance.MapData
                .MapStageDataDict[tag].StageRandomSeed;

            for (int i = 0; i < mapStageStepItems.Count; i++)
            {
                if (i >= MapSites.Count)
                {
                    mapStageStepItems[i].Init(new Data_MapStep()
                    {
                        MapRoute = mapRoute,
                        StepIdx = i,
                    }, EMapSite.Empty, random);
                    
                }
                else
                {
                    mapStageStepItems[i].Init(new Data_MapStep()
                    {
                        MapRoute = mapRoute,
                        StepIdx = i,
                    }, MapSites[i], random);
                }
                
                //mapStageStepItems[i].RandomPosition();
            }

        }

        public void Select(bool valueChanged)
        {
            if(!valueChanged)
                return;
            
            TagCallBack?.Invoke(tag);
        }

    }
}