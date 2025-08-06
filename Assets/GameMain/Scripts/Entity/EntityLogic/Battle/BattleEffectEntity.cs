using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleEffectEntity : EffectEntity
    {
        [SerializeField]
        private EColorGODictionary effectPrefabs;

        private GameObject effectPrefab;

        
        public BattleEffectEntityData BattleEffectEntityData { get; protected set; }
        
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            BattleEffectEntityData = userData as BattleEffectEntityData;
            if (BattleEffectEntityData == null)
            {
                Log.Error("Error BattleEffectEntityData");
                return;
            }

            ShowEffect();
        }
        
        private void ShowEffect()
        {
            var color = BattleEffectEntityData.EffectColor;
            foreach (var kv in effectPrefabs)
            {
                if(kv.Value == null)
                    continue;
                kv.Value.SetActive(kv.Key == color);
  
            }
            
            
            
            //Instantiate(effectPrefabs[color], transform.position, transform.rotation) as GameObject;
            //effectPrefab = Instantiate(effectPrefabs[color], Root.transform);
            //effectPrefab.transform.parent = Root.transform;
            //effectPrefab.transform.rotation = Quaternion.Euler(Vector3.zero);
            //effectPrefab.transform.localScale = Vector3.one;
            


            // if (AutoHide)
            // {
            //     GameUtility.DelayExcute(HideTime, () =>
            //     {
            //         //Destroy(effectPrefab);
            //         GameEntry.Entity.HideEntity(this);
            //     });
            // }
            
        }
    }
}