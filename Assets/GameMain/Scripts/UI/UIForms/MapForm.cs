using System;
using System.Collections.Generic;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class MapForm : UGuiForm
    {
        [SerializeField]
        private List<MapStageRoute> MapStageRoutes;
        private ProcedureGamePlay procedureGamePlay;
        
        [SerializeField]
        private ScrollView mapStageScrollView;

        private MapStageRoute MapStageRoute;


        [SerializeField] private GameObject SelectRouteGO;
        
        [SerializeField] private ToggleGroup toggleGroup;
        
        [SerializeField] private ScrollRect scrollRect;
        
        private SceneEntity restSceneEntity;
    
        protected async override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            procedureGamePlay = (ProcedureGamePlay)userData;
            GameEntry.Event.Subscribe(RefreshMapStageEventArgs.EventId, OnRefreshMapStage);
            GameEntry.Event.Subscribe(ClickMapStageStepItemEventArgs.EventId, OnClickMapStageStepItem);
            
            
            
            // if (!BattleMapManager.Instance.MapData.CurMapStageIdx.IsSelectRoute)
            // {
            //     GameEntry.UI.OpenUIForm(UIFormId.MapStageRouteSelectForm);
            //
            // }
            
            restSceneEntity = await GameEntry.Entity.ShowSceneEntityAsync("Rest");

            InitMapStageRoute();

            GameEntry.Event.Fire(null, RefreshMapStageEventArgs.Create());
        }

        // protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        // {
        //     base.OnUpdate(elapseSeconds, realElapseSeconds);
        //     Log.Debug(scrollRect.verticalNormalizedPosition);
        // }


        public void InitMapStageRoute()
        {
            for (int i = 0; i < MapStageRoutes.Count; i++)
            {
                MapStageRoutes[i].Init(i, BattleMapManager.Instance.MapData.MapStageDataDict[i].StageRandomSeed);
            }
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            GameEntry.Event.Unsubscribe(RefreshMapStageEventArgs.EventId, OnRefreshMapStage);
            GameEntry.Event.Unsubscribe(ClickMapStageStepItemEventArgs.EventId, OnClickMapStageStepItem);
            
            GameEntry.Entity.HideEntity(restSceneEntity);
        }

        private void OnRefreshMapStage(object sender, GameEventArgs e)
        {
            MapStageRoute = MapStageRoutes[BattleMapManager.Instance.MapData.CurMapStageIdx.StageIdx];
            
            var isSelectRoute = BattleMapManager.Instance.MapData.CurMapStageIdx.IsSelectRoute;
            toggleGroup.allowSwitchOff = isSelectRoute;
            toggleGroup.enabled = !isSelectRoute;
            SelectRouteGO.SetActive(!isSelectRoute);
            
            for (int i = 0; i < MapStageRoutes.Count; i++)
            {
                MapStageRoutes[i].gameObject.SetActive(i <= BattleMapManager.Instance.MapData.CurMapStageIdx.StageIdx);
                if (i == BattleMapManager.Instance.MapData.CurMapStageIdx.StageIdx)
                {
                    MapStageRoutes[i].Refresh();
                }
               
            }

            GameUtility.DelayExcute(0.1f, () => {
                scrollRect.verticalNormalizedPosition = 1;
            });
            




            // for (int i = 0; i < MapStageRouteItems.Count; i++)
            // {
            //     if (i < BattleMapManager.Instance.MapData.MapStageDataDict.Count)
            //     {
            //         var mapStageData = BattleMapManager.Instance.MapData.MapStageDataDict[i];
            //         
            //         if(mapStageData.MapIdx != BattleMapManager.Instance.MapData.CurMapStageIdx.MapIdx)
            //             continue;
            //         
            //         var stage = BattleMapManager.Instance.GenerateStage(BattleMapManager.Instance.MapData
            //             .MapStageDataDict[i].StageRandomSeed);
            //     //     MapStageRouteItems[i].Init(new Data_MapRoute()
            //     //     {
            //     //         MapIdx = mapStageData.MapIdx,
            //     //         StageIdx = mapStageData.StageIdx,
            //     //         RouteIdx = mapStageData.SelectRouteIdx,
            //     //
            //     //     }, stage[mapStageData.SelectRouteIdx], i, null);
            //     }
            // }

        }

        private void OnClickMapStageStepItem(object sender, GameEventArgs e)
        {
            var ne = e as ClickMapStageStepItemEventArgs;
            if (ne.MapStep.MapRoute.MapIdx == BattleMapManager.Instance.MapData.CurMapStageIdx.MapIdx &&
                ne.MapStep.MapRoute.StageIdx == BattleMapManager.Instance.MapData.CurMapStageIdx.StageIdx &&
                ne.MapStep.MapRoute.RouteIdx == BattleMapManager.Instance.MapData
                    .MapStageDataDict[BattleMapManager.Instance.MapData.CurMapStageIdx.StageIdx].SelectRouteIdx &&
                ne.MapStep.StepIdx == BattleMapManager.Instance.MapData.CurMapStageIdx.StepIdx + 1)
            {
                switch (ne.MapSite)
                {
                    case EMapSite.NormalBattle:
                        // Close();
                        // procedureGamePlay.StartBattle();
                        // break;
                        //PVEManager.Instance.Init(ne.MapStep.RandomSeed, EEnemyType.Normal);
                        
                        
                    case EMapSite.EliteBattle:
                        //PVEManager.Instance.Init(ne.MapStep.RandomSeed, EEnemyType.Elite);
                        // Close();
                        // procedureGamePlay.StartBattle();
                        // break;
                        GameEntry.UI.OpenUIForm(UIFormId.RestForm);
                        break;
                    
                    case EMapSite.BossBattle:
                        //PVEManager.Instance.Init(ne.MapStep.RandomSeed, EEnemyType.Boss);
                        Close();
                        procedureGamePlay.StartBattle();
                        break;
                    case EMapSite.Store:
                        GameEntry.UI.OpenUIForm(UIFormId.RestForm);
                        break;
                    case EMapSite.Rest:
                        GameEntry.UI.OpenUIForm(UIFormId.RestForm);
                        break;
                    case EMapSite.Treasure:
                        GameEntry.UI.OpenUIForm(UIFormId.RestForm);
                        break;
                    case EMapSite.Random:
                        GameEntry.UI.OpenUIForm(UIFormId.BattleEventForm);
                        break;
                    case EMapSite.Empty:
                        GameEntry.UI.OpenUIForm(UIFormId.RestForm);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                Log.Debug("Un Current StepIdx");
            }
            
        }
        
        public void Confirm()
        {
            BattleMapManager.Instance.MapData
                    .MapStageDataDict[BattleMapManager.Instance.MapData.CurMapStageIdx.StageIdx].SelectRouteIdx =
                MapStageRoute.SelectRouteIdx;
            //BattleMapManager.Instance.MapData.CurMapStageIdx.RouteIdx = MapStageRoute.SelectRouteIdx;
            BattleMapManager.Instance.MapData.CurMapStageIdx.IsSelectRoute = true;
            
            GameEntry.Event.Fire(null, RefreshMapStageEventArgs.Create());

        }
    }
}