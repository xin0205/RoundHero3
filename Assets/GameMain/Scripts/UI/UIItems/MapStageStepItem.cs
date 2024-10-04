using UnityEngine;
using System;
using Random = System.Random;

namespace RoundHero
{
    public class MapStageStepItem : MonoBehaviour
    {
        [SerializeField] private EMapSite mapSite;
        [SerializeField] private MapSiteIcon mapSiteIcon;
        
        [SerializeField] private bool allowClick;
        
        private Data_MapStep mapStep;

        private int randomSeed;
        private System.Random random;

        public void Init(Data_MapStep mapStep, EMapSite mapSite, int randomSeed)
        {
            this.mapSite = mapSite;
            this.mapStep = mapStep;
            this.randomSeed = randomSeed;
            this.random = new Random(this.randomSeed);
            
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