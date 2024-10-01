using UnityEngine;
using System;

namespace RoundHero
{
    public class MapStageStepItem : MonoBehaviour
    {
        [SerializeField] private EMapSite mapSite;
        [SerializeField] private MapSiteIcon mapSiteIcon;
        
        [SerializeField] private bool allowClick;
        
        private Data_MapStep mapStep;

        private System.Random random;

        public void Init(Data_MapStep mapStep, EMapSite mapSite, System.Random random)
        {
            this.mapSite = mapSite;
            this.mapStep = mapStep;
            this.random = random;
            
            mapSiteIcon.Init(this.mapSite);
        }

        public void RandomPosition()
        {
            gameObject.transform.SetLocalPositionX(random.Next(-50, 50));
        }

        public void Click()
        {
            GameEntry.Event.Fire(null, ClickMapStageStepItemEventArgs.Create(this.mapStep, this.mapSite));
        }
    }
}