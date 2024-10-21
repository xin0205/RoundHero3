using System;
using UnityEngine;

namespace RoundHero
{
    public class Ttest : MonoBehaviour
    {
        [SerializeField]
        private RectTransform rectTransform;

        private Vector2 size;

        private void Awake()
        {
            size = rectTransform.sizeDelta;
            
            
            
            size.x = 1200;
        }

        private void Update()
        {
            if (rectTransform.sizeDelta.x > 1200)
            {
                rectTransform.sizeDelta = size;
            }
        }
    }
}