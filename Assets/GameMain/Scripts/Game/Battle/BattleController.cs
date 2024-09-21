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

    }
}