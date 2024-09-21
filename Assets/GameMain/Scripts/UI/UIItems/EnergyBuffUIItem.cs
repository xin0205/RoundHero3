using GameFramework.Event;
using TMPro;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class EnergyBuffUIItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        private Data_EnergyBuff energyBuffData;

        private string BuffID;


        public void OnEnable()
        {
            GameEntry.Event.Subscribe(RefreshBattleUIEventArgs.EventId, OnRefreshBattleUI);
        }
        
        public void OnDisable()
        {
            GameEntry.Event.Unsubscribe(RefreshBattleUIEventArgs.EventId, OnRefreshBattleUI);
        }
        
        public void OnRefreshBattleUI(object sender, GameEventArgs e)
        {
            Refresh();
        }

        public void Init(Data_EnergyBuff dataEnergyBuff, string buffID)
        {
            InitEnergyBuffData(dataEnergyBuff);
            BuffID = buffID;

            Refresh();
        }

        public void InitEnergyBuffData(Data_EnergyBuff dataEnergyBuff)
        {
            this.energyBuffData = dataEnergyBuff;
        }

        private void Refresh()
        {
            // switch (energyBuffType)
            // {
            //     case EEnergyBuffType.NormalEnergyBuff:
            //         text.text = normalEnergyBuff.ToString() + "_" + this.energyBuffPoint.EffectCount;
            //         break;
            //     case EEnergyBuffType.UnitState:
            //         text.text = unitState.ToString() + "_" + this.energyBuffPoint.EffectCount;
            //         break;
            //     case EEnergyBuffType.GridProp:
            //         
            //         text.text = gridPropID.ToString() + "_" + this.energyBuffPoint.EffectCount;
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException();
            // }
            
            text.text = BuffID + "_" + this.energyBuffData.EffectCount;
        }
        
        public void PreTriggerEnergyBuff()
        {
            if(this.energyBuffData.EffectCount <= 0)
                return;

            Log.Debug("PreTriggerEnergyBuff");

            if (!BattleEnergyBuffManager.Instance.TriggerEnergyBuff(this.energyBuffData))
            {
                return;
            }
            

            TriggerEnergyBuff();

        }

        public void TriggerEnergyBuff()
        {
            this.energyBuffData.EffectCount = 0;
            Log.Debug("TriggerEnergyBuff");
            Refresh();
        }
        
                
        // public void Init(EnergyBuffPoint energyBuffPoint, ENormalEnergyBuff normalEnergyBuff)
        // {
        //     InitEnergyBuffPoint(energyBuffPoint);
        //     energyBuffType = EEnergyBuffType.NormalEnergyBuff;
        //     this.normalEnergyBuff = normalEnergyBuff;
        //
        //     Refresh();
        // }
        //
        // public void Init(EnergyBuffPoint energyBuffPoint, EUnitState unitState)
        // {
        //     InitEnergyBuffPoint(energyBuffPoint);
        //     energyBuffType = EEnergyBuffType.UnitState;
        //     this.unitState = unitState;
        //
        //     Refresh();
        // }
        // public void Init(EnergyBuffPoint energyBuffPoint, EGridPropID gridPropID)
        // {
        //     InitEnergyBuffPoint(energyBuffPoint);
        //     energyBuffType = EEnergyBuffType.GridProp;
        //     this.gridPropID = gridPropID;
        //
        //     Refresh();
        // }


    }
}