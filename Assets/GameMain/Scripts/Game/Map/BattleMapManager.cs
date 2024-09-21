using System;
using System.Collections.Generic;

namespace RoundHero
{
    public class BattleMapManager : Singleton<BattleMapManager>
    {
        
        public Data_Map MapData => GamePlayManager.Instance.GamePlayData.MapData;
        
        
        public System.Random Random;
        private int randomSeed;

        public void Init(int randomSeed)
        {
            this.randomSeed = randomSeed;
            Random = new System.Random(this.randomSeed);

        }

        private void ChangeStageMapSite(List<List<EMapSite>> stage, int stepIdx, EMapSite mapSite)
        {
            var oriStepIdx = stepIdx;
            foreach (var mapSites in stage)
            {
                oriStepIdx = stepIdx;
                stepIdx -= mapSites.Count;
                if (stepIdx < 0)
                {
                    mapSites[oriStepIdx] = mapSite;
                    break;
                }

            }
        }

        public List<List<EMapSite>> GenerateStage(int randomSeed)
        {
            var  stage = new List<List<EMapSite>>();
            var random = new Random(randomSeed);
            
            var mapSiteRatios = new List<EMapSite>();
            foreach (var kv in Constant.Map.MapSiteRatio)
            {
                for (int i = 0; i < kv.Value; i++)
                {
                    mapSiteRatios.Add(kv.Key);
                }
            }
            
            var stepRangeRatios = new List<int>();
            foreach (var kv in Constant.Map.StepRangeRatio)
            {
                for (int i = 0; i < kv.Value; i++)
                {
                    stepRangeRatios.Add(kv.Key);
                }
            }

            var stepCount = 0;
            for (int i = 0; i < Constant.Map.StepRouteCount; i++)
            {
                var mapSites = new List<EMapSite>();
                var stepIdx = random.Next(0, 100);
                for (int j = 0; j < stepRangeRatios[stepIdx]; j++)
                {
                    mapSites.Add(EMapSite.Empty);
                    stepCount++;
                   
                }
                stage.Add(mapSites);
            }

            var stepIdxs = MathUtility.GetRandomNum(stepCount, 0, stepCount, random);

            var idx = 0;
            foreach (var kv in Constant.Map.MapSiteGuarantee)
            {
                for (int i = 0; i < kv.Value; i++)
                {
                    ChangeStageMapSite(stage, stepIdxs[idx], kv.Key);
                    idx++;
                }
            }

            for (int i = idx; i < stepCount; i++)
            {
                var mapSiteIdx = random.Next(0, 100);
                ChangeStageMapSite(stage, stepIdxs[i], mapSiteRatios[mapSiteIdx]);
            }

            return stage;

        }

        public void NextStep()
        {
            BattleMapManager.Instance.MapData.CurMapStageIdx.StepIdx += 1;
            
            var stageIdx = BattleMapManager.Instance.MapData.CurMapStageIdx.StageIdx;
            var mapStageData = BattleMapManager.Instance.MapData.MapStageDataDict[stageIdx];
            var stage = BattleMapManager.Instance.GenerateStage(mapStageData.StageRandomSeed);

            if (BattleMapManager.Instance.MapData.CurMapStageIdx.StepIdx >= stage[mapStageData.SelectRouteIdx].Count)
            {
                BattleMapManager.Instance.MapData.CurMapStageIdx.StepIdx = -1;
                BattleMapManager.Instance.MapData.CurMapStageIdx.StageIdx += 1;
                BattleMapManager.Instance.MapData.CurMapStageIdx.IsSelectRoute = false;
                if (BattleMapManager.Instance.MapData.CurMapStageIdx.StageIdx >= Constant.Map.StageCount)
                {
                    BattleMapManager.Instance.MapData.CurMapStageIdx.StageIdx = 0;
                    BattleMapManager.Instance.MapData.CurMapStageIdx.MapIdx += 1;
                    if (BattleMapManager.Instance.MapData.CurMapStageIdx.MapIdx >= Constant.Map.MapCount)
                    {
                        BattleMapManager.Instance.MapData.CurMapStageIdx.MapIdx = -1;
                    }

                }
                
            }
            
            if (!BattleMapManager.Instance.MapData.CurMapStageIdx.IsSelectRoute && BattleMapManager.Instance.MapData.CurMapStageIdx.MapIdx != -1)
            {
                GameEntry.UI.OpenUIForm(UIFormId.MapStageRouteSelectForm);

            }
        }
    }
}