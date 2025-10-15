
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
        //public Vector2 Position;
        public EShowPosition ShowPosition = EShowPosition.MousePosition;
    }

    public class InfoForm : UGuiForm
    {
        [SerializeField] private Text desc;
        [SerializeField] private Text name;
        [SerializeField] private GameObject root;
        private InfoFormParams infoFormParams;
        [SerializeField] private RectTransform bg;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            infoFormParams = userData as InfoFormParams;

            SetText(infoFormParams);


        }

        public void SetText(InfoFormParams infoFormParams)
        {
            this.infoFormParams = infoFormParams;
            
            name.gameObject.SetActive(!string.IsNullOrEmpty(infoFormParams.Name));
            desc.gameObject.SetActive(!string.IsNullOrEmpty(infoFormParams.Desc));
            
            name.text = infoFormParams.Name;
            desc.text = infoFormParams.Desc;

            //root.SetActive(false);
            GameUtility.DelayExcute(0.05f, () =>
            {

                Vector3 mousePosition = Input.mousePosition;

                if (infoFormParams.ShowPosition == EShowPosition.MousePosition)
                {
                    var pos = AreaController.Instance.UICamera.ScreenToWorldPoint(new Vector3(mousePosition.x,
                        mousePosition.y, mousePosition.z));
                    var delta = new Vector2(0.3f + bg.rect.width / 200f, 0.15f + bg.rect.height / 200f);
                    if (mousePosition.x < Screen.width / 2)
                    {
                        pos.x += delta.x;
                        if (mousePosition.y < Screen.height / 2)
                        {
                            pos.y += delta.y;
                        }
                        else
                        {
                            pos.y -= delta.y;
                        }
                    }
                    else
                    {
                        pos.x -= delta.x;
                        if (mousePosition.y < Screen.height / 2)
                        {
                            pos.y += delta.y;
                        }
                        else
                        {
                            pos.y -= delta.y;
                        }
                    }

                    //root.SetActive(true);
                    root.transform.position = pos;
                }
            });

            //root.transform.localPosition = infoFormParams.Position;

        }
    }

}