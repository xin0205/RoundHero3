using System.Collections.Generic;

namespace RoundHero
{
    public class BattlePlayerManager : Singleton<BattlePlayerManager>
    {
        public Data_BattlePlayer BattlePlayerData;
        public Data_Player PlayerData;
        
        
        
        public void SetCurPlayer()
        {
            BattlePlayerData = GamePlayManager.Instance.GamePlayData.BattleData.GetBattlePlayerData(BattleManager.Instance.CurUnitCamp);
            PlayerData = GamePlayManager.Instance.GamePlayData.GetPlayerData(BattleManager.Instance.CurUnitCamp);

        }
        
        public void InitData(EUnitCamp unitCamp)
        {
            var playerData = GamePlayManager.Instance.GamePlayData.GetPlayerData(unitCamp);
            playerData.Coin = Constant.Hero.InitDatas[unitCamp].Coin;
            
            
            foreach (var funeID in Constant.Hero.InitDatas[unitCamp].InitFunes)
            {
                var funeIdx = playerData.FuneIdx++;
                var drFune = GameEntry.DataTable.GetBuff(funeID);
                var value = drFune == null ? 0 : BattleBuffManager.Instance.GetBuffValue(drFune.BuffValues[0]);
                playerData.FuneDatas.Add(funeIdx, new Data_Fune(funeIdx, funeID, (int)value));
                playerData.UnusedFuneIdxs.Add(funeIdx);
            }
            
            foreach (var blessID in Constant.Hero.InitDatas[unitCamp].InitBlesses)
            {
                var blessIdx = playerData.BlessIdx++;
                //var drBless = GameEntry.DataTable.GetBless(blessID);
                
                playerData.BlessDatas.Add(blessIdx, new Data_Bless(blessIdx, blessID));
                
            }
            
            foreach (var cardID in GameManager.Instance.TmpInitCards)
            {
                var cardIdx = playerData.CardIdx++;
                playerData.CardDatas.Add(cardIdx, new Data_Card(cardIdx, cardID));
            }

            // var energyBuffDict = new Dictionary<int, string>();
            // var energyBuffIdx = 0;
            // var drHero = GameEntry.DataTable.GetHero(playerData.BattleHero.HeroID);
            // foreach (var buffStr in drHero.EnergyBuffIDs)
            // {
            //     var buffData = BattleBuffManager.Instance.GetBuffData(buffStr);
            //     
            //     // var cardID = playerData.CardIDIdx++;
            //     // playerData.EnergyBuffDatas.Add(cardID, new Data_EnergyBuff(cardID, buffData.DrBuff.Id, new List<int>()));
            //     
            //     energyBuffDict.Add(energyBuffIdx++, buffData.BuffStr);
            // }
            //
            // var idx = 0;
            // for (int i = drHero.Heart; i > 0 ; i--)
            // {
            //     energyBuffIdx = 0;
            //     for (int j = drHero.HP; j > 0 ; j--)
            //     {
            //         if (energyBuffIdx < drHero.EnergyBuffIntervals.Count && drHero.EnergyBuffIntervals[energyBuffIdx] == idx)
            //         {
            //             playerData.EnergyBuffDatas.Add(i * 100 + j, new Data_EnergyBuff()
            //             {
            //                 Heart = i,
            //                 HP = j,
            //                 EnergyBuffIdx = energyBuffIdx,
            //                 BuffStr = energyBuffDict[energyBuffIdx],
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

    }
}