using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoundHero
{
    public class EnergyBuffBarUItem : MonoBehaviour
    {
        [SerializeField] private List<EnergyBuffUIItem> energyBuffUIItems;
        
        
        public void Init(Data_BattleHero battleHero)
        {

            foreach (var energyBuffUIItem in energyBuffUIItems)
            {
                energyBuffUIItem.gameObject.SetActive(false);
            }
            
            var drHero = GameEntry.DataTable.GetHero(battleHero.HeroID);
            var idx = 0;
            var hp = drHero.HP;
            foreach (var energyBuffID in  drHero.EnergyBuffIDs)
            {
                energyBuffUIItems[idx].gameObject.SetActive(true);
                
                hp -= drHero.EnergyBuffIntervals[idx];
                var energyBuffPoint = BattleEnergyBuffManager.Instance.GetEnergyBuff(battleHero.UnitCamp,
                    (int)battleHero.Attribute.GetAttribute(EHeroAttribute.CurHeart), hp);
                energyBuffUIItems[idx].Init(energyBuffPoint, energyBuffID);
                
                // var energyBuffIDSplits = energyBuffID.Split("_");
                // var energyBuffType = Enum.Parse<EEnergyBuffType>(energyBuffIDSplits[0]);
                // switch (energyBuffType)
                // {
                //     case EEnergyBuffType.NormalEnergyBuff:
                //         var normalEnergyBuff = Enum.Parse<ENormalEnergyBuff>(energyBuffIDSplits[1]);
                //         energyBuffUIItems[idx].Init(energyBuffPoint, normalEnergyBuff);
                //         break;
                //     case EEnergyBuffType.UnitState:
                //         var unitState = Enum.Parse<EUnitState>(energyBuffIDSplits[1]);
                //         energyBuffUIItems[idx].Init(energyBuffPoint, unitState);
                //         break;
                //     case EEnergyBuffType.GridProp:
                //         var gridPropID = Enum.Parse<EGridPropID>(energyBuffIDSplits[1]);
                //         energyBuffUIItems[idx].Init(energyBuffPoint, gridPropID);
                //         break;
                //     default:
                //         break;
                // }

                idx++;

            }
           
        }
    }
}