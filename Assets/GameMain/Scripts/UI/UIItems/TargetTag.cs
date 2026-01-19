using UnityEngine;

namespace RoundHero
{
    public class TargetTag : MonoBehaviour
    {
        public void SetData(Vector2 pos)
        {
            transform.SetParent(AreaController.Instance.BattleFormRoot.transform);
            transform.localScale = Vector3.one; 

            transform.localPosition = pos;
        }
    }
}