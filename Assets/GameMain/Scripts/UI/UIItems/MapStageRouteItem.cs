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

        public void Init(Data_MapRoute mapRoute, List<EMapSite> mapSites, int tag, Action<int> tagCallBack)
        {
            MapSites = mapSites;
            this.tag = tag;
            TagCallBack = tagCallBack;

            for (int i = 0; i < mapStageStepItems.Count; i++)
            {
                if (i >= MapSites.Count)
                {
                    mapStageStepItems[i].Init(new Data_MapStep()
                    {
                        MapRoute = mapRoute,
                        StepIdx = i,
                    }, EMapSite.Empty);
                }
                else
                {
                    mapStageStepItems[i].Init(new Data_MapStep()
                    {
                        MapRoute = mapRoute,
                        StepIdx = i,
                    }, MapSites[i]);
                }
                
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