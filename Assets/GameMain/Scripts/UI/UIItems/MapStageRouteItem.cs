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

        private int randomSeed;
        private System.Random random;

        public void Init(Data_MapRoute mapRoute, List<EMapSite> mapSites, Action<int> tagCallBack)
        {
            gameObject.SetActive(true);
            this.mapRoute = mapRoute;
            MapSites = mapSites;
            
            TagCallBack = tagCallBack;

            randomSeed = BattleMapManager.Instance.MapData
                .MapStageDataDict[mapRoute.StageIdx].StageRandomSeed;
            this.random = new Random(randomSeed);

            for (int i = 0; i < mapStageStepItems.Count; i++)
            {
                if (i >= MapSites.Count)
                {
                    mapStageStepItems[i].Init(new Data_MapStep()
                    {
                        MapRoute = mapRoute,
                        StepIdx = i,
                    }, EMapSite.Empty, this.random.Next(0, Constant.Game.RandomRange));
                    
                }
                else
                {
                    mapStageStepItems[i].Init(new Data_MapStep()
                    {
                        MapRoute = mapRoute,
                        StepIdx = i,
                    }, MapSites[i], this.random.Next(0, Constant.Game.RandomRange));
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