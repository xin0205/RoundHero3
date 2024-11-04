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
        //[SerializeField] private List<Toggle> toggles;
        
        private int randomSeed;
        private int stageIdx = -1;
        private Random random;
        [SerializeField] public int SelectRouteIdx = -1;
        

        public void Init(int stageIdx, int randomSeed)
        {
            this.stageIdx = stageIdx;
            this.randomSeed = randomSeed;
            this.random = new Random(randomSeed);
            
            randomSeed = random.Next(0, Constant.Game.RandomRange);

            var stage = BattleMapManager.Instance.GenerateStage(randomSeed);

            for (int i = 0; i < mapStageRouteItems.Count; i++)
            {
                mapStageRouteItems[i].Init(new Data_MapRoute()
                {
                    MapIdx = BattleMapManager.Instance.MapData.CurMapStageIdx.MapIdx,
                    StageIdx = stageIdx,
                    RouteIdx = i,

                }, stage[i], RefreshSelectRouteIdx);
            }

            //ToggleEnable(true);

        }

        // private void ToggleEnable(bool isEnable)
        // {
        //     foreach (var toggle in toggles)
        //     {
        //         
        //         if (!isEnable)
        //         {
        //             toggle.isOn = false;
        //         }
        //         toggle.graphic.gameObject.SetActive(isEnable);
        //         toggle.enabled = isEnable;
        //     }
        // }
        
        public void Refresh()
        {
            if (stageIdx == BattleMapManager.Instance.MapData.CurMapStageIdx.StageIdx)
            {
                if (BattleMapManager.Instance.MapData.CurMapStageIdx.IsSelectRoute)
                {
                    // GameUtility.DelayExcute(0.1f, ()=>
                    // {
                    //     
                    // });
                    //ToggleEnable(false);
                    for (int i = 0; i < mapStageRouteItems.Count; i++)
                    {
                        mapStageRouteItems[i].gameObject
                            .SetActive(i == SelectRouteIdx);
                        mapStageRouteItems[i].ShowSelect(false);
                    }
                }
                else
                {
                    
                }
            }
            
            
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