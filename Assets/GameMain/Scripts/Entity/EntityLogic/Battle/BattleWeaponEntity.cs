using GameFramework;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleWeaponEntity : Entity
    {

        protected override void OnShow(object userData)

        {

            m_EntityData = userData as EntityData;
            if (m_EntityData == null)
            {
                Log.Error("Entity data is invalid.");
                return;
            }

            Name = Utility.Text.Format("[Entity {0}]", Id);
            
        }
    }
}