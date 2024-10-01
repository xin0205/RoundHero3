using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = System.Random;

namespace RoundHero
{
    public class MapStageRoute : MonoBehaviour
    {
        [SerializeField] private List<MapStageRouteItem> mapStageRouteItems;
        
        private int randomSeed;
        private Random random;
        [SerializeField] public int SelectRouteIdx;

        public void Init(int randomSeed, int selectRouteIdx)
        {
            this.randomSeed = randomSeed;
            this.random = new Random(randomSeed);
            this.SelectRouteIdx = selectRouteIdx;
         
            
            randomSeed = random.Next(0, Constant.Game.RandomRange);

            var stage = BattleMapManager.Instance.GenerateStage(randomSeed);

            for (int i = 0; i < mapStageRouteItems.Count; i++)
            {
                mapStageRouteItems[i].Init(new Data_MapRoute()
                {
                    MapIdx = BattleMapManager.Instance.MapData.CurMapStageIdx.MapIdx,
                    StageIdx = BattleMapManager.Instance.MapData.CurMapStageIdx.StageIdx,
                    RouteIdx = i,

                }, stage[i],  i, RefreshSelectRouteIdx);
            }
            
        }
        
        public void Refresh()
        {
            // randomSeed = random.Next(0, Constant.Game.RandomRange);
            //
            // var stage = BattleMapManager.Instance.GenerateStage(randomSeed);
            //
            // for (int i = 0; i < mapStageRouteItems.Count; i++)
            // {
            //     mapStageRouteItems[i].Init(new Data_MapRoute()
            //     {
            //         MapIdx = BattleMapManager.Instance.MapData.CurMapStageIdx.MapIdx,
            //         StageIdx = BattleMapManager.Instance.MapData.CurMapStageIdx.StageIdx,
            //         RouteIdx = i,
            //
            //     }, stage[i],  i, RefreshSelectRouteIdx);
            // }
        }

        public void RefreshSelectRouteIdx(int selectRouteIdx)
        {
            this.SelectRouteIdx = selectRouteIdx;
        }

        

    }
}