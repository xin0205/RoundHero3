using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Transporting;
using UnityGameFramework.Runtime;
using GameFramework.Event;
using Steamworks;
using UnityEngine;
using Random = System.Random;

namespace RoundHero
{
    public struct BroadCast_InitData : IBroadcast
    {
        public Data_Player PlayerData;
        public BroadCast_InitData(Data_Player playerData)
        {
            PlayerData = playerData;
        }
    }
    
    public struct BroadCast_InitGame : IBroadcast
    {
        public List<Data_Player> PlayerDatas;
        public int RandomSeed;
     
        public BroadCast_InitGame(List<Data_Player> playerDatas, int randomSeed)
        {
            PlayerDatas = playerDatas;
            RandomSeed = randomSeed;
        }
    }

    public enum EPlayState
    {
        SendInitData,
        InitSuccess,
        EndRound,
        EndAction,
        EndTurn,
        UseCard,
        StartRound,
    }

    public struct BroadCast_UseCard : IBroadcast
    {
        public EUnitCamp PlayerUnitCamp;
        public EUnitCamp CardUnitCamp;
        public int CardID;
        public int PlaceGridPosIdx;

    }
    
    public class PVPManager : Singleton<PVPManager>, IBattleTypeManager
    {
        public Random Random;
        private int randomSeed;
        public List<Data_Player> Players = new List<Data_Player>();
        public Data_Player PlayerData;
        
        public EBattleState BattleState { get; set; }

        private List<string> clientBroadCasts = new List<string>();

        
        public Data_GamePlay GamePlayData => DataManager.Instance.DataGame.User.CurGamePlayData;
        public Data_Battle BattleData => GamePlayData.BattleData;

        public void ResetClientBroadCasts()
        {
            clientBroadCasts.Clear();
        }

        private void GetPlayerData()
        {
            foreach (var playerData in Players)
            {
                if (playerData.PlayerID == SteamUser.GetSteamID().m_SteamID)
                {
                    PlayerData = playerData;
                }
            }
             
        }
        
        public void Init(int randomSeed, List<Data_Player> playerDatas)
        {
            this.randomSeed = randomSeed;
            Random = new Random(randomSeed);
            
            BattleManager.Instance.SetBattleTypeManager(this);
            
            GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
            GameEntry.Event.Subscribe(RefreshBattleStateEventArgs.EventId, OnRefreshBattleState);
            InstanceFinder.NetworkManager.ClientManager.RegisterBroadcast<BroadCast_PlayState>(OnClientBroadCast_PlayState);
            InstanceFinder.NetworkManager.ServerManager.RegisterBroadcast<BroadCast_PlayState>(OnServerBroadCast_PlayState);
            InstanceFinder.NetworkManager.ClientManager.RegisterBroadcast<BroadCast_UseCard>(OnClientBroadCast_UseCard);
            InstanceFinder.NetworkManager.ServerManager.RegisterBroadcast<BroadCast_UseCard>(OnServerBroadCast_UseCard);

            GamePlayData.GameMode = EGamMode.PVP;
            
            Players.Clear();
            foreach (var playerData in playerDatas)
            {
                Players.Add(playerData);
            }

            GetPlayerData();

            BattleManager.Instance.Init(randomSeed);
            
            // var randoms = MathUtility.GetRandomNum(1, 0,
            //     Constant.Game.RandomRange, Random);
            SetCurPlayer(EUnitCamp.Player1);
            BattleCardManager.Instance.InitCards();
            SetCurPlayer(EUnitCamp.Player2);
            BattleCardManager.Instance.InitCards();
            
            SetCurPlayer(EUnitCamp.Player1);
            GameEntry.UI.OpenUIForm(UIFormId.BattleForm, this);
            
            
            
        }


        public void SetCurPlayer(EUnitCamp unitCamp)
        {
            BattleManager.Instance.SetCurPlayer(unitCamp);
        }
        
        public void Destory()
        {
            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
            GameEntry.Event.Unsubscribe(RefreshBattleStateEventArgs.EventId, OnRefreshBattleState);
            InstanceFinder.NetworkManager.ClientManager.UnregisterBroadcast<BroadCast_PlayState>(OnClientBroadCast_PlayState);
            InstanceFinder.NetworkManager.ServerManager.UnregisterBroadcast<BroadCast_PlayState>(OnServerBroadCast_PlayState);
            InstanceFinder.NetworkManager.ClientManager.UnregisterBroadcast<BroadCast_UseCard>(OnClientBroadCast_UseCard);
            InstanceFinder.NetworkManager.ServerManager.UnregisterBroadcast<BroadCast_UseCard>(OnServerBroadCast_UseCard);
            BattleManager.Instance.Destory();
                
        }
        
        private void OnClientBroadCast_PlayState(BroadCast_PlayState msg, Channel channel)
        {
            Log.Debug("OnClientBroadCast_PlayState:" + msg.PlayState);
            switch (msg.PlayState)
            {
                case EPlayState.StartRound:
                    RoundStartTrigger();
                    break;
                case EPlayState.EndAction:
                    TriggerEndAction();
                    break;
                case EPlayState.EndTurn:
                    SwitchTurn();
                    GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
                    GameEntry.Event.Fire(null, RefreshUnitDataEventArgs.Create());
                    break;
                case EPlayState.EndRound:
                    
                    BattleManager.Instance.RoundEndTrigger();
                    SwitchTurn();
                
                    GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
                    GameEntry.Event.Fire(null, RefreshUnitDataEventArgs.Create());
                    break;

            }

        }
        
        private void OnServerBroadCast_PlayState(NetworkConnection conn, BroadCast_PlayState msg, Channel channel)
        {
            Log.Debug("OnServerBroadCast_PlayState:" + msg.PlayState);
            Debug.Log("address:" + conn.GetAddress());
            switch (msg.PlayState)
            {
                case EPlayState.EndAction:
                    InstanceFinder.NetworkManager.ServerManager.Broadcast(new BroadCast_PlayState(EPlayState.EndAction));
                    break;
                case EPlayState.InitSuccess:
                    
                    clientBroadCasts.Add(conn.GetAddress());
                    if (clientBroadCasts.Count >= InstanceFinder.NetworkManager.ServerManager.Clients.Count)
                    {
                        ResetClientBroadCasts();
                        InstanceFinder.NetworkManager.ServerManager.Broadcast(new BroadCast_PlayState(EPlayState.StartRound));
                    }
                    break;
                case EPlayState.EndTurn:
                    clientBroadCasts.Add(conn.GetAddress());
                    if (clientBroadCasts.Count >= InstanceFinder.NetworkManager.ServerManager.Clients.Count)
                    {
                        ResetClientBroadCasts();
                        InstanceFinder.NetworkManager.ServerManager.Broadcast(new BroadCast_PlayState(EPlayState.EndTurn));
                    }
                    break;
                case EPlayState.EndRound:
                    clientBroadCasts.Add(conn.GetAddress());
                    if (clientBroadCasts.Count >= InstanceFinder.NetworkManager.ServerManager.Clients.Count)
                    {
                        ResetClientBroadCasts();
                        InstanceFinder.NetworkManager.ServerManager.Broadcast(new BroadCast_PlayState(EPlayState.EndRound));
                    }
                    break;       
                
            }

        }
        
        private void OnRefreshBattleState(object sender, GameEventArgs e)
        {
            var ne = e as RefreshBattleStateEventArgs;
            BattleState = ne.BattleState;

            if (BattleState == EBattleState.EndRound)
            {
                InstanceFinder.NetworkManager.ClientManager.Broadcast(new BroadCast_PlayState(EPlayState.EndRound));

                
            }
            else if (BattleState == EBattleState.EndTurn)
            {
                InstanceFinder.NetworkManager.ClientManager.Broadcast(new BroadCast_PlayState(EPlayState.EndTurn));
                
            }
        }
        
        private async void OnOpenUIFormSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
            if (ne.UIForm.Logic is BattleForm)
            {
                await BattleAreaManager.Instance.InitArea();
                SetCurPlayer(EUnitCamp.Player1);
                await HeroManager.Instance.GenerateHero();
                SetCurPlayer(EUnitCamp.Player2);
                await HeroManager.Instance.GenerateHero();
                
                SetCurPlayer(EUnitCamp.Player1);
                
                //await BattleGridPropManager.Instance.GenerateMoveDirect();

                BattleManager.Instance.RoundStartTrigger();
                
                BattleManager.Instance.RefreshEnemyAttackData();
                
                InstanceFinder.NetworkManager.ClientManager.Broadcast(new BroadCast_PlayState(EPlayState.InitSuccess));
            }

        }

        public void RoundStartTrigger()
        {
            BattleState = EBattleState.UseCard;
            BattleCardManager.Instance.RoundAcquireCards(true);
            
            BattleManager.Instance.RoundStartTrigger();
            
            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
        }
        
        public void SwitchTurn()
        {
            BattleState = EBattleState.UseCard;
            BattleCardManager.Instance.RoundAcquireCards(false);
            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
            
        }

        public void StartAction()
        {
            if (BattleState != EBattleState.UseCard)
            {
                return;
            }
            
            BlessManager.Instance.StartActionTrigger();
            StartUnitAction();
            
            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
            

        }
        
        public void StartUnitAction()
        {
            BattleFightManager.Instance.AcitonUnitIdx = 0;
            BattleManager.Instance.SetBattleState(EBattleState.PlayerAction);
            BattleFightManager.Instance.ActionProgress = EActionProgress.ActionStart;
            ContinueAction();

        }

        public void ContinueAction()
        {
            if (BattleFightManager.Instance.ActionProgress == EActionProgress.ActionStart)
            {
                //FightManager.Instance.RoundStartTrigger();
            }
            
            if (BattleFightManager.Instance.ActionProgress == EActionProgress.ThirdUnitAttack)
            {
                BattleFightManager.Instance.ThirdUnitAttack();
            }
            
            if (BattleFightManager.Instance.ActionProgress == EActionProgress.SoliderAttack)
            {
                BattleFightManager.Instance.SoliderAttack();
            }
            

            if (BattleFightManager.Instance.ActionProgress == EActionProgress.ActionEnd)
            {
                
                GameEntry.Event.Fire(null, RefreshBattleStateEventArgs.Create(BattleManager.Instance.BattleState));
            }
        }
        
        public void NextAction()
        {
            if (BattleFightManager.Instance.ActionProgress == EActionProgress.ActionStart)
            {
                BattleFightManager.Instance.ActionProgress = EActionProgress.ThirdUnitAttack;
            }
            
            else if (BattleFightManager.Instance.ActionProgress == EActionProgress.ThirdUnitAttack)
            {
                BattleFightManager.Instance.ActionProgress = EActionProgress.SoliderAttack;
            }
            
            else if (BattleFightManager.Instance.ActionProgress == EActionProgress.SoliderAttack)
            {
                if (BattleManager.Instance.CurUnitCamp == EUnitCamp.Player1)
                {
                    BattleManager.Instance.CurUnitCamp = EUnitCamp.Player2;
                    BattleManager.Instance.SetBattleState(EBattleState.EndTurn);
                    BattleFightManager.Instance.ActionProgress = EActionProgress.ActionEnd;
                    
                }
                else if (BattleManager.Instance.CurUnitCamp == EUnitCamp.Player2)
                {
                    BattleManager.Instance.CurUnitCamp = EUnitCamp.Player1;
                    BattleManager.Instance.SetBattleState(EBattleState.EndRound);
                    BattleFightManager.Instance.ActionProgress = EActionProgress.ActionEnd;
                }
                
                
            }
            else
            {
                BattleFightManager.Instance.ActionProgress = EActionProgress.ActionEnd;
                
            }

        }
        
        public void EndAction()
        {
            InstanceFinder.NetworkManager.ClientManager.Broadcast(new BroadCast_PlayState(EPlayState.EndAction));
        }
        
        public void TriggerEndAction()
        {
            if(BattleState != EBattleState.UseCard)
                return;
            
            StartAction();
        
            if (GamePlayData.BlessCount(EBlessID.UnPassCards, BattleManager.Instance.CurUnitCamp) <= 0)
            {
                BattleCardManager.Instance.PassCards();
            }
            
            
            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
        }
        
        private void OnServerBroadCast_UseCard(NetworkConnection conn, BroadCast_UseCard msg, Channel channel)
        {
            InstanceFinder.NetworkManager.ServerManager.Broadcast(
                msg);
            
        }
        
        private void OnClientBroadCast_UseCard(BroadCast_UseCard msg, Channel channel)
        {
            BattleAreaManager.Instance.PlaceUnitCard(msg.CardID, msg.PlaceGridPosIdx, msg.PlayerUnitCamp);
        }

        // public void UseCard(int cardID, int posIdx)
        // {
        //     var cardData = GetCard(cardID);
        //
        //     if (cardData == null)
        //         return;
        //     
        //     if (!Input.GetMouseButtonUp(0))
        //     {
        //         return;
        //     }
        //     
        //     if(BattleManager.Instance.BattleState != EBattleState.UseCard)
        //         return;
        //
        //     
        //     if(cardData.UnUse)
        //         return;
        //
        //     
        //     if(!BattleCardManager.Instance.PreUseCard(cardID))
        //         return;
        // }

        public Data_Card GetCard(int cardIdx)
        {
            foreach (var playerData in GamePlayManager.Instance.GamePlayData.PlayerDatas)
            {
                foreach (var kv2 in playerData.CardDatas)
                {
                    if (kv2.Value.CardIdx == cardIdx)
                        return kv2.Value;
                }
            }

            return null;

        }
        
        public void PlaceUnitCard(int cardID, int gridPosIdx, EUnitCamp playerUnitCamp)
        {
            InstanceFinder.NetworkManager.ClientManager.Broadcast(new BroadCast_UseCard()
            {
                CardID = cardID,
                PlaceGridPosIdx = gridPosIdx,
                PlayerUnitCamp = playerUnitCamp,
            });
        }
        
        public void PlaceProp(int propID, int gridPosIdx, EUnitCamp playerUnitCamp)
        {
            // InstanceFinder.NetworkManager.ClientManager.Broadcast(new BroadCast_UseCard()
            // {
            //     CardID = cardID,
            //     PlaceGridPosIdx = gridPosIdx,
            //     PlayerUnitCamp = playerUnitCamp,
            // });
        }
        
        public Data_EnergyBuff GetEnergyBuff(EUnitCamp unitCamp, int heart, int hp)
        {
            return BattleEnergyBuffManager.Instance.GetEnergyBuff(unitCamp, heart, hp);

        }

        public void ShowGameOver()
        {
            
        }
    }
}