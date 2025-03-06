using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleCoreEntity : BattleUnitEntity
    {
        public BattleCoreEntityData BattleCoreEntityData { get; protected set; }
        
        public virtual Vector3 Position
        {
            get => transform.position; 
            set => transform.position = value;
        }

        public virtual int GridPosIdx
        {
            get => BattleCoreEntityData.BattleCoreData.GridPosIdx; 
            set => BattleCoreEntityData.BattleCoreData.GridPosIdx = value;
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            BattleCoreEntityData = userData as BattleCoreEntityData;
            if (BattleCoreEntityData == null)
            {
                Log.Error("Error BattleCoreEntityData");
                return;
            }
        }

    }
}