﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Steamworks;

namespace RoundHero
{
    public class HeroManager : Singleton<HeroManager>
    {
        public Data_BattleHero BattleHeroData
        {
            get
            {
                return BattlePlayerManager.Instance.PlayerData.BattleHero;
            }
            
            set
            {
                BattlePlayerManager.Instance.PlayerData.BattleHero = value;
            }

        }


        //private int id;
        public Random Random;
        private int randomSeed;
        
        public Dictionary<ulong, BattleHeroEntity> HeroEntities = new ();
        
        public BattleHeroEntity HeroEntity =>  HeroEntities[PlayerManager.Instance.PlayerData.PlayerID];
        
        public void Init(int randomSeed)
        {
            this.randomSeed = randomSeed;
            Random = new System.Random(this.randomSeed);
            //HeroEntities.Clear();
            BattleUnitManager.Instance.BattleUnitDatas.Add(HeroManager.Instance.BattleHeroData.ID, HeroManager.Instance.BattleHeroData);
            
            //id = 0;
        }

        public void Destory()
        {
            HeroEntities.Clear();
        }

        // public int GetID()
        // {
        //     return id++;
        // }

        public void InitHeroData(EHeroID heroID)
        {
            BattleHeroData = new Data_BattleHero(BattleUnitManager.Instance.GetID(),
                heroID, 0, BattleManager.Instance.CurUnitCamp, new List<int>());
            BattleHeroData.UnitRole = EUnitRole.Hero;
            
        }

        public async Task GenerateHero()
        {
            BattleAreaManager.Instance.RefreshObstacles();
            
            var places = BattleAreaManager.Instance.GetPlaces();
            var randoms = MathUtility.GetRandomNum(1, 0,
                places.Count, Random);
            BattleHeroData.GridPosIdx = places[randoms[0]];
            
            var heroEntity = await GameEntry.Entity.ShowBattleHeroEntityAsync(BattleHeroData);
            
            if (heroEntity is IMoveGrid moveGrid)
            {
                BattleAreaManager.Instance.MoveGrids.Add(heroEntity.BattleHeroEntityData.Id, moveGrid);
            }
            
            BattleUnitManager.Instance.BattleUnitEntities.Add(heroEntity.BattleHeroEntityData.BattleHeroData.ID, heroEntity);
            HeroEntities.Add(PlayerManager.Instance.GetPlayerID(BattleManager.Instance.CurUnitCamp), heroEntity);
        }
        
        // public bool IsHero(int gridPosIdx)
        // {
        //     return HeroEntity.GridPosIdx == gridPosIdx;
        // }
        
        public void ChangeHP(int deltaHP, EHPChangeType hpChangeType, bool useDefense = true, bool addHeroHP = false, bool changeHPInstantly = false)
        {
            BattleEnergyBuffManager.Instance.HeroCurHPChanged(BattleHeroData, deltaHP);
            
            BattleManager.Instance.ChangeHP(BattleHeroData, deltaHP, GamePlayManager.Instance.GamePlayData, hpChangeType, useDefense, addHeroHP, changeHPInstantly);
            
            // var curEnergy = BattleHeroData.Attribute.GetAttribute(EHeroAttribute.CurEnergy);
            // var maxEnergy = BattleHeroData.Attribute.GetAttribute(EHeroAttribute.MaxEnergy);
            //
            // BattleHeroData.Attribute.SetAttribute(EHeroAttribute.CurEnergy,
            //     curEnergy + deltaHP);
            // curEnergy = BattleHeroData.Attribute.GetAttribute(EHeroAttribute.CurEnergy);
            //
            // BattleHeroData.Attribute.SetAttribute(EHeroAttribute.CurEnergy,  curEnergy > maxEnergy
            //     ? maxEnergy
            //     : curEnergy);
            // curEnergy = BattleHeroData.Attribute.GetAttribute(EHeroAttribute.CurEnergy);
            //
            // BattleHeroData.Attribute.SetAttribute(EHeroAttribute.CurEnergy, curEnergy < 0
            //     ? 0
            //     : curEnergy);
            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
        }
        
        public void RecoverEnergy()
        {
            //ChangeHP((int)BattleHeroData.Attribute.GetAttribute(EHeroAttribute.RecoverEnergy));
   
        }
        
        public void ClearEnergy()
        {
            // BattleHeroData.Attribute.SetAttribute(EHeroAttribute.CurEnergy,
            //     0);
            
        }

        public bool IsHero(int unitID)
        {
            var unit = BattleUnitManager.Instance.GetUnitByID(unitID);
            if (unit == null)
                return false;

            var playerData = GamePlayManager.Instance.GamePlayData.GetPlayerData(unit.UnitCamp);

            if (playerData == null)
                return false;

            if (playerData.BattleHero == null)
                return false;

            return playerData.BattleHero.ID == unitID;
        }
        
        public List<float> GetHeroBuffValues()
        {
            return GetHeroBuffValues(BattleHeroData.HeroID);

        }
        
        public List<BuffData> GetBuffData(EHeroID heroID)
        {
            var drEnemy = RoundHero.GameEntry.DataTable.GetHero(heroID);
            
            var buffDatas = new List<BuffData>();

            foreach (var buffID in drEnemy.Buffs)
            {
                var buffData = BattleBuffManager.Instance.GetBuffData(buffID);
                buffDatas.Add(buffData);
            }

            return buffDatas;
        }
        
        public List<float> GetHeroBuffValues(EHeroID heroID)
        {
            var values = new List<float>();
            
            var drHero = GameEntry.DataTable.GetHero(heroID);
            
            foreach (var value in drHero.Values1)
            {
                var targerValue = GameUtility.GetBuffValue(value);
                values.Add(targerValue);

            }

            return values;

        }
        
        public List<List<float>> GetBuffValues(EHeroID heroID)
        {
            var drHero = GameEntry.DataTable.GetHero(heroID);
            
            var valuelist = new List<List<float>>();

            var idx = 1;
            foreach (var buffID in drHero.Buffs)
            {
                var values = new List<float>();
                foreach (var value in drHero.GetValues(idx++))
                {
                    var targetValue = GameUtility.GetBuffValue(value);
                    values.Add(targetValue);
                }
                valuelist.Add(values);
            }

            return valuelist;
        }

        public void RoundStartTrigger()
        {
            var drHero = GameEntry.DataTable.GetHero(BattleHeroData.HeroID);
            foreach (var buffID in drHero.Buffs)
            {
                var buffData = BattleBuffManager.Instance.GetBuffData(buffID);
                if (buffData.BuffTriggerType == EBuffTriggerType.AutoAttack)
                {
                    BattleHeroData.UnitState.AddState(EUnitState.AutoAtk);
                    break;
                }
            }
            
        }
    }
}