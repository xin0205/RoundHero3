using GameFramework.Event;
using UnityEngine;

namespace RoundHero
{
    public class TutorialForm : UGuiForm
    {

        private TutorialStepGODictionary TutorialStepGoDictionary = new TutorialStepGODictionary();
        
        // [SerializeField] private GameObject SelectUnitCardGO;
        // [SerializeField] private GameObject UseCardEnergyGO;
        // [SerializeField] private GameObject UnitOwnEnergyGO;

        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            GameEntry.Event.Subscribe(RefreshTutorialEventArgs.EventId, OnRefreshTutorial);

            TutorialStepGoDictionary.Clear();
            var tutorialItems = GetComponentsInChildren<TutorialItem>(true);

            foreach (var tutorialItem in tutorialItems)
            {
                TutorialStepGoDictionary.Add(tutorialItem.TutorialStep, tutorialItem.gameObject);
            }

        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            GameEntry.Event.Unsubscribe(RefreshTutorialEventArgs.EventId, OnRefreshTutorial);
        }
        
        private void OnRefreshTutorial(object sender, GameEventArgs e)
        {
            var ne = e as RefreshTutorialEventArgs;
            foreach (var kv in TutorialStepGoDictionary)
            {
                kv.Value.SetActive(false);
            }

            if (TutorialStepGoDictionary.ContainsKey(BattleManager.Instance.TutorialStep))
            {
                TutorialStepGoDictionary[BattleManager.Instance.TutorialStep].SetActive(true);
            }
            
            
        }
    }
}