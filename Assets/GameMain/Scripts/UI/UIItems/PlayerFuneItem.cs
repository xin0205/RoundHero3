using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public class PlayerFuneItem : MonoBehaviour
    {
        public int cardIdx;
        public int funeIdx;
        [SerializeField] public Image funeIcon;
        [SerializeField] public GameObject unEquipGO;
        [SerializeField] public InfoTrigger funeInfoTrigger;
        public async void SetFune(int cardIdx, int funeIdx)
        {
            this.cardIdx = cardIdx;
            this.funeIdx = funeIdx;
            if (funeIdx == -1)
            {
                funeIcon.gameObject.SetActive(false);
                unEquipGO.SetActive(false);
            }
            else
            {
                var funeData = FuneManager.Instance.GetFuneData(funeIdx);
                funeIcon.gameObject.SetActive(true);
                funeIcon.overrideSprite = await AssetUtility.GetFuneIcon(funeData.FuneID);

                string funeName = "";
                string funeDesc = "";
                GameUtility.GetFuneText(funeData.FuneID, ref funeName, ref funeDesc);
                
                funeInfoTrigger.SetNameDesc(funeName, funeDesc);
            }
            
            
            
            
            
        }

        public void ShowUnEquip(bool isShow)
        {
            unEquipGO.SetActive(isShow && funeIdx != -1);
        }
        
        public void UnEquipFune()
        {
            var cardData = CardManager.Instance.GetCard(cardIdx);
            //var funeIdx = cardData.FuneIdxs[funeOrder];
            // if(!GameManager.Instance.CardsForm_EquipFuneIdxs.Contains(funeIdx))
            //     return;
            // GameManager.Instance.CardsForm_EquipFuneIdxs.Remove(funeIdx);
            
            BattlePlayerManager.Instance.PlayerData.UnusedFuneIdxs.Add(funeIdx);
            
            cardData.FuneIdxs.Remove(funeIdx);
            //cardData.FuneIdxs.RemoveAt(funeOrder);
            //PlayerFuneItems[funeOrder].ShowUnEquip(false);
            GameEntry.Event.Fire(null, RefreshCardsFormEventArgs.Create());
            GameEntry.Event.Fire(null, UnEquipFuneEventArgs.Create(funeIdx));
        }
    }
}