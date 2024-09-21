using System.Collections;
using UnityEngine;

namespace RoundHero
{
    public class LayoutRebuilder : MonoBehaviour
    {
        [SerializeField] private GameObject go;
        private void Start()
        {
            //StartCoroutine(RebuildLayout());
        }
        
        private void OnEnable()
        {
            go.SetActive(true);
            StartCoroutine(RebuildLayout());
        }
        
        private IEnumerator RebuildLayout()
        {
            yield return new WaitForEndOfFrame();
            go.SetActive(false);
            StartCoroutine(RebuildLayout2());
            //UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
        }
        
        private IEnumerator RebuildLayout2()
        {
            yield return new WaitForEndOfFrame();
            go.SetActive(true);
        }
    }
}