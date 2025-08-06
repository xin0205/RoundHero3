using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class CommonEffectEntity : EffectEntity
    {
        [SerializeField]
        protected EColorGODictionary effectPrefabs;

        protected GameObject effectPrefab;

        
        public CommonEffectEntityData CommonEffectEntityData { get; protected set; }
        
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            CommonEffectEntityData = userData as CommonEffectEntityData;
            if (CommonEffectEntityData == null)
            {
                Log.Error("Error CommonEffectEntityData");
                return;
            }

            var color = CommonEffectEntityData.EffectColor;
            foreach (var kv in effectPrefabs)
            {
                if(kv.Value == null)
                    continue;
                kv.Value.SetActive(kv.Key == color);
  
            }
        }
        
        protected virtual void ShowEffect()
        {
            
            
            
            
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