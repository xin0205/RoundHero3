using System;

namespace RoundHero
{
    
    
    public class BattleEnergyBuffManager : Singleton<BattleEnergyBuffManager>
    {
        // private Dictionary<EHeroID, Dictionary<int, EnergyBuffPoint>> energyBuffPoints = new ();
        //
        // public Data_BattleCurse BattleCurseData => BattlePlayerManager.Instance.BattlePlayerData.EnergyBuffPointDict;
        
        public System.Random Random;
        private int randomSeed;

        public void Init(int randomSeed)
        {
            this.randomSeed = randomSeed;
            Random = new System.Random(this.randomSeed);
            
        }
        
        public void Destory()
        {
            
        }
        
        
        public void InitHero(Data_BattleHero hero)
        {
            // var energyBuffDatas = GamePlayManager.Instance.GamePlayData.PlayerDatas[hero.UnitCamp]
            //     .EnergyBuffDatas;
            // var drHero = GameEntry.DataTable.GetHero(hero.HeroID);
            //
            // var idx = 0;
            // var energyBuffIdx = 0;
            // for (int i = drHero.Heart; i > 0 ; i--)
            // {
            //     energyBuffIdx = 0;
            //     for (int j = drHero.HP; j > 0 ; j--)
            //     {
            //         if (energyBuffIdx < drHero.EnergyBuffIntervals.Count && drHero.EnergyBuffIntervals[energyBuffIdx] == idx)
            //         {
            //             energyBuffDatas.Add(i * 100 + j, new Data_EnergyBuff()
            //             {
            //                 Heart = i,
            //                 HP = j,
            //                 EnergyBuffIdx = energyBuffIdx,
            //                 CardID = 
            //             });
            //             energyBuffIdx++;
            //             idx = 0;
            //         }
            //         
            //         if (j == 1)
            //         {
            //             idx = 0;
            //         }
            //         else
            //         {
            //             idx += 1;
            //         }
            //     }
            // }
        }

        public void HeroCurHPChanged(Data_BattleHero hero, int deltaHP)
        {
            if(PlayerManager.Instance.PlayerData.UnitCamp != hero.UnitCamp)
                return;
            
            if(deltaHP > 0)
                return;
            
            var energyBuffDatas = GamePlayManager.Instance.GamePlayData.PlayerDataCampDict[hero.UnitCamp].EnergyBuffDatas;

            var copyHero = hero.Copy();
            var eachDeltaHP = deltaHP / Math.Abs(deltaHP); 
            var times = Math.Abs(deltaHP);
            while (true)
            {
                
                copyHero.ChangeHP(eachDeltaHP);
                times -= 1;
                if(times < 0)
                    break;

                var key = (int)copyHero.Attribute.GetAttribute(EHeroAttribute.CurHeart) * 100 + copyHero.CurHP;
                if(energyBuffDatas.ContainsKey(key))
                {
                    energyBuffDatas[key].EffectCount += 1;
                }
                
            }
            
            
        }

        public Data_EnergyBuff GetEnergyBuff(EUnitCamp unitCamp, int heart, int hp)
        {
            var energyBuffDatas = GamePlayManager.Instance.GamePlayData.PlayerDataCampDict[unitCamp].EnergyBuffDatas;
            
            var key = (int)heart * 100 + hp;
            if(energyBuffDatas.ContainsKey(key))
            {
                return energyBuffDatas[key];
            }

            return null;
        }
        
        public Data_EnergyBuff GetEnergyBuff(EUnitCamp unitCamp, int cardID)
        {
            //var energyBuffDatas = GamePlayManager.Instance.GamePlayData.PlayerDatas[unitCamp].EnergyBuffDatas;
            //
            // var drHero = GameEntry.DataTable.GetHero(GamePlayManager.Instance.GamePlayData.PlayerDatas[unitCamp]
            //     .BattleHero.HeroID);
            //
            //
            // foreach (var kv in energyBuffDatas)
            // {
            //     
            //     if(drHero.EnergyCardIDs[kv.Value.EnergyBuffIdx])
            // }
            //
            // var key = (int)heart * 100 + hp;
            // if(energyBuffDatas.ContainsKey(key))
            // {
            //     return energyBuffDatas[key];
            // }

            return null;
        }
        
        public void CacheTacticCardData(Data_BattleHero hero, int heart, int hp, EUnitCamp camp, Data_BattleUnit effectUnit)
        {
            var drHero = GameEntry.DataTable.GetHero(hero.HeroID);
            
            var energyBuffPoint = BattleEnergyBuffManager.Instance.GetEnergyBuff(hero.UnitCamp, heart, hp);
            
            //BattleBuffManager.Instance.CacheBuffData(drHero.EnergyBuffIDs[energyBuffPoint.EnergyBuffIdx], camp, effectUnit, drHero.EnergyBuffValues, 1);

        }

        public bool TriggerEnergyBuff(Data_EnergyBuff energyBuffData)
        {
            var drHero = GameEntry.DataTable.GetHero(HeroManager.Instance.BattleHeroData.HeroID);
            var buffID = drHero.EnergyBuffIDs[energyBuffData.EnergyBuffIdx];
            

            var buffData = BattleBuffManager.Instance.GetBuffData(buffID);

            // if (drCard.CardType == ECardType.Unit)
            // {
            //     BattleManager.Instance.BattleState = EBattleState.UnitSelectGrid;
            //     BattleManager.Instance.TempTriggerData.TriggerBuffData.TriggerBuffType = TriggerBuffType.EnergyBuff;
            //     BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData = energyBuffData.Copy();
            //     return false;
            // }
            // else 
            
            if (buffData.BuffTriggerType == EBuffTriggerType.SelectUnit)
            {
                BattleManager.Instance.BattleState = EBattleState.TacticSelectUnit;
                BattleManager.Instance.TempTriggerData.TriggerBuffData.TriggerBuffType = TriggerBuffType.EnergyBuff;
                BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData = energyBuffData.Copy();


                return false;
            }

            return true;
        }

        public void UseEnergyBuff(int heart, int hp)
        {
            var energyBuff = GetEnergyBuff(HeroManager.Instance.BattleHeroData.UnitCamp, heart, hp);
            energyBuff.EffectCount = 0;
            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
            
            BattleBuffManager.Instance.RecoverUseBuffState();
            BattleManager.Instance.Refresh();
            
        }
    }
}