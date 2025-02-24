
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;

namespace RoundHero
{
    public class GamePlayInitData
    {
        public int RandomSeed;
        public EGamMode GameMode;
        public List<Data_Player> PlayerDatas;
        public EEnemyType EnemyType;
        public EGameDifficulty GameDifficulty;

        public GamePlayInitData(int randomSeed, List<Data_Player> playerDatas)
        {
            RandomSeed = randomSeed;
            GameMode = EGamMode.PVP;
            PlayerDatas = playerDatas;
        }
        
        public GamePlayInitData(int randomSeed, EGameDifficulty gameDifficulty)
        {
            RandomSeed = randomSeed;
            GameMode = EGamMode.PVE;
            GameDifficulty = gameDifficulty;
        }
    }
    
    public class VarGamePlayInitData : Variable<GamePlayInitData>
    {
        public VarGamePlayInitData()
        {
        }

        
        public static implicit operator VarGamePlayInitData(GamePlayInitData value)
        {
            VarGamePlayInitData varValue = ReferencePool.Acquire<VarGamePlayInitData>();
            varValue.Value = value;
            return varValue;
        }

       
        public static implicit operator GamePlayInitData(VarGamePlayInitData value)
        {
            return value.Value;
        }
    }
    public class GamePlayInitGameEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(GamePlayInitGameEventArgs).GetHashCode();
        public override int Id => EventId;

        
        public GamePlayInitData GamePlayInitData;
        
        public static GamePlayInitGameEventArgs Create(int randomSeed, List<Data_Player> playerDatas)
        {
            var args = ReferencePool.Acquire<GamePlayInitGameEventArgs>();
            args.GamePlayInitData = new GamePlayInitData(randomSeed, playerDatas);
            return args;
        }
        
        public static GamePlayInitGameEventArgs Create(int randomSeed, EGameDifficulty gameDifficulty)
        {
            var args = ReferencePool.Acquire<GamePlayInitGameEventArgs>();
            args.GamePlayInitData = new GamePlayInitData(randomSeed, gameDifficulty);
            return args;
        }
        
        public override void Clear()
        {

        }
    }
}