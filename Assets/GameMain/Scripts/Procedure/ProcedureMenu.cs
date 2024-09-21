//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework.Event;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace RoundHero
{
    public class ProcedureMenu : ProcedureBase
    {

        private bool InitSuccess = false;
        
        protected override void OnEnter(ProcedureOwner procedureOwner)
        { 
            base.OnEnter(procedureOwner);
            
            
            GameEntry.Event.Subscribe(GamePlayInitGameEventArgs.EventId, OnGamePlayInitGame);
            
            GameEntry.Sound.PlayMusic(0);

            InitSuccess = false;
            GameEntry.UI.OpenUIForm(UIFormId.LobbyForm, this);

            
            
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            GameEntry.Sound.StopMusic();

            base.OnLeave(procedureOwner, isShutdown);
            
            GameEntry.Event.Unsubscribe(GamePlayInitGameEventArgs.EventId, OnGamePlayInitGame);
        }

        public void OnGamePlayInitGame(object sender, GameEventArgs e)
        {
            var ne = e as GamePlayInitGameEventArgs;
            var data = new VarGamePlayInitData();
            data.SetValue(ne.GamePlayInitData);
            
            procedureOwner.SetData("GamePlayInitData", data);
            GamePlayManager.Instance.Init(ne.GamePlayInitData);
            
            if (data.Value.GameMode == EGamMode.PVE)
            {
                ChangeState<ProcedureGamePlay>(procedureOwner);
            }
            else if (data.Value.GameMode == EGamMode.PVP)
            {
                ChangeState<ProcedureBattle>(procedureOwner);
            }
            
            
            

        }


    }
}
