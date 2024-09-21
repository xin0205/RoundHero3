using UnityEngine;

namespace RoundHero
{
    public class MapStageStepItem : MonoBehaviour
    {
        [SerializeField] private EMapSite mapSite;
        [SerializeField] private MapSiteIcon mapSiteIcon;
        
        [SerializeField] private bool allowClick;
        
        private Data_MapStep mapStep;

        public void Init(Data_MapStep mapStep, EMapSite mapSite)
        {
            this.mapSite = mapSite;
            this.mapStep = mapStep;
            
            mapSiteIcon.Init(this.mapSite);
        }

        public void Click()
        {
            GameEntry.Event.Fire(null, ClickMapStageStepItemEventArgs.Create(this.mapStep, this.mapSite));
        }
    }
}