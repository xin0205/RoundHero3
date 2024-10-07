using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace RoundHero
{
    public class MapStageRouteItem : MonoBehaviour
    {
        [SerializeField] private List<MapStageStepItem> mapStageStepItems = new ();

        private List<EMapSite> MapSites;
        private Data_MapRoute mapRoute;

        private Action<int> TagCallBack;

        // private int randomSeed;
        // private System.Random random;

        public void Init(Data_MapRoute mapRoute, List<EMapSite> mapSites, Action<int> tagCallBack)
        {
            gameObject.SetActive(true);
            this.mapRoute = mapRoute;
            MapSites = mapSites;
            
            TagCallBack = tagCallBack;

            // randomSeed = BattleMapManager.Instance.MapData
            //     .MapStageDataDict[mapRoute.StageIdx].StageRandomSeed;
            // this.random = new Random(randomSeed);
            var stageIdx = BattleMapManager.Instance.MapData.CurMapStageIdx.StageIdx;
            
            
            for (int i = 0; i < mapStageStepItems.Count; i++)
            {
                var randomSeed = BattleMapManager.Instance.MapData.MapStageDataDict[stageIdx]
                    .MapSteps[mapRoute.RouteIdx][i].RandomSeed;
                if (i >= MapSites.Count)
                {
                    mapStageStepItems[i].Init(new Data_MapStep()
                    {
                        MapRoute = mapRoute,
                        StepIdx = i,
                    }, EMapSite.Empty, randomSeed);
                    
                }
                else
                {
                    mapStageStepItems[i].Init(new Data_MapStep()
                    {
                        MapRoute = mapRoute,
                        StepIdx = i,
                    }, MapSites[i], randomSeed);
                }
                
                //mapStageStepItems[i].RandomPosition();
            }

        }

        public void Select(bool valueChanged)
        {
            if(!valueChanged)
                return;
            
            TagCallBack?.Invoke(mapRoute.RouteIdx);
        }

    }
}