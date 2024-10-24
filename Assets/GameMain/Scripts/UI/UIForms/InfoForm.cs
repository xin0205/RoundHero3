
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    [Serializable]
    public class InfoFormParams
    {
        public string Desc;
        public string Name;
        public Vector2 Position;
    }

    public class InfoForm : UGuiForm
    {
        [SerializeField] private Text desc;
        [SerializeField] private Text name;
        [SerializeField] private GameObject root;
        private InfoFormParams infoFormParams;


        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            infoFormParams = userData as InfoFormParams;
            
            name.gameObject.SetActive(!string.IsNullOrEmpty(infoFormParams.Name));
            desc.gameObject.SetActive(!string.IsNullOrEmpty(infoFormParams.Desc));
            
            name.text = infoFormParams.Name;
            desc.text = infoFormParams.Desc;

            root.transform.position = infoFormParams.Position;

        }
    }

}