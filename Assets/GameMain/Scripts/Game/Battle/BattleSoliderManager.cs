using System;
using System.Collections.Generic;
using GameFramework.Event;

namespace RoundHero
{
    public enum EUnitActionType
    {
        Own,
        Linkage,
    }

    public enum ETempTriggerType
    {
        NewUnit,
        NewProp,
        MoveUnit,
        UseBuff,
        SelectHurtUnit,
        AutoAtk,
        ActiveAtk,
        Empty,
    }
    
    public class UnitActionRange
    {
        public int OwnUnitID;
        public int ActionUnitID;
        public EUnitActionType UnitActionType;
        public EBuffTriggerType BuffTriggerType;

    }

    

    public class BattleSoliderManager : Singleton<BattleSoliderManager>
    {
        //private int id;
        
        private Random Random;
        private int randomSeed;
        
        public Dictionary<int, BattleSoliderEntity> SoliderEntities = new ();
        //public Data_BattleSolider CurPointSolider;

        //public Dictionary<int, Data_BattleSolider> BattleSoliderDatas => DataManager.Instance.CurUser.GamePlayData.BattleData.BattleSoliders;
        
        
        public void RefreshSoliderEntities()
        {
            SoliderEntities.Clear();
            foreach (var kv in BattleUnitManager.Instance.BattleUnitEntities)
            {
                if ((kv.Value.UnitCamp == EUnitCamp.Player1 || kv.Value.UnitCamp == EUnitCamp.Player2) && kv.Value.UnitRole == EUnitRole.Staff)
                {
                    SoliderEntities.Add(kv.Key, kv.Value as BattleSoliderEntity);
                }
            }

        }

        public void Init(int randomSeed)
        {
            
            Subscribe();
            this.randomSeed = randomSeed;
            Random = new System.Random(this.randomSeed);
            //id = 0;
            
        }

        public void Continue()
        {
            
        }

        public void Destory()
        {
            SoliderEntities.Clear();
            Unsubscribe();
        }
        
        public void Subscribe()
        {
            GameEntry.Event.Subscribe(RefreshUnitDataEventArgs.EventId, OnRefreshUnitData);
        }

        public void Unsubscribe()
        {
            GameEntry.Event.Unsubscribe(RefreshUnitDataEventArgs.EventId, OnRefreshUnitData);
        }
        
        
        // public int GetID()
        // {
        //     return id++;
        // }
        
        // public int GetSoliderID(int gridPosIdx)
        // {
        //     var soliderID = -1;
        //
        //     foreach (var kv in SoliderEntities)
        //     {
        //         if (kv.Value.GridPosIdx == gridPosIdx)
        //         {
        //             soliderID = kv.Value.BattleSoliderEntityData.BattleSoliderData.ID;
        //         }
        //     }
        //
        //     return soliderID;
        // }

        
        
        public void OnRefreshUnitData(object sender, GameEventArgs e)
        {
            foreach (var kv in SoliderEntities)
            {
                kv.Value.RefreshData();
            }
        }

        public void RefreshSoliderHP()
        {
            // var soliderKeys = SoliderEntities.Keys.ToList();
            // foreach (var soliderKey in soliderKeys)
            // {
            //     var solider = SoliderEntities[soliderKey];
            //     var drCard = CardManager.Instance.GetCardTable(solider.BattleSoliderEntityData.BattleSoliderData.CardID);
            //     var hpRefreshType = GameUtility.GetEnum<EHPRefreshType>(drCard.HPRefreshType);
            //     if(hpRefreshType != EHPRefreshType.Round)
            //         continue;
            //     
            //     solider.ChangeCurHP(-1, false);
            //     GameEntry.Event.FireNow(null, RefreshUnitDataEventArgs.Create());
            //     if (solider.CurHP <= 0)
            //     {
            //         SoliderEntities[soliderKey].Quit();
            //     }
            // }
            
        }

        public void RemoveSolider(int soliderID)
        {
            BattleAreaManager.Instance.MoveGrids.Remove(SoliderEntities[soliderID].BattleSoliderEntityData.Id);
            BattleUnitManager.Instance.BattleUnitDatas.Remove(soliderID);
            BattleUnitManager.Instance.BattleUnitEntities.Remove(soliderID);
            
            RefreshSoliderEntities();
            BattleAreaManager.Instance.RefreshObstacles();
        }
        
        public int GetAttackInRoundCount()
        {
            var count = 0;
            foreach (var kv in SoliderEntities)
            {
                var soliderData = kv.Value.BattleSoliderEntityData;
                
                if (soliderData.BattleSoliderData.AttackInRound)
                {
                    count += 1;

                }

            }

            return count;
        }

    }
}