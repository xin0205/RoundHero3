using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoundHero
{
    public class BattleController : TMonoSingleton<BattleController>
    {
        [SerializeField] public Transform Root;
        
        [SerializeField] public Transform StandByCardPos;
        [SerializeField] public Transform PassCardPos;
        [SerializeField] public Transform HandCardPos;
        [SerializeField] public Transform CenterPos;
        [SerializeField] public Transform ConsumeCardPos;

        public void Awake()
        {
            Constant.Battle.CardPos = new Dictionary<ECardPos, Vector3>()
            {
                [ECardPos.Center] = BattleController.Instance.CenterPos.localPosition,
                [ECardPos.Pass] = BattleController.Instance.PassCardPos.localPosition,
                [ECardPos.StandBy] = BattleController.Instance.StandByCardPos.localPosition,
                [ECardPos.Hand] = BattleController.Instance.HandCardPos.localPosition,
                [ECardPos.Consume] = BattleController.Instance.ConsumeCardPos.localPosition,
            };
        }
    }
}