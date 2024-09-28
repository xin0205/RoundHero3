using System;
using System.Collections.Generic;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class MapForm : UGuiForm
    {
        [SerializeField]
        private List<MapStageRouteItem> MapStageRouteItems;
        private ProcedureGamePlay procedureGamePlay;
        
        private SceneEntity restSceneEntity;
    
        protected async override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            procedureGamePlay = (ProcedureGamePlay)userData;
            GameEntry.Event.Subscribe(RefreshMapStageEventArgs.EventId, OnRefreshMapStage);
            GameEntry.Event.Subscribe(ClickMapStageStepItemEventArgs.EventId, OnClickMapStageStepItem);
            
            GameEntry.Event.Fire(null, RefreshMapStageEventArgs.Create());
            
            if (!BattleMapManager.Instance.MapData.CurMapStageIdx.IsSelectRoute)
            {
                GameEntry.UI.OpenUIForm(UIFormId.MapStageRouteSelectForm);

            }
            
            restSceneEntity = await GameEntry.Entity.ShowSceneEntityAsync("Rest");

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
            for (int i = 0; i < MapStageRouteItems.Count; i++)
            {
                if (i < BattleMapManager.Instance.MapData.MapStageDataDict.Count)
                {
                    var mapStageData = BattleMapManager.Instance.MapData.MapStageDataDict[i];
                    
                    if(mapStageData.MapIdx != BattleMapManager.Instance.MapData.CurMapStageIdx.MapIdx)
                        continue;
                    
                    var stage = BattleMapManager.Instance.GenerateStage(BattleMapManager.Instance.MapData
                        .MapStageDataDict[i].StageRandomSeed);
                    MapStageRouteItems[i].Init(new Data_MapRoute()
                    {
                        MapIdx = mapStageData.MapIdx,
                        StageIdx = mapStageData.StageIdx,
                        RouteIdx = mapStageData.SelectRouteIdx,

                    }, stage[mapStageData.SelectRouteIdx], i, null);
                }
            }

        }

        private void OnClickMapStageStepItem(object sender, GameEventArgs e)
        {
            var ne = e as ClickMapStageStepItemEventArgs;
            if (ne.MapStep.MapRoute.MapIdx == BattleMapManager.Instance.MapData.CurMapStageIdx.MapIdx &&
                ne.MapStep.MapRoute.StageIdx == BattleMapManager.Instance.MapData.CurMapStageIdx.StageIdx &&
                ne.MapStep.MapRoute.RouteIdx == BattleMapManager.Instance.MapData.CurMapStageIdx.RouteIdx &&
                ne.MapStep.StepIdx == BattleMapManager.Instance.MapData.CurMapStageIdx.StepIdx + 1)
            {
                switch (ne.MapSite)
                {
                    case EMapSite.NormalBattle:
                        Close();
                        procedureGamePlay.StartBattle();
                        //PVEManager.Instance.Init(ne.MapStep.RandomSeed, EEnemyType.Normal);
                        
                        break;
                    case EMapSite.EliteBattle:
                        //PVEManager.Instance.Init(ne.MapStep.RandomSeed, EEnemyType.Elite);
                        Close();
                        procedureGamePlay.StartBattle();
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
    }
}