#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using HeathenEngineering.SteamworksIntegration;
using System;
using UnityEngine;
using CloudSave = HeathenEngineering.SteamworksIntegration.API.RemoteStorage.Client;

namespace HeathenEngineering.DEMO
{
    public class DemoDataDisplayRecord : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TextMeshProUGUI title;

        private RemoteStorageFile record;
        private Action<RemoteStorageFile> callback;

        public void Initialize(RemoteStorageFile file, Action<RemoteStorageFile> loadCallback)
        {
            record = file;
            title.text = record.name;
            callback = loadCallback;
        }

        public void Delete() => CloudSave.FileDelete(record.name);

        public void Load() => callback?.Invoke(record);
    }
}
#endif