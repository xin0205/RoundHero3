#if HE_SYSCORE && (STEAMWORKSNET || FACEPUNCH) && !DISABLESTEAMWORKS 
using UnityEngine;
using UGC = HeathenEngineering.SteamworksIntegration.API.UserGeneratedContent.Client;
using HeathenEngineering.SteamworksIntegration;
using System.Collections.Generic;
using System;
using System.Text;

namespace HeathenEngineering.DEMO
{
    /// <summary>
    /// This is for demonstration purposes only
    /// </summary>
    [System.Obsolete("This script is for demonstration purposes ONLY")]
    public class Scene7Behaviour : MonoBehaviour
    {
        public UserGeneratedContentQueryManager queryManager;
        public UnityEngine.UI.Text pageCount;
        public UnityEngine.UI.InputField searchInput;
        public GameObject recordTemplate;
        public Transform contentRoot;

        private List<GameObject> currentRecords = new List<GameObject>();

        public void QueryItems()
        {
            queryManager.SearchAll(searchInput.text);
        }

        public void UpdateResults(List<WorkshopItem> results)
        {
            pageCount.text = queryManager.activeQuery.Page.ToString() + " of " + queryManager.activeQuery.pageCount.ToString();

            while(currentRecords.Count > 0)
            {
                var target = currentRecords[0];
                currentRecords.Remove(target);
                Destroy(target);
            }

            foreach(var result in results)
            {
                var go = Instantiate(recordTemplate, contentRoot);
                currentRecords.Add(go);
                var comp = go.GetComponent<Scene7DisplayItem>();
                comp.AssignResult(result);
            }
        }

        public void ListSubscribedItems()
        {
            var items = UGC.GetSubscribedItems();
            if(items != null)
            {
                Debug.Log("Found " + items.Length + " items.");
            }
            else
                Debug.Log("Found 0 items.");
        }

        [ContextMenu("Create Item Example")]
        public void CreatItemExample()
        {
            var itemData = new WorkshopItemData
            {
                title = "A new title",
                description = "A new description",
                preview = new System.IO.FileInfo("D:\\UGCTest\\testPreviewImage.jpg"),
                content = new System.IO.DirectoryInfo("D:\\UGCTest\\testUGCFolder"),
                appId = AppData.Me,
                visibility = Steamworks.ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityUnlisted
            };
            itemData.Create(completedCallback =>
            {
                if(completedCallback.hasError)
                {
                    if(completedCallback.submitItemUpdateResult.HasValue)
                    {
                        Debug.Log("Completed Callback was invoked\nError on Item Update: " + completedCallback.errorMessage);
                    }
                    else if (completedCallback.createItemResult.HasValue) 
                    {
                        Debug.Log("Completed Callback was invoked\nError on Item Create: " + completedCallback.errorMessage);
                    }
                    else
                    {
                        Debug.Log("Completed Callback was invoked\nIO Error: " + completedCallback.errorMessage);
                    }
                }
                else
                {
                    Debug.Log("Completed Callback was invoked");
                }
            },
            uploadStartedCallback =>
            {
                Debug.Log("Upload Started Callback was invoked");
            },
            fileCreatedCallback =>
            {
                Debug.Log("File Created Callback was invoked");
            });
        }
    }
}
#endif