using System;
using System.Collections.Generic;
using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Transporting;
using GameFramework;
using HeathenEngineering.DEMO;
using HeathenEngineering.SteamworksIntegration;
using Steamworks;
using UnityEngine;
using UnityGameFramework.Runtime;
using Random = System.Random;

namespace RoundHero
{
    public struct BroadCast_PlayState : IBroadcast
    {
        public EPlayState PlayState;
        public BroadCast_PlayState(EPlayState playState)
        {
            PlayState = playState;
        }
    }
    
    public class LobbyForm : UGuiForm
    {
        private ProcedureMenu procedureMenu;
        public LobbyManager lobbyManager;
        public GameObject template;
        public Transform root;
        public GameObject startGame;

        private string hostAddressKey = "HostAddressKey";
        private List<Data_Player> playerDatas = new List<Data_Player>();
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            
            procedureMenu = (ProcedureMenu)userData;
            if (procedureMenu == null)
            {
                Log.Warning("ProcedureMenu is null.");
                return;
            }

            
            
            startGame.SetActive(false);
            
            InstanceFinder.NetworkManager.ServerManager.RegisterBroadcast<BroadCast_InitData>(OnServerBroadCast_InitData);
            InstanceFinder.NetworkManager.ClientManager.RegisterBroadcast<BroadCast_PlayState>(OnClientBroadCast_PlayState);
            InstanceFinder.NetworkManager.ClientManager.RegisterBroadcast<BroadCast_InitGame>(OnClientBroadCast_InitGame);
            InstanceFinder.NetworkManager.TransportManager.Transport.OnRemoteConnectionState += OnRemoteConnectionState;
            InstanceFinder.NetworkManager.TransportManager.Transport.OnServerConnectionState += OnServerConnectionState;
            InstanceFinder.NetworkManager.TransportManager.Transport.OnClientConnectionState += OnClientConnectionState;
            InstanceFinder.NetworkManager.TransportManager.Transport.OnClientReceivedData += OnClientReceivedData;
            InstanceFinder.NetworkManager.TransportManager.Transport.OnServerReceivedData += OnServerReceivedData;
            
            Refresh();

            
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            InstanceFinder.NetworkManager.ServerManager.UnregisterBroadcast<BroadCast_InitData>(OnServerBroadCast_InitData);
            InstanceFinder.NetworkManager.ClientManager.UnregisterBroadcast<BroadCast_PlayState>(OnClientBroadCast_PlayState);
            InstanceFinder.NetworkManager.ClientManager.UnregisterBroadcast<BroadCast_InitGame>(OnClientBroadCast_InitGame);
            InstanceFinder.NetworkManager.TransportManager.Transport.OnRemoteConnectionState -= OnRemoteConnectionState;
            InstanceFinder.NetworkManager.TransportManager.Transport.OnServerConnectionState -= OnServerConnectionState;
            InstanceFinder.NetworkManager.TransportManager.Transport.OnClientConnectionState -= OnClientConnectionState;
            InstanceFinder.NetworkManager.TransportManager.Transport.OnClientReceivedData -= OnClientReceivedData;
            InstanceFinder.NetworkManager.TransportManager.Transport.OnServerReceivedData -= OnServerReceivedData;
        }

        public void OnRemoteConnectionState(RemoteConnectionStateArgs remoteConnectionStateArgs)
        {
            Log.Debug("Remote:" + remoteConnectionStateArgs.ConnectionId + "-" + remoteConnectionStateArgs.ConnectionState);
            Log.Debug("Clients Count:" + InstanceFinder.NetworkManager.ServerManager.Clients.Count);

            if (InstanceFinder.NetworkManager.ServerManager.Clients.Count >= lobbyManager.Lobby.MaxMembers)
            {
                InstanceFinder.NetworkManager.ServerManager.Broadcast(new BroadCast_PlayState(EPlayState.SendInitData));
            }
        }
        
        public void OnServerConnectionState(ServerConnectionStateArgs serverConnectionStateArgs)
        {
            Log.Debug("Server:"+ serverConnectionStateArgs.ConnectionState);
        }
        
        public void OnClientConnectionState(ClientConnectionStateArgs clientConnectionStateArgs)
        {
            Log.Debug("Client:"+ clientConnectionStateArgs.ConnectionState);
        }

        public void OnClientReceivedData(ClientReceivedDataArgs clientReceivedDataArgs)
        {
            
        }
        
        public void OnServerReceivedData(ServerReceivedDataArgs serverRseceivedDataArgs)
        {
            SteamUser.GetSteamID();
        }

        private void OnServerBroadCast_InitData(NetworkConnection conn, BroadCast_InitData msg, Channel channel)
        {
            playerDatas.Add(msg.PlayerData);
            
            Log.Debug("OnServerBroadCast_InitData:" + conn.ClientId);
            Debug.Log("address:" + conn.GetAddress());
            
            if (playerDatas.Count >= InstanceFinder.ServerManager.Clients.Count)
            {
                InstanceFinder.NetworkManager.ServerManager.Broadcast(
                    new BroadCast_InitGame(playerDatas, UnityEngine.Random.Range(0, Constant.Game.RandomRange)));
            }
        }

        private void OnClientBroadCast_InitGame(BroadCast_InitGame msg, Channel channel)
        {
            foreach (var playerData in msg.PlayerDatas)
            {
                if (!GamePlayManager.Instance.GamePlayData.Contain(playerData.PlayerID))
                {
                    GamePlayManager.Instance.GamePlayData.AddPlayerData(playerData);
                }
            }

            GameEntry.UI.CloseUIForm(this);
            GameEntry.Event.Fire(null, GamePlayInitGameEventArgs.Create(msg.RandomSeed, msg.PlayerDatas));
        }


        private void OnClientBroadCast_PlayState(BroadCast_PlayState msg, Channel channel)
        {
            Log.Debug("OnClientBroadCast_PlayState:" + msg.PlayState);
            
            switch (msg.PlayState)
            {
                case EPlayState.SendInitData:
                    var playerData = PlayerManager.Instance.PlayerData.Copy();
                    if (playerData.PlayerID == lobbyManager.Lobby.Owner.user.id.m_SteamID)
                    {
                        playerData.UnitCamp = EUnitCamp.Player1;
                    }
                    else
                    {
                        playerData.UnitCamp = EUnitCamp.Player2;
                    }

                    if (playerData.PlayerID == lobbyManager.Lobby.Me.user.id.m_SteamID)
                    {
                        PlayerManager.Instance.PlayerData.UnitCamp = playerData.UnitCamp;
                    }

                    
                    InstanceFinder.NetworkManager.ClientManager.Broadcast(new BroadCast_InitData(playerData));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

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

            var hostAddressValue = SteamUser.GetSteamID().ToString();
            Debug.Log("hostAddressValue：" + hostAddressValue);
            lobbyManager.SetLobbyData(hostAddressKey, hostAddressValue);
            InstanceFinder.NetworkManager.TransportManager.Transport.SetClientAddress(hostAddressValue);

        }
        
        public void OnJoinedALobby(LobbyData lobbyData)
        {
            //string hostAddress = lobbyManager.GetLobbyData(hostAddressKey);
            //ClientConnection(hostAddress);
            //Debug.Log("OnJoinedALobby");

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

        public void OnGameCreated(LobbyGameServer lobbyGameServer)
        {
            ServerOrClientConnect();
        }

        public void PVPStartGame()
        {
            lobbyManager.Lobby.SetGameServer();
        }
        
        public void PVEStartGame()
        {
            GameEntry.UI.CloseUIForm(this);
            var randomSeed = UnityEngine.Random.Range(0, Constant.Game.RandomRange);
            randomSeed = 94204398;//2198030
            Log.Debug("randomSeed:" + randomSeed);
            GamePlayManager.Instance.GamePlayData.RandomSeed = randomSeed;
            // GameEntry.Event.Fire(null,
            //     GamePlayInitGameEventArgs.Create(randomSeed, EGameDifficulty.Difficulty1,
            //         GamePlayManager.Instance.GamePlayData.PVEType));
        }

        public void ServerOrClientConnect()  
        {
            string hostAddress = lobbyManager.GetLobbyData(hostAddressKey);
            if (lobbyManager.IsPlayerOwner)
            {
                ServerConnection();
            }

            ClientConnection(hostAddress);
        }
        
        // public void ClientConnect()
        // {
        //     string hostAddress = lobbyManager.GetLobbyData(hostAddressKey);
        //
        //     ClientConnection(hostAddress);
        // }

        public void ServerBroadCast()
        {
            var a = new ABroadCast();
            a.A = "Server HaHa";
            InstanceFinder.NetworkManager.ServerManager.Broadcast(a);
        }
        
        public void ClientBroadCast()
        {
            var a = new ABroadCast();
            a.A = "Client HaHa";
            InstanceFinder.NetworkManager.ClientManager.Broadcast(a);
        }

        private void Refresh()
        {
        }

        public void ContinueGame()
        {
        }

        public void OnLobbyDataUpdate(LobbyDataUpdateEventData lobbyDataUpdateEventData)
        {
            
            Debug.Log("OnLobbyDataUpdate");
            if (lobbyDataUpdateEventData.lobby.MemberCount >= lobbyDataUpdateEventData.lobby.MaxMembers && lobbyManager.AllPlayersReady && lobbyManager.IsPlayerOwner)
            {
                startGame.SetActive(true);
                // string hostAddress = lobbyManager.GetLobbyData(hostAddressKey);
                //
                // if (lobbyManager.Lobby.Me.IsOwner)
                // {
                //     ServerConnection();
                //     ClientConnection(hostAddress);
                // }
                // else
                // {
                //     ClientConnection(hostAddress);
                // }
            }
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