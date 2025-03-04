﻿using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleFlyDirectEntity : Entity
    {
        public BattleFlyDirectEntityData BattleFlyDirectEntityData { get; protected set; }

        [SerializeField]
        private MoveDirectGODictionary MoveDirectGODict;
        
        
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            BattleFlyDirectEntityData = userData as BattleFlyDirectEntityData;
            if (BattleFlyDirectEntityData == null)
            {
                Log.Error("Error MoveDirectEntityData");
                return;
            }

            foreach (var kv in MoveDirectGODict)
            {
                MoveDirectGODict[kv.Key]
                    .SetActive(kv.Key == BattleFlyDirectEntityData.Direct);

            }
        }
    }
}