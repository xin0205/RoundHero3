
using UnityEngine;

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
                var preStep = GetPreStep(ETutorialStep.SelectUnitCard);
                if (BattleManager.Instance.TutorialStep == preStep)
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
                var preStep = GetPreStep(ETutorialStep.SelectMoveCard);
                if (BattleManager.Instance.TutorialStep == preStep)
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
                var preStep = GetPreStep(ETutorialStep.UseUnitCard);
                if (BattleManager.Instance.TutorialStep == preStep)
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
                var preStep = GetPreStep(ETutorialStep.SelectMoveUnit);
                if (BattleManager.Instance.TutorialStep == preStep)
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
                var preStep = GetPreStep(ETutorialStep.SelectMovePos);
                if (BattleManager.Instance.TutorialStep == preStep)
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
                var preStep = GetPreStep(ETutorialStep.SelectAttackCard);
                if (BattleManager.Instance.TutorialStep == preStep)
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
                var preStep = GetPreStep(ETutorialStep.SelectAttackUnit);
                if (BattleManager.Instance.TutorialStep == preStep)
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
                BattleManager.Instance.TutorialStep = ETutorialStep.SelectUnitCard;
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
                BattleManager.Instance.TutorialStep = ETutorialStep.SelectMoveCard;
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
                BattleManager.Instance.TutorialStep = ETutorialStep.SelectAttackCard;
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
                BattleManager.Instance.TutorialStep = ETutorialStep.UseUnitCard;
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
                BattleManager.Instance.TutorialStep = ETutorialStep.SelectMoveUnit;
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
                BattleManager.Instance.TutorialStep = ETutorialStep.SelectMovePos;
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
                BattleManager.Instance.TutorialStep = ETutorialStep.SelectAttackUnit;
                GameEntry.Event.Fire(null, RefreshTutorialEventArgs.Create());
            }

            return state;
        }
        
        

        public ETutorialStep GetPreStep(ETutorialStep tutorialStep)
        {
            return (ETutorialStep)((int)tutorialStep - 1);
        }
        
        public ETutorialStep GetNextStep(ETutorialStep tutorialStep)
        {
            return (ETutorialStep)((int)tutorialStep + 1);
        }

        public ETutorialState SwitchStep(ETutorialStep tutorialStep)
        {
            var preStep = GetPreStep(tutorialStep);
            if (preStep != null)
            {
                stepIntverval = 0;
                return Switch_Common(preStep, tutorialStep);
            }

            return ETutorialState.None;
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
                
                SwitchStep(ETutorialStep.CoreHP);
                if(tutorialStep != BattleManager.Instance.TutorialStep)
                    return;
                
                SwitchStep(ETutorialStep.Core);
                if(tutorialStep != BattleManager.Instance.TutorialStep)
                    return;
                
                SwitchStep(ETutorialStep.CoreHPDelta);
                if(tutorialStep != BattleManager.Instance.TutorialStep)
                    return;
                
                SwitchStep(ETutorialStep.UseCardEnergy);
                if(tutorialStep != BattleManager.Instance.TutorialStep)
                    return;
                
                SwitchStep(ETutorialStep.UnitOwnEnergy);
                if(tutorialStep != BattleManager.Instance.TutorialStep)
                    return;
                
                // SwitchStep(ETutorialStep.UnitHurt);
                // if(tutorialStep != BattleManager.Instance.TutorialStep)
                //     return;
                
                SwitchStep(ETutorialStep.End);
                if(tutorialStep != BattleManager.Instance.TutorialStep)
                    return;

            }
        }
    }
}