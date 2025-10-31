#if HE_SYSCORE && (STEAMWORKSNET || FACEPUNCH) && !DISABLESTEAMWORKS 
using UnityEngine;
using HeathenEngineering.SteamworksIntegration;
using System;
#if ENABLE_INPUT_SYSTEM
#endif

namespace HeathenEngineering.DEMO
{
    [System.Obsolete("This script is for demonstration purposes ONLY")]
    public class LobbyBrowser_LobbyRecord : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI nameOrID;
        public TMPro.TextMeshProUGUI buttonLabel;
        
        public TMPro.TextMeshProUGUI ready;

        private LobbyData target;
        private LobbyManager lobbyManager;

        private void OnDestroy()
        {
            if (lobbyManager != null)
            {
                lobbyManager.evtEnterSuccess.RemoveListener(HandleLobbyEnter);
                lobbyManager.evtLeave.RemoveListener(HandleLobbyLeave);
                lobbyManager.evtUserJoined.RemoveListener(HandleUserJoined);
                lobbyManager.evtUserLeft.RemoveListener(HandleUserLeft);
                lobbyManager.evtDataUpdated.RemoveListener(HandleUpdateLobbyData);
            }
            
            
        }

        public void SetLobby(LobbyData lobby, LobbyManager parent)
        {
            target = lobby;
            lobbyManager = parent;

            if (lobbyManager.Lobby == target)
                buttonLabel.text = "Leave";
            else
                buttonLabel.text = "Join";

            RefreshLobbyData(target);

            parent.evtEnterSuccess.AddListener(HandleLobbyEnter);
            parent.evtLeave.AddListener(HandleLobbyLeave);
            parent.evtUserJoined.AddListener(HandleUserJoined);
            parent.evtUserLeft.AddListener(HandleUserLeft);
            parent.evtDataUpdated.AddListener(HandleUpdateLobbyData);
        }
        
        private void HandleUserJoined(UserData userData)
        {
            RefreshLobbyData(target);
        }
        
        private void HandleUserLeft(UserLobbyLeaveData userLobbyLeaveData)
        {
            RefreshLobbyData(target);
        }

        private void RefreshLobbyData(LobbyData lobby)
        {
            nameOrID.text = lobby.Name + "(" + lobby.MemberCount + "/" + lobby.MaxMembers + ")";
            if(string.IsNullOrEmpty(nameOrID.text))
                nameOrID.text = lobby.ToString();
            
            ready.text = lobbyManager.IsPlayerReady ? "Ready" : "UnReady";
        }

        private void HandleLobbyLeave()
        {
            if (lobbyManager.Lobby == target)
                buttonLabel.text = "Leave";
            else
                buttonLabel.text = "Join";

            RefreshLobbyData(target);
        }

        private void HandleLobbyEnter(LobbyData arg0)
        {
            if (target.IsAMember(UserData.Me))
                buttonLabel.text = "Leave";
            else
                buttonLabel.text = "Join";

            RefreshLobbyData(target);
        }

        public void JoinOrExit()
        {
            if (lobbyManager.Lobby == target)
                lobbyManager.Leave();
            else
                lobbyManager.Join(target);
        }
        
        public void ReadyOrUnReady()
        {
            lobbyManager.IsPlayerReady = !lobbyManager.IsPlayerReady;
            RefreshLobbyData(target);
        }

        public void HandleUpdateLobbyData(LobbyDataUpdateEventData lobbyDataUpdateEventData)
        {
            RefreshLobbyData(target);
        }
    }
}
#endif
