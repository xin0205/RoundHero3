
using System.Linq;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public enum ETutorialState
    {
        Match,
        UnMatch,
        None,
    }
    
    public class TutorialManager : Singleton<TutorialManager>
    {

        public void Init()
        {
            stepIntverval = -5;
        }

        private float stepIntverval = 0;
        
        public ETutorialState Check_SelectUnitCard(BattleCardEntity battleCardEntity)
        {
            if (BattleManager.Instance.TutorialStep == ETutorialStep.End)
            {
                return ETutorialState.None;
            }
            else if (GamePlayManager.Instance.GamePlayData.IsTutorialBattle)
            {
                //var preStep = GetPreStep(ETutorialStep.SelectUnitCard);
                if (BattleManager.Instance.TutorialStep == ETutorialStep.SelectUnitCard)
                {
                    if (battleCardEntity.BattleCardEntityData.HandSortingIdx == 0)
                    {
                        return ETutorialState.Match;
                    }


                }
                
                return ETutorialState.UnMatch;
            }
            else
            {
                return ETutorialState.None;
            }


            
        }
        
        public ETutorialState Check_SelectMoveCard(BattleCardEntity battleCardEntity)
        {
            if (BattleManager.Instance.TutorialStep == ETutorialStep.End)
            {
                return ETutorialState.None;
            }
            else if (GamePlayManager.Instance.GamePlayData.IsTutorialBattle)
            {
                //var preStep = GetPreStep(ETutorialStep.SelectMoveCard);
                if (BattleManager.Instance.TutorialStep == ETutorialStep.SelectMoveCard)
                {
                    if (battleCardEntity.BattleCardEntityData.HandSortingIdx == 0)
                    {
                        return ETutorialState.Match;
                    }


                }
                
                return ETutorialState.UnMatch;
            }
            else
            {
                return ETutorialState.None;
            }


            
        }
        
        public ETutorialState Check_UseUnitCard(int gridPosIdx)
        {
            if (BattleManager.Instance.TutorialStep == ETutorialStep.End)
            {
                return ETutorialState.None;
            }
            else if (GamePlayManager.Instance.GamePlayData.IsTutorialBattle)
            {
                //var preStep = GetPreStep(ETutorialStep.UseUnitCard);
                if (BattleManager.Instance.TutorialStep == ETutorialStep.UseUnitCard)
                {
                    if (gridPosIdx == Constant.Tutorial.UseUnitCardGridPosIdx)
                    {
                        return ETutorialState.Match;
                    }
                    

                }
                
                return ETutorialState.UnMatch;
            }
            else
            {
                return ETutorialState.None;
            }

            
        }
        
        public ETutorialState Check_SelectMoveUnit(int gridPosIdx)
        {
            if (BattleManager.Instance.TutorialStep == ETutorialStep.End)
            {
                return ETutorialState.None;
            }
            else if (GamePlayManager.Instance.GamePlayData.IsTutorialBattle)
            {
                //var preStep = GetPreStep(ETutorialStep.SelectMoveUnit);
                if (BattleManager.Instance.TutorialStep == ETutorialStep.SelectMoveUnit)
                {
                    if (gridPosIdx == Constant.Tutorial.UseUnitCardGridPosIdx)
                    {
                        return ETutorialState.Match;
                    }
                    

                }
                
                return ETutorialState.UnMatch;
            }
            else
            {
                return ETutorialState.None;
            }

            
        }
        
        public ETutorialState Check_SelectMovePos(int gridPosIdx)
        {
            if (BattleManager.Instance.TutorialStep == ETutorialStep.End)
            {
                return ETutorialState.None;
            }
            else if (GamePlayManager.Instance.GamePlayData.IsTutorialBattle)
            {
                //var preStep = GetPreStep(ETutorialStep.SelectMovePos);
                if (BattleManager.Instance.TutorialStep == ETutorialStep.SelectMovePos)
                {
                    if (gridPosIdx == Constant.Tutorial.MoveGridPosIdx)
                    {
                        return ETutorialState.Match;
                    }
                    

                }
                
                return ETutorialState.UnMatch;
            }
            else
            {
                return ETutorialState.None;
            }

            
        }
        
        public ETutorialState Check_SelectAttackCard(BattleCardEntity battleCardEntity)
        {
            if (BattleManager.Instance.TutorialStep == ETutorialStep.End)
            {
                return ETutorialState.None;
            }
            else if (GamePlayManager.Instance.GamePlayData.IsTutorialBattle)
            {
                //var preStep = GetPreStep(ETutorialStep.SelectAttackCard);
                if (BattleManager.Instance.TutorialStep == ETutorialStep.SelectAttackCard)
                {
                    if (battleCardEntity.BattleCardEntityData.HandSortingIdx == 0)
                    {
                        return ETutorialState.Match;
                    }
                    

                }
                
                return ETutorialState.UnMatch;
            }
            else
            {
                return ETutorialState.None;
            }

            
        }
        
        public ETutorialState Check_SelectAttackUnit(int gridPosIdx)
        {
            if (BattleManager.Instance.TutorialStep == ETutorialStep.End)
            {
                return ETutorialState.None;
            }
            else if (GamePlayManager.Instance.GamePlayData.IsTutorialBattle)
            {
                //var preStep = GetPreStep(ETutorialStep.SelectAttackUnit);
                if (BattleManager.Instance.TutorialStep == ETutorialStep.SelectAttackUnit)
                {
                    if (gridPosIdx == Constant.Tutorial.MoveGridPosIdx)
                    {
                        return ETutorialState.Match;
                    }
                    

                }
                
                return ETutorialState.UnMatch;
            }
            else
            {
                return ETutorialState.None;
            }

            
        }

        public ETutorialState CheckStep(ETutorialStep tutorialStep)
        {
            if (BattleManager.Instance.TutorialStep == ETutorialStep.End)
            {
                
                GameManager.Instance.GameData.User.IsEndTutorial = true;
                return ETutorialState.None;
            }
            else if (GamePlayManager.Instance.GamePlayData.IsTutorialBattle)
            {
                if (BattleManager.Instance.TutorialStep == tutorialStep)
                {
                    return ETutorialState.Match;

                }
                else
                {
                    return ETutorialState.UnMatch;
                }
            }

            return ETutorialState.None;
        }
        
        public ETutorialState Switch_SelectUnitCard(BattleCardEntity battleCardEntity)
        {
            var state = Check_SelectUnitCard(battleCardEntity);
            if (state == ETutorialState.Match)
            {
                stepIntverval = 0;
                var nextStep = GetNextStep(ETutorialStep.SelectUnitCard);
                BattleManager.Instance.TutorialStep = nextStep;
                GameEntry.Event.Fire(null, RefreshTutorialEventArgs.Create());
            }

            return state;
        }
        
        public ETutorialState Switch_SelectMoveCard(BattleCardEntity battleCardEntity)
        {
            var state = Check_SelectMoveCard(battleCardEntity);
            if (state == ETutorialState.Match)
            {
                stepIntverval = 0;
                var nextStep = GetNextStep(ETutorialStep.SelectMoveCard);
                BattleManager.Instance.TutorialStep = nextStep;
                GameEntry.Event.Fire(null, RefreshTutorialEventArgs.Create());
            }

            return state;
        }
        
        public ETutorialState Switch_SelectAttackCard(BattleCardEntity battleCardEntity)
        {
            var state = Check_SelectAttackCard(battleCardEntity);
            if (state == ETutorialState.Match)
            {
                stepIntverval = 0;
                var nextStep = GetNextStep(ETutorialStep.SelectAttackCard);
                BattleManager.Instance.TutorialStep = nextStep;
                GameEntry.Event.Fire(null, RefreshTutorialEventArgs.Create());
            }

            return state;
        }
        
        public ETutorialState Switch_UseUnitCard(int gridPosIdx)
        {
            var state = Check_UseUnitCard(gridPosIdx);
            if (state == ETutorialState.Match)
            {
                stepIntverval = 0;
                var nextStep = GetNextStep(ETutorialStep.UseUnitCard);
                BattleManager.Instance.TutorialStep = nextStep;
                GameEntry.Event.Fire(null, RefreshTutorialEventArgs.Create());
            }

            return state;
        }
        
        public ETutorialState Switch_SelectMoveUnit(int gridPosIdx)
        {
            var state = Check_SelectMoveUnit(gridPosIdx);
            if (state == ETutorialState.Match)
            {
                stepIntverval = 0;
                var nextStep = GetNextStep(ETutorialStep.SelectMoveUnit);
                BattleManager.Instance.TutorialStep = nextStep;
                GameEntry.Event.Fire(null, RefreshTutorialEventArgs.Create());
            }

            return state;
        }
        
        public ETutorialState Switch_SelectMovePos(int gridPosIdx)
        {
            var state = Check_SelectMovePos(gridPosIdx);
            if (state == ETutorialState.Match)
            {
                stepIntverval = 0;
                var nextStep = GetNextStep(ETutorialStep.SelectMovePos);
                BattleManager.Instance.TutorialStep = nextStep;
                GameEntry.Event.Fire(null, RefreshTutorialEventArgs.Create());
            }

            return state;
        }
        
        public ETutorialState Switch_SelectAttackUnit(int gridPosIdx)
        {
            var state = Check_SelectAttackUnit(gridPosIdx);
            if (state == ETutorialState.Match)
            {
                stepIntverval = 0;
                var nextStep = GetNextStep(ETutorialStep.SelectAttackUnit);
                BattleManager.Instance.TutorialStep = nextStep;
                GameEntry.Event.Fire(null, RefreshTutorialEventArgs.Create());
            }

            return state;
        }
        
        

        public ETutorialStep GetPreStep(ETutorialStep tutorialStep)
        {
            for (int i = 0; i < Constant.Tutorial.StepSort.Count; i++)
            {
                if (Constant.Tutorial.StepSort[i] == tutorialStep)
                {
                    if (i - 1 >= 0 && i - 1 < Constant.Tutorial.StepSort.Count - 1)
                    {
                        return Constant.Tutorial.StepSort[i - 1];
                    }
                    
                }
            }

            return tutorialStep;

        }
        
        public ETutorialStep GetNextStep(ETutorialStep tutorialStep)
        {
            for (int i = 0; i < Constant.Tutorial.StepSort.Count; i++)
            {
                if (Constant.Tutorial.StepSort[i] == tutorialStep)
                {
                    if (i + 1 >= 0 && i + 1 < Constant.Tutorial.StepSort.Count)
                    {
                        return Constant.Tutorial.StepSort[i + 1];
                    }
                }
            }

            return tutorialStep;
        }

        public ETutorialState SwitchStep(ETutorialStep tutorialStep)
        {
            var nextStep = GetNextStep(tutorialStep);
            if (nextStep != tutorialStep)
            {
                stepIntverval = 0;
                return Switch_Common(tutorialStep, nextStep);
            }

            return ETutorialState.UnMatch;
        }


        
        public ETutorialState Switch_Common(ETutorialStep tutorialStep1, ETutorialStep tutorialStep2)
        {
            var state = CheckStep(tutorialStep1);
            if (state == ETutorialState.Match)
            {
                BattleManager.Instance.TutorialStep = tutorialStep2;
                GameEntry.Event.Fire(null, RefreshTutorialEventArgs.Create());
            }
            

            return state;
        }

        public bool IsTutorial()
        {
            return GamePlayManager.Instance.GamePlayData.IsTutorialBattle;
        }

        public bool CheckTutorialEnd()
        {
            return IsTutorial() && BattleManager.Instance.TutorialStep >= ETutorialStep.End;
        }

        public void Update()
        {

            stepIntverval += Time.deltaTime;
            if (stepIntverval < Constant.Tutorial.StepInterval)
                return;
            
            //stepIntverval += Constant.Tutorial.StepInterval + 1;
            
            if (Input.GetMouseButtonDown(0))
            {
                
                var tutorialStep = BattleManager.Instance.TutorialStep;
                //Log.Debug("AE:" +BattleManager.Instance.TutorialStep);
                // SwitchStep(ETutorialStep.CoreHP);
                // if(tutorialStep != BattleManager.Instance.TutorialStep)
                //     return;
                
                SwitchStep(ETutorialStep.CoreHP);
                if(tutorialStep != BattleManager.Instance.TutorialStep)
                    return;
                
                SwitchStep(ETutorialStep.Core);
                if(tutorialStep != BattleManager.Instance.TutorialStep)
                    return;
                
                SwitchStep(ETutorialStep.CoreHPDelta);
                if(tutorialStep != BattleManager.Instance.TutorialStep)
                    return;
                
                // SwitchStep(ETutorialStep.UseCardEnergy);
                // if(tutorialStep != BattleManager.Instance.TutorialStep)
                //     return;
                
                SwitchStep(ETutorialStep.UseCardEnergy);
                if(tutorialStep != BattleManager.Instance.TutorialStep)
                    return;
                
                SwitchStep(ETutorialStep.UnitOwnEnergy);
                if(tutorialStep != BattleManager.Instance.TutorialStep)
                    return;
                
                
                SwitchStep(ETutorialStep.SwitchTarget);
                if(tutorialStep != BattleManager.Instance.TutorialStep)
                    return;
                
                SwitchStep(ETutorialStep.UnitHurt);
                if(tutorialStep != BattleManager.Instance.TutorialStep)
                    return;
                
               
                SwitchStep(ETutorialStep.ContinueBattle);
                if(tutorialStep != BattleManager.Instance.TutorialStep)
                    return;

            }
        }
    }
}