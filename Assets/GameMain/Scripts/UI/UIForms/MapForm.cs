using System;
using System.Collections.Generic;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityGameFramework.Runtime;
using Image = UnityEngine.UI.Image;

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
        
        [SerializeField] private Image bg;

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
            
            procedureGamePlay.MapEntity = await GameEntry.Entity.ShowSceneEntityAsync("Map");

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
                var stageIdx = BattleMapManager.Instance.MapData.CurMapStageIdx.StageIdx;
                var routeIdx = BattleMapManager.Instance.MapData.MapStageDataDict[stageIdx].SelectRouteIdx;
                var randomSeed = BattleMapManager.Instance.MapData.MapStageDataDict[stageIdx]
                    .MapSteps[routeIdx][ne.MapStep.StepIdx].RandomSeed;
                
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
                        GameEntry.UI.OpenUIForm(UIFormId.StoreForm, new StoreFormData()
                        {
                            RandomSeed = randomSeed,
                        });
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
                        GameEntry.UI.OpenUIForm(UIFormId.TreasureForm, new TreasureFormData()
                        {
                            RandomSeed = randomSeed,
                        });
                        break;
                        
                    case EMapSite.Event:
                        GameEntry.UI.OpenUIForm(UIFormId.BattleEventForm, new BattleEventFormData()
                        {
                            RandomSeed = randomSeed,
                        });
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
        
        public void Confirm(int selectIdx)
        {
            BattleMapManager.Instance.MapData
                    .MapStageDataDict[BattleMapManager.Instance.MapData.CurMapStageIdx.StageIdx].SelectRouteIdx = selectIdx;
            MapStageRoute.SelectRouteIdx = selectIdx;
            
            //BattleMapManager.Instance.MapData.CurMapStageIdx.RouteIdx = MapStageRoute.SelectRouteIdx;
            BattleMapManager.Instance.MapData.CurMapStageIdx.IsSelectRoute = true;
            
            GameEntry.Event.Fire(null, RefreshMapStageEventArgs.Create());

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

        public void ShowBG(bool isShow)
        {
            bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, isShow ? 1 : 0.1f);
        }
        
        public void Back()
        {
            procedureGamePlay.BackToStart();
        }
    }
}