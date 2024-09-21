using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace RoundHero
{

    // public class MapStageRouteSelectFormData
    // {
    //     public int RandomSeed;
    // }
    
    public class MapStageRouteSelectForm : UGuiForm
    {
        //[SerializeField] private MapStageRouteSelectFormData mapStageRouteSelectFormData;

        [SerializeField] private List<MapStageRouteItem> mapStageRouteItems;
        
        private int randomSeed;
        private int selectRouteIdx;

        private ToggleGroup toggleGroup;
        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            //mapStageRouteSelectFormData = (MapStageRouteSelectFormData)userData;
            // if (mapStageRouteSelectFormData == null)
            // {
            //     Log.Warning("mapStageRouteSelectFormData is null.");
            //     return;
            // }

            Refresh();

        }

        public void Refresh()
        {
            randomSeed = BattleMapManager.Instance.Random.Next(0, Constant.Game.RandomRange);
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

        public void RefreshSelectRouteIdx(int selectRouteIdx)
        {
            this.selectRouteIdx = selectRouteIdx;
        }

        public void Confirm()
        {
            BattleMapManager.Instance.MapData.MapStageDataDict.Add(BattleMapManager.Instance.MapData.CurMapStageIdx.StageIdx, new Data_MapStage()
            {
                StageRandomSeed = randomSeed,
                SelectRouteIdx = selectRouteIdx,
                StageIdx = BattleMapManager.Instance.MapData.CurMapStageIdx.StageIdx,
            });
            BattleMapManager.Instance.MapData.CurMapStageIdx.RouteIdx = selectRouteIdx;
            BattleMapManager.Instance.MapData.CurMapStageIdx.IsSelectRoute = true;
            
            GameEntry.Event.Fire(null, RefreshMapStageEventArgs.Create());


        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            
        }
    }
}