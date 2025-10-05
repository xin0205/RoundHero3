using System;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleBeamBulletEntity : Entity
    {
        public BattleBulletEntityData BattleBulletEntityData { get; protected set; }

        [SerializeField]
        private EColorGODictionary beamStartPrefabs;
        [SerializeField]
        private EColorGODictionary beamEndPrefabs;
        [SerializeField]
        private EColorGODictionary beamPrefabs;
        
        public bool AutoHide = false;
        public float HideTime = 3f;
        
        private GameObject beamStart;
        private GameObject beamEnd;
        private GameObject beam;
        private LineRenderer line;
        
        public float textureScrollSpeed = 8f; //How fast the texture scrolls along the beam
        public float textureLengthScale = 3; 
        
        
        
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            BattleBulletEntityData = userData as BattleBulletEntityData;
            if (BattleBulletEntityData == null)
            {
                Log.Error("Error BattleBulletEntityData");
                return;
            }
            
            if (AutoHide)
            {
                GameUtility.DelayExcute(HideTime, () =>
                {
                    Destroy(beamStart);
                    Destroy(beamEnd);
                    Destroy(beam);
                    
                    GameEntry.Entity.HideEntity(this);
                });
            }

            var color = BattleBulletEntityData.BulletData.EffectColor;
            beamStart = Instantiate(beamStartPrefabs[color], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            beamEnd = Instantiate(beamEndPrefabs[color], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            beam = Instantiate(beamPrefabs[color], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            line = beam.GetComponent<LineRenderer>();

            var startPos = GameUtility.GridPosIdxToPos(BattleBulletEntityData.BulletData.MoveGridPosIdxs[0]);
            var endPos = GameUtility.GridPosIdxToPos(BattleBulletEntityData.BulletData.MoveGridPosIdxs[BattleBulletEntityData.BulletData.MoveGridPosIdxs.Count - 1]);
            startPos.y += 1;
            endPos.y += 1;
            
            
            ShootBeam(startPos, endPos);
        }

        private void Update()
        {
            BulletUpdate();
        }

        private void BulletUpdate()
        {
            line.positionCount = 2;
            if (BattleBulletEntityData.BulletData.ActionUnitGO != null)
            {
                var startPos = BattleBulletEntityData.BulletData.ActionUnitGO.transform.position;
                //startPos.y += 1;
                beamStart.transform.position = startPos;
                line.SetPosition(0,  startPos);
            }
            
            if (BattleBulletEntityData.BulletData.EffectUnitGO != null)
            {
                var endPos = BattleBulletEntityData.BulletData.EffectUnitGO.transform.position;
                endPos.y += 1;
                beamEnd.transform.position = endPos;
                line.SetPosition(1, endPos);
            }
        }

        void ShootBeam(Vector3 start, Vector3 end)
        {
            BulletUpdate();

            beamStart.transform.LookAt(beamEnd.transform.position);
            beamEnd.transform.LookAt(beamStart.transform.position);

            //float distance = Vector3.Distance(start, end);
            line.sharedMaterial.mainTextureScale = new Vector2(3, 1);
            line.sharedMaterial.mainTextureOffset -= new Vector2(Time.deltaTime * textureScrollSpeed, 0);
        }
    }
}