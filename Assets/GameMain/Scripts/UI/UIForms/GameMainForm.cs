using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Transporting;
using HeathenEngineering.DEMO;
using HeathenEngineering.SteamworksIntegration;
using Steamworks;
using UnityEngine;
using UnityGameFramework.Runtime;


namespace RoundHero
{
    
    public struct ABroadCast : IBroadcast
    {
        public string A;
    }
    
    public class GameMainForm : UGuiForm
    {
        private ProcedureMenu procedureMenu;
        public LobbyManager lobbyManager;
        public GameObject template;
        public Transform root;

        private string hostAddressKey = "HostAddressKey";
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            
            procedureMenu = (ProcedureMenu)userData;
            if (procedureMenu == null)
            {
                Log.Warning("ProcedureMenu is null.");
                return;
            }

            Refresh();
            InstanceFinder.NetworkManager.ServerManager.RegisterBroadcast<ABroadCast>(OnABroadCast);
            

        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            InstanceFinder.NetworkManager.ServerManager.UnregisterBroadcast<ABroadCast>(OnABroadCast);
        }

        private void OnABroadCast(NetworkConnection conn, ABroadCast msg, Channel channel)
        {
            Debug.Log(msg.A);
        }
        
        public void LobbyResults(LobbyData[] results)
        {
            //First we clean out the old records
            foreach (Transform tran in root)
                Destroy(tran.gameObject);

            //Next we spawn new records for each lobby
            foreach(var lobby in results)
            {
                var GO = Instantiate(template, root);
                var com = GO.GetComponent<LobbyBrowser_LobbyRecord>();
                com.SetLobby(lobby, lobbyManager);
            }
        }

        
        public void OnLobbyCreated(LobbyData lobby)
        {
            Debug.Log("OnLobbyCreated");

            //Hide the proccessing animation on the PlayButton
            
            var hostAddressValue = SteamUser.GetSteamID().ToString();
            Debug.Log("hostAddressValue：" + hostAddressValue);
            //LobbyManager.SetLobbyData(hostAddressKey, hostAddressValue);
            InstanceFinder.NetworkManager.TransportManager.Transport.SetClientAddress(hostAddressValue);
            
            //ServerConnection();
        }
        
        public void OnJoinedALobby(LobbyData lobbyData)
        {
            string hostAddress = lobbyManager.GetLobbyData(hostAddressKey);
            ClientConnection(hostAddress);
            Debug.Log("OnJoinedALobby");
                
        }
        
        public void ClientConnection(string hostAddress)
        {
            Debug.Log("Connecting to: "+hostAddress);
            InstanceFinder.NetworkManager.ClientManager.StartConnection(hostAddress);
            
        }

        public void ServerConnection()
        {
            Debug.Log("ServerConnection");
            
            
            InstanceFinder.NetworkManager.ServerManager.StartConnection();
        }

        public void ServerBroadCast()
        {
            var a = new ABroadCast();
            a.A = "HaHa";
            InstanceFinder.NetworkManager.ClientManager.Broadcast(a);
        }

        

        private void Refresh()
        {
           
        }

        public void ContinueGame()
        {
            

        }
        

        public void RestartGame()
        {
            // GameUtility.RestartGame(() =>
            // {
            //     DataManager.Instance.CurUser.Reset();
            //     Refresh();
            //     GameEntry.UI.OpenUIForm(UIFormId.HeroSelectForm, procedureMenu);
            // });
            
        }
        
        public void NewGame()
        {
            GameEntry.UI.OpenUIForm(UIFormId.HeroSelectForm, procedureMenu);
            
        }
        
        public void Setting()
        {
            GameEntry.UI.OpenUIForm(UIFormId.SettingForm);
            
        }
        
        public void Bless()
        {
            GameEntry.UI.OpenUIForm(UIFormId.BlessForm, procedureMenu);
            
        }
        
        public void QuitGame()
        {
            Application.Quit();
            
        }
        

        
    }
}