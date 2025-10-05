using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoundHero
{
    // public class UnitActionType
    // {
    //     public 
    // }
    
    public static partial class Constant
    {
        public static class Tutorial
        {
            public static List<int> Obstacles = new List<int>()
            {
                8, 17, 26, 30, 39,
            };
            
            public static List<int> Cores = new List<int>()
            {
                11, 24, 38,
            };

            public static int RandomSeed = 42536450;//26987145;
            public static int UseUnitCardGridPosIdx = 18;
            public static int MoveGridPosIdx = 15;
            public static float StepInterval = 0.5f;

            public static List<int> Cards = new List<int>()
            {
                0, 0, 0, 1, 1, 1, 2, 2, 2, 10000, 10000, 10000
            };

            // public static EnemyGenrateRule EnemyGenrateRule = new EnemyGenrateRule()
            // {
            //     RoundGenerateUnitCount = new Dictionary<int, int>()
            //     {
            //         [0] = 2,
            //     },
            //     EachRoundUnitCount = 2,
            //     // NormalUnitCount = 2,
            //     // EliteUnitCount = 0,
            //     // NormalUnitTypeCount = 1,
            //     // EliteUnitTypeCount = 0,
            //     GlobalDebuffCount = 0,
            // };

        }
        
        public static class Area
        {
            public static Vector2Int GridSize = new Vector2Int(7, 7);
            public static int ObstacleCount = 0;
            public static int MaxRoleInGrid = 1;

            public static Vector2 GridInterval = new Vector2()
            {
                x = 0.05f,
                y = 0.05f,
            };

            public static Vector2 GridLength = new Vector2()
            {
                x = 1.65f,
                y = 1.65f,
            };

            public static Vector2 GridRange = new Vector2()
            {
                x = GridLength.x + GridInterval.x,
                y = GridLength.y + GridInterval.y,
            };

        }

        public static class BattleMode
        {
            public static int MaxRewardCount = 3;
            //public static int MaxFuneCount = 4;
            public static int MaxBattleCount = 12;
            
            
            public static List<int> InitCards = new List<int>()
            {
                0, 0, 0, 1, 1, 1, 10000, 10000, 10009, 10009
            };
        }

        public static class Battle
        {
            public static Dictionary<EItemType, int> BattleModeRewardRatios = new ()
            {
                [EItemType.UnitCard] = 25,
                [EItemType.TacticCard] = 25,
                [EItemType.Fune] = 25,
                [EItemType.Bless] = 0,
                [EItemType.RemoveCard] = 10,
                [EItemType.AddCardFuneSlot] = 10,
                [EItemType.AddMaxHP] = 5,
                
                // [EItemType.UnitCard] = 20,
                // [EItemType.TacticCard] = 20,
                // [EItemType.Fune] = 20,
                // [EItemType.Bless] = 20,
                // [EItemType.RemoveCard] = 8,
                // [EItemType.AddCardFuneSlot] = 8,
                // [EItemType.AddMaxHP] = 4,
            };
            
            
            public static List<ERelativeCamp> AllRelativeCamps = new List<ERelativeCamp>()
            {
                ERelativeCamp.Enemy,
                ERelativeCamp.Us
            };
            
            public static float CardPosInterval = 155;
            public static float ViewMaxHandCardCount = 10;
            public static int EachHardCardCount = 5;
            public static int CoreCount = 3;
            public static int SelectCardHeight = 105;
            public static int ResetActionTimes = 3;

            public static int SelectInitCardEachCount = 1;
            public static int InitCardMaxCount = 1;
            public static int RushHurt = -1;
            public static int FlyHurt = -1;
            public static float ParabolaBulletShootTime = 0.3f;
            public static float LineBulletShootTime = 0.2f;
            public static float BattleValueVelocity = 4;
            public static float G = 9.8f / 2f;
            public static int ObstacleGridID = 88;
            public static int UnUnitTriggerIdx = -99999999;

            public static Dictionary<EUnitStateEffectType, List<EUnitState>> EffectUnitStates =
                new ()
                {
                    [EUnitStateEffectType.Buff] = new()
                    {       
                        EUnitState.DeBuffUnEffect,
                        //EUnitState.Dodge,
                        EUnitState.AtkPassEnemy,
                        EUnitState.CounterAtk,
                        EUnitState.AddDmg,
                        EUnitState.HurtSubDmg,
                        EUnitState.AtkAddSelfHP,
                    },
                    [EUnitStateEffectType.DeBuff] = new()
                    {
                        EUnitState.HurtRoundStart,
                        EUnitState.HurtEachMove,
                        EUnitState.UnMove,
                        EUnitState.UnAtk,
                        //EUnitState.UnAction,
                        EUnitState.AtkPassUs,
                        EUnitState.SubDmg,
                        EUnitState.HurtAddDmg,
                    }


                };

            public static Dictionary<int, EBattleUnitPos> BatleUnitPoses = new Dictionary<int, EBattleUnitPos>()
            {
                [0] = EBattleUnitPos.Left,
                [1] = EBattleUnitPos.Right,
                [2] = EBattleUnitPos.Center,

            };

            public static Dictionary<EBattleUnitPos, Vector3> BatleUnitDeltaPos = new()
            {
                [EBattleUnitPos.Left] = new Vector3(0f, 0, 0f),
                [EBattleUnitPos.Right] = new Vector3(0.6f, 0, 0.5f),
                [EBattleUnitPos.Center] = new Vector3(0f, 0, -0.5f),

            };

            public static List<EActionType> RelatedUnitFlyRanges = new List<EActionType>()
            {
                EActionType.Cross2Short,
                EActionType.Cross2Extend,
                
            };
            
            public static List<EActionType> DynamicRelatedUnitFlyRanges = new List<EActionType>()
            {
                EActionType.Cross2Short,
                EActionType.Cross2Extend,
                EActionType.Horizontal2Short,
                EActionType.Horizontal2Long,
                EActionType.Horizontal2Extend,
                EActionType.Vertical2Short,
                EActionType.Vertical2Long,
                EActionType.Vertical2Extend,
                //EActionType.LineExtend,
            };

            public static Dictionary<ECardPos, Vector3> CardPos;

            public static Dictionary<EActionType, List<List<Vector2Int>>> ActionTypePoints =
                new()
                {
                    [EActionType.Self] = new List<List<Vector2Int>>()
                    {
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 0),
                        }
                    },
                    
                    [EActionType.Cross2Extend] = new List<List<Vector2Int>>()
                    {
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 0),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 1),
                            new Vector2Int(0, 2),
                            new Vector2Int(0, 3),
                            new Vector2Int(0, 4),
                            new Vector2Int(0, 5),
                            new Vector2Int(0, 6),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(1, 0),
                            new Vector2Int(2, 0),
                            new Vector2Int(3, 0),
                            new Vector2Int(4, 0),
                            new Vector2Int(5, 0),
                            new Vector2Int(6, 0),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, -1),
                            new Vector2Int(0, -2),
                            new Vector2Int(0, -3),
                            new Vector2Int(0, -4),
                            new Vector2Int(0, -5),
                            new Vector2Int(0, -6),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(-1, 0),
                            new Vector2Int(-2, 0),
                            new Vector2Int(-3, 0),
                            new Vector2Int(-4, 0),
                            new Vector2Int(-5, 0),
                            new Vector2Int(-6, 0),
                        },
                        
                        
                    },
                    
                    [EActionType.Cross2Short] = new List<List<Vector2Int>>()
                    {
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 0),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 1),

                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(1, 0),

                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, -1),

                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(-1, 0),

                        },
                        
                        
                        
                    },
                    
                    [EActionType.Cross2Long] = new List<List<Vector2Int>>()
                    {
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 0),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 1),
                            new Vector2Int(0, 2),
                            new Vector2Int(0, 3),
                            new Vector2Int(0, 4),
                            new Vector2Int(0, 5),
                            new Vector2Int(0, 6),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(1, 0),
                            new Vector2Int(2, 0),
                            new Vector2Int(3, 0),
                            new Vector2Int(4, 0),
                            new Vector2Int(5, 0),
                            new Vector2Int(6, 0),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, -1),
                            new Vector2Int(0, -2),
                            new Vector2Int(0, -3),
                            new Vector2Int(0, -4),
                            new Vector2Int(0, -5),
                            new Vector2Int(0, -6),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(-1, 0),
                            new Vector2Int(-2, 0),
                            new Vector2Int(-3, 0),
                            new Vector2Int(-4, 0),
                            new Vector2Int(-5, 0),
                            new Vector2Int(-6, 0),
                        },
                        
                    },
                    
                    [EActionType.Cross2Parabola] = new List<List<Vector2Int>>()
                    {
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 0),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 2),
                            new Vector2Int(0, 3),
                            new Vector2Int(0, 4),
                            new Vector2Int(0, 5),
                            new Vector2Int(0, 6),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(2, 0),
                            new Vector2Int(3, 0),
                            new Vector2Int(4, 0),
                            new Vector2Int(5, 0),
                            new Vector2Int(6, 0),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, -2),
                            new Vector2Int(0, -3),
                            new Vector2Int(0, -4),
                            new Vector2Int(0, -5),
                            new Vector2Int(0, -6),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(-2, 0),
                            new Vector2Int(-3, 0),
                            new Vector2Int(-4, 0),
                            new Vector2Int(-5, 0),
                            new Vector2Int(-6, 0),
                        },
                        
                    },
                    
                    [EActionType.Cross_Long_Empty] = new List<List<Vector2Int>>()
                    {
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 0),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 1),
                            new Vector2Int(0, 2),
                            new Vector2Int(0, 3),
                            new Vector2Int(0, 4),
                            new Vector2Int(0, 5),
                            new Vector2Int(0, 6),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(1, 0),
                            new Vector2Int(2, 0),
                            new Vector2Int(3, 0),
                            new Vector2Int(4, 0),
                            new Vector2Int(5, 0),
                            new Vector2Int(6, 0),
                        },
                        
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, -1),
                            new Vector2Int(0, -2),
                            new Vector2Int(0, -3),
                            new Vector2Int(0, -4),
                            new Vector2Int(0, -5),
                            new Vector2Int(0, -6),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(-1, 0),
                            new Vector2Int(-2, 0),
                            new Vector2Int(-3, 0),
                            new Vector2Int(-4, 0),
                            new Vector2Int(-5, 0),
                            new Vector2Int(-6, 0),
                        },

                        
                    },
                    
                    // [EActionType.X_Extend] = new List<List<Vector2Int>>()
                    // {
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(0, 0),
                    //     },
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(-1, -1),
                    //         new Vector2Int(-2, -2),
                    //         new Vector2Int(-3, -3),
                    //         new Vector2Int(-4, -4),
                    //         new Vector2Int(-5, -5),
                    //         new Vector2Int(-6, -6),
                    //     },
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(1, 1),
                    //         new Vector2Int(2, 2),
                    //         new Vector2Int(3, 3),
                    //         new Vector2Int(4, 4),
                    //         new Vector2Int(5, 5),
                    //         new Vector2Int(6, 6),
                    //     },
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(1, -1),
                    //         new Vector2Int(2, -2),
                    //         new Vector2Int(3, -3),
                    //         new Vector2Int(4, -4),
                    //         new Vector2Int(5, -5),
                    //         new Vector2Int(6, -6),
                    //     },
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(-1, 1),
                    //         new Vector2Int(-2, 2),
                    //         new Vector2Int(-3, 3),
                    //         new Vector2Int(-4, 4),
                    //         new Vector2Int(-5, 5),
                    //         new Vector2Int(-6, 6),
                    //     },
                    // },
                    
                    // [EActionType.X_Short] = new List<List<Vector2Int>>()
                    // {
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(0, 0),
                    //     },
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(-1, -1),
                    //     },
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(1, 1),
                    //     },
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(1, -1),
                    //     },
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(-1, 1),
                    //     },
                    // },
                    
                    // [EActionType.X_Long] = new List<List<Vector2Int>>()
                    // {
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(0, 0),
                    //     },
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(-1, -1),
                    //         new Vector2Int(-2, -2),
                    //         new Vector2Int(-3, -3),
                    //         new Vector2Int(-4, -4),
                    //         new Vector2Int(-5, -5),
                    //         new Vector2Int(-6, -6),
                    //     },
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(1, 1),
                    //         new Vector2Int(2, 2),
                    //         new Vector2Int(3, 3),
                    //         new Vector2Int(4, 4),
                    //         new Vector2Int(5, 5),
                    //         new Vector2Int(6, 6),
                    //     },
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(1, -1),
                    //         new Vector2Int(2, -2),
                    //         new Vector2Int(3, -3),
                    //         new Vector2Int(4, -4),
                    //         new Vector2Int(5, -5),
                    //         new Vector2Int(6, -6),
                    //     },
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(-1, 1),
                    //         new Vector2Int(-2, 2),
                    //         new Vector2Int(-3, 3),
                    //         new Vector2Int(-4, 4),
                    //         new Vector2Int(-5, 5),
                    //         new Vector2Int(-6, 6),
                    //     },
                    // },
                
                    [EActionType.Direct82Short] = new List<List<Vector2Int>>()
                    {
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 0),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 1),
                        },
                        new List<Vector2Int>()
                        {

                            new Vector2Int(1, 1),

                        },
                        
                        new List<Vector2Int>()
                        {
                            
                            new Vector2Int(1, 0),

                        },
                        new List<Vector2Int>()
                        {
                            
                            new Vector2Int(1, -1),

                        },
                        new List<Vector2Int>()
                        {
                            
                            new Vector2Int(0, -1),
                        },
                        new List<Vector2Int>()
                        {
                            
                            new Vector2Int(-1, -1),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(-1, 0),

                        },
                        new List<Vector2Int>()
                        {

                            new Vector2Int(-1, 1),

                        },

                    },
                    
                    [EActionType.Direct82Long] = new List<List<Vector2Int>>()
                    {
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 0),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 1),
                            new Vector2Int(0, 2),
                            new Vector2Int(0, 3),
                            new Vector2Int(0, 4),
                            new Vector2Int(0, 5),
                            new Vector2Int(0, 6),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(1, 1),
                            new Vector2Int(2, 2),
                            new Vector2Int(3, 3),
                            new Vector2Int(4, 4),
                            new Vector2Int(5, 5),
                            new Vector2Int(6, 6),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(1, 0),
                            new Vector2Int(2, 0),
                            new Vector2Int(3, 0),
                            new Vector2Int(4, 0),
                            new Vector2Int(5, 0),
                            new Vector2Int(6, 0),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(1, -1),
                            new Vector2Int(2, -2),
                            new Vector2Int(3, -3),
                            new Vector2Int(4, -4),
                            new Vector2Int(5, -5),
                            new Vector2Int(6, -6),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, -1),
                            new Vector2Int(0, -2),
                            new Vector2Int(0, -3),
                            new Vector2Int(0, -4),
                            new Vector2Int(0, -5),
                            new Vector2Int(0, -6),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(-1, -1),
                            new Vector2Int(-2, -2),
                            new Vector2Int(-3, -3),
                            new Vector2Int(-4, -4),
                            new Vector2Int(-5, -5),
                            new Vector2Int(-6, -6),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(-1, 0),
                            new Vector2Int(-2, 0),
                            new Vector2Int(-3, 0),
                            new Vector2Int(-4, 0),
                            new Vector2Int(-5, 0),
                            new Vector2Int(-6, 0),
                        },

                        new List<Vector2Int>()
                        {
                            new Vector2Int(-1, 1),
                            new Vector2Int(-2, 2),
                            new Vector2Int(-3, 3),
                            new Vector2Int(-4, 4),
                            new Vector2Int(-5, 5),
                            new Vector2Int(-6, 6),
                        },
                        
                    },
                    
                    [EActionType.Direct82Parabola] = new List<List<Vector2Int>>()
                    {
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 0),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 2),
                            new Vector2Int(0, 3),
                            new Vector2Int(0, 4),
                            new Vector2Int(0, 5),
                            new Vector2Int(0, 6),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(2, 2),
                            new Vector2Int(3, 3),
                            new Vector2Int(4, 4),
                            new Vector2Int(5, 5),
                            new Vector2Int(6, 6),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(2, 0),
                            new Vector2Int(3, 0),
                            new Vector2Int(4, 0),
                            new Vector2Int(5, 0),
                            new Vector2Int(6, 0),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(2, -2),
                            new Vector2Int(3, -3),
                            new Vector2Int(4, -4),
                            new Vector2Int(5, -5),
                            new Vector2Int(6, -6),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, -2),
                            new Vector2Int(0, -3),
                            new Vector2Int(0, -4),
                            new Vector2Int(0, -5),
                            new Vector2Int(0, -6),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(-2, -2),
                            new Vector2Int(-3, -3),
                            new Vector2Int(-4, -4),
                            new Vector2Int(-5, -5),
                            new Vector2Int(-6, -6),
                        },
                        
                        new List<Vector2Int>()
                        {
                            new Vector2Int(-2, 0),
                            new Vector2Int(-3, 0),
                            new Vector2Int(-4, 0),
                            new Vector2Int(-5, 0),
                            new Vector2Int(-6, 0),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(-2, 2),
                            new Vector2Int(-3, 3),
                            new Vector2Int(-4, 4),
                            new Vector2Int(-5, 5),
                            new Vector2Int(-6, 6),
                        },
                        
                    },
                    
                    [EActionType.Direct82Extend] = new List<List<Vector2Int>>()
                    {
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 0),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 1),
                            new Vector2Int(0, 2),
                            new Vector2Int(0, 3),
                            new Vector2Int(0, 4),
                            new Vector2Int(0, 5),
                            new Vector2Int(0, 6),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(1, 1),
                            new Vector2Int(2, 2),
                            new Vector2Int(3, 3),
                            new Vector2Int(4, 4),
                            new Vector2Int(5, 5),
                            new Vector2Int(6, 6),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(1, 0),
                            new Vector2Int(2, 0),
                            new Vector2Int(3, 0),
                            new Vector2Int(4, 0),
                            new Vector2Int(5, 0),
                            new Vector2Int(6, 0),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(1, -1),
                            new Vector2Int(2, -2),
                            new Vector2Int(3, -3),
                            new Vector2Int(4, -4),
                            new Vector2Int(5, -5),
                            new Vector2Int(6, -6),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, -1),
                            new Vector2Int(0, -2),
                            new Vector2Int(0, -3),
                            new Vector2Int(0, -4),
                            new Vector2Int(0, -5),
                            new Vector2Int(0, -6),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(-1, -1),
                            new Vector2Int(-2, -2),
                            new Vector2Int(-3, -3),
                            new Vector2Int(-4, -4),
                            new Vector2Int(-5, -5),
                            new Vector2Int(-6, -6),
                        },
                        
                        new List<Vector2Int>()
                        {
                            new Vector2Int(-1, 0),
                            new Vector2Int(-2, 0),
                            new Vector2Int(-3, 0),
                            new Vector2Int(-4, 0),
                            new Vector2Int(-5, 0),
                            new Vector2Int(-6, 0),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(-1, 1),
                            new Vector2Int(-2, 2),
                            new Vector2Int(-3, 3),
                            new Vector2Int(-4, 4),
                            new Vector2Int(-5, 5),
                            new Vector2Int(-6, 6),
                        },
                    },
                    
                    [EActionType.Direct8_Long_Empty] = new List<List<Vector2Int>>()
                    {
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 0),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 1),
                            new Vector2Int(0, 2),
                            new Vector2Int(0, 3),
                            new Vector2Int(0, 4),
                            new Vector2Int(0, 5),
                            new Vector2Int(0, 6),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(1, 1),
                            new Vector2Int(2, 2),
                            new Vector2Int(3, 3),
                            new Vector2Int(4, 4),
                            new Vector2Int(5, 5),
                            new Vector2Int(6, 6),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(1, 0),
                            new Vector2Int(2, 0),
                            new Vector2Int(3, 0),
                            new Vector2Int(4, 0),
                            new Vector2Int(5, 0),
                            new Vector2Int(6, 0),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(1, -1),
                            new Vector2Int(2, -2),
                            new Vector2Int(3, -3),
                            new Vector2Int(4, -4),
                            new Vector2Int(5, -5),
                            new Vector2Int(6, -6),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, -1),
                            new Vector2Int(0, -2),
                            new Vector2Int(0, -3),
                            new Vector2Int(0, -4),
                            new Vector2Int(0, -5),
                            new Vector2Int(0, -6),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(-1, -1),
                            new Vector2Int(-2, -2),
                            new Vector2Int(-3, -3),
                            new Vector2Int(-4, -4),
                            new Vector2Int(-5, -5),
                            new Vector2Int(-6, -6),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(-1, 0),
                            new Vector2Int(-2, 0),
                            new Vector2Int(-3, 0),
                            new Vector2Int(-4, 0),
                            new Vector2Int(-5, 0),
                            new Vector2Int(-6, 0),
                        },

                        new List<Vector2Int>()
                        {
                            new Vector2Int(-1, 1),
                            new Vector2Int(-2, 2),
                            new Vector2Int(-3, 3),
                            new Vector2Int(-4, 4),
                            new Vector2Int(-5, 5),
                            new Vector2Int(-6, 6),
                        },
                    },
                    
                    // [EActionType.Direct8_Extend] = new List<List<Vector2Int>>()
                    // {
                    //     // new List<Vector2Int>()
                    //     // {
                    //     //     new Vector2Int(0, 0),
                    //     // },
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(-1, 0),
                    //         new Vector2Int(-2, 0),
                    //         new Vector2Int(-3, 0),
                    //         new Vector2Int(-4, 0),
                    //         new Vector2Int(-5, 0),
                    //         new Vector2Int(-6, 0),
                    //     },
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(1, 0),
                    //         new Vector2Int(2, 0),
                    //         new Vector2Int(3, 0),
                    //         new Vector2Int(4, 0),
                    //         new Vector2Int(5, 0),
                    //         new Vector2Int(6, 0),
                    //     },
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(0, -1),
                    //         new Vector2Int(0, -2),
                    //         new Vector2Int(0, -3),
                    //         new Vector2Int(0, -4),
                    //         new Vector2Int(0, -5),
                    //         new Vector2Int(0, -6),
                    //     },
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(0, 1),
                    //         new Vector2Int(0, 2),
                    //         new Vector2Int(0, 3),
                    //         new Vector2Int(0, 4),
                    //         new Vector2Int(0, 5),
                    //         new Vector2Int(0, 6),
                    //     },
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(-1, -1),
                    //         new Vector2Int(-2, -2),
                    //         new Vector2Int(-3, -3),
                    //         new Vector2Int(-4, -4),
                    //         new Vector2Int(-5, -5),
                    //         new Vector2Int(-6, -6),
                    //     },
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(1, 1),
                    //         new Vector2Int(2, 2),
                    //         new Vector2Int(3, 3),
                    //         new Vector2Int(4, 4),
                    //         new Vector2Int(5, 5),
                    //         new Vector2Int(6, 6),
                    //     },
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(-1, 1),
                    //         new Vector2Int(-2, 2),
                    //         new Vector2Int(-3, 3),
                    //         new Vector2Int(-4, 4),
                    //         new Vector2Int(-5, 5),
                    //         new Vector2Int(-6, 6),
                    //     },
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(1, -1),
                    //         new Vector2Int(2, -2),
                    //         new Vector2Int(3, -3),
                    //         new Vector2Int(4, -4),
                    //         new Vector2Int(5, -5),
                    //         new Vector2Int(6, -6),
                    //     },
                    // },

                    // [EActionType.Parabola] = new List<List<Vector2Int>>()
                    // {
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(0, 0),
                    //     },
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(-2, 0),
                    //         new Vector2Int(-3, 0),
                    //         new Vector2Int(-4, 0),
                    //         new Vector2Int(-5, 0),
                    //         new Vector2Int(-6, 0),
                    //     },
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(2, 0),
                    //         new Vector2Int(3, 0),
                    //         new Vector2Int(4, 0),
                    //         new Vector2Int(5, 0),
                    //         new Vector2Int(6, 0),
                    //     },
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(0, -2),
                    //         new Vector2Int(0, -3),
                    //         new Vector2Int(0, -4),
                    //         new Vector2Int(0, -5),
                    //         new Vector2Int(0, -6),
                    //     },
                    //     new List<Vector2Int>()
                    //     {
                    //         new Vector2Int(0, 2),
                    //         new Vector2Int(0, 3),
                    //         new Vector2Int(0, 4),
                    //         new Vector2Int(0, 5),
                    //         new Vector2Int(0, 6),
                    //     },
                    //     // new List<Vector2Int>()
                    //     // {
                    //     //     new Vector2Int(-2, -2),
                    //     //     new Vector2Int(-3, -3),
                    //     //     new Vector2Int(-4, -4),
                    //     //     new Vector2Int(-5, -5),
                    //     //     new Vector2Int(-6, -6),
                    //     // },
                    //     // new List<Vector2Int>()
                    //     // {
                    //     //     new Vector2Int(2, 2),
                    //     //     new Vector2Int(3, 3),
                    //     //     new Vector2Int(4, 4),
                    //     //     new Vector2Int(5, 5),
                    //     //     new Vector2Int(6, 6),
                    //     // },
                    //     // new List<Vector2Int>()
                    //     // {
                    //     //     new Vector2Int(-2, 2),
                    //     //     new Vector2Int(-3, 3),
                    //     //     new Vector2Int(-4, 4),
                    //     //     new Vector2Int(-5, 5),
                    //     //     new Vector2Int(-6, 6),
                    //     // },
                    //     // new List<Vector2Int>()
                    //     // {
                    //     //     new Vector2Int(2, -2),
                    //     //     new Vector2Int(3, -3),
                    //     //     new Vector2Int(4, -4),
                    //     //     new Vector2Int(5, -5),
                    //     //     new Vector2Int(6, -6),
                    //     // },
                    // },
                    
                    [EActionType.Empty] = new List<List<Vector2Int>>()
                    {},
                    
                    [EActionType.Row] = new List<List<Vector2Int>>()
                    {

                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 1),
                            new Vector2Int(0, 2),
                            new Vector2Int(0, 3),
                            new Vector2Int(0, 4),
                            new Vector2Int(0, 5),
                            new Vector2Int(0, 6),
                        },
                    },
                    
                    [EActionType.Column] = new List<List<Vector2Int>>()
                    {

                        new List<Vector2Int>()
                        {
                            new Vector2Int(1, 0),
                            new Vector2Int(2, 0),
                            new Vector2Int(3, 0),
                            new Vector2Int(4, 0),
                            new Vector2Int(5, 0),
                            new Vector2Int(6, 0),
                        },
                    },
                    
                    [EActionType.Horizontal2Short] = new List<List<Vector2Int>>()
                    {
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 0),
                        },
                        
                        new List<Vector2Int>()
                        {
                            new Vector2Int(1, 0),
                        },
                        
                        new List<Vector2Int>()
                        {
                            new Vector2Int(-1, 0),
                        },
                    },
                    
                    [EActionType.Horizontal2Long] = new List<List<Vector2Int>>()
                    {
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 0),
                            
                        },
                        
                        new List<Vector2Int>()
                        {
                            new Vector2Int(1, 0),
                            new Vector2Int(2, 0),
                            new Vector2Int(3, 0),
                            new Vector2Int(4, 0),
                            new Vector2Int(5, 0),
                            new Vector2Int(6, 0),
                        },
                        
                        new List<Vector2Int>()
                        {
                            new Vector2Int(-1, 0),
                            new Vector2Int(-2, 0),
                            new Vector2Int(-3, 0),
                            new Vector2Int(-4, 0),
                            new Vector2Int(-5, 0),
                            new Vector2Int(-6, 0),
                        },
                    },
                    
                    [EActionType.Horizontal2Extend] = new List<List<Vector2Int>>()
                    {
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 0),
                            
                        },
                        
                        new List<Vector2Int>()
                        {
                            new Vector2Int(1, 0),
                            new Vector2Int(2, 0),
                            new Vector2Int(3, 0),
                            new Vector2Int(4, 0),
                            new Vector2Int(5, 0),
                            new Vector2Int(6, 0),
                        },
                        
                        new List<Vector2Int>()
                        {
                            new Vector2Int(-1, 0),
                            new Vector2Int(-2, 0),
                            new Vector2Int(-3, 0),
                            new Vector2Int(-4, 0),
                            new Vector2Int(-5, 0),
                            new Vector2Int(-6, 0),
                        },
                    },
                    
                    [EActionType.Vertical2Short] = new List<List<Vector2Int>>()
                    {
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 0),
                        },
                        
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 1),
                        },
                        
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, -1),
                        },
                    },
                    
                    [EActionType.Vertical2Long] = new List<List<Vector2Int>>()
                    {
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 0),
                            
                        },
                        
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 1),
                            new Vector2Int(0, 2),
                            new Vector2Int(0, 3),
                            new Vector2Int(0, 4),
                            new Vector2Int(0, 5),
                            new Vector2Int(0, 6),
                        },
                        
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, -1),
                            new Vector2Int(0, -2),
                            new Vector2Int(0, -3),
                            new Vector2Int(0, -4),
                            new Vector2Int(0, -5),
                            new Vector2Int(0, -6),
                        },
                    },
                    
                    [EActionType.Vertical2Extend] = new List<List<Vector2Int>>()
                    {
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 0),
                            
                        },
                        
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 1),
                            new Vector2Int(0, 2),
                            new Vector2Int(0, 3),
                            new Vector2Int(0, 4),
                            new Vector2Int(0, 5),
                            new Vector2Int(0, 6),
                        },
                        
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, -1),
                            new Vector2Int(0, -2),
                            new Vector2Int(0, -3),
                            new Vector2Int(0, -4),
                            new Vector2Int(0, -5),
                            new Vector2Int(0, -6),
                        },
                    }
                };
            
            
            

            public static Dictionary<ERelativePos, Vector2Int> EPos2CoordMap = new()
            {
                [ERelativePos.Left] = new(-1, 0),

                [ERelativePos.LeftUp] = new(-1, 1),

                [ERelativePos.Up] = new(0, 1),

                [ERelativePos.RightUp] = new(1, 1),

                [ERelativePos.Right] = new(1, 0),

                [ERelativePos.RightDown] = new(1, -1),

                [ERelativePos.Down] = new(0, -1),

                [ERelativePos.LeftDown] = new(-1, -1),

            };

            public static Dictionary<Vector2Int, ERelativePos> Coord2PosMap = new()
            {
                [new(-1, 0)] = ERelativePos.Left,

                [new(-1, 1)] = ERelativePos.LeftUp,

                [new(0, 1)] = ERelativePos.Up,

                [new(1, 1)] = ERelativePos.RightUp,

                [new(1, 0)] = ERelativePos.Right,

                [new(1, -1)] = ERelativePos.RightDown,

                [new(0, -1)] = ERelativePos.Down,

                [new(-1, -1)] = ERelativePos.LeftDown,

            };

            public static Dictionary<ERelativePos, ERelativePos> UnitPassRoute = new()
            {
                [ERelativePos.Left] = ERelativePos.Left,

                [ERelativePos.LeftUp] = ERelativePos.Left,

                [ERelativePos.Up] = ERelativePos.Left,

                [ERelativePos.RightUp] = ERelativePos.Up,

                [ERelativePos.Right] = ERelativePos.Down,

                [ERelativePos.RightDown] = ERelativePos.Down,

                [ERelativePos.Down] = ERelativePos.Left,

                [ERelativePos.LeftDown] = ERelativePos.Left,

            };

            // public static Dictionary<EBuffID, EActionType> LinkageBuffs = new Dictionary<EBuffID, EActionType>()
            // {
            //     [EBuffID.Linkage_Around] = EActionType.Around,
            //     [EBuffID.Linkage_Cross_Around] = EActionType.Cross_Short,
            //     [EBuffID.Linkage_X_Around] = EActionType.X_Short,
            //     [EBuffID.Linkage_Cross_Long] = EActionType.Cross_Long,
            //     [EBuffID.Linkage_X_Long] = EActionType.X_Long,
            // };
        }

        public static class Unit
        {
            public static Dictionary<EUnitActionState, float> MoveTimes = new Dictionary<EUnitActionState, float>()
            {
                [EUnitActionState.Run] = 0.5f,
                [EUnitActionState.Fly] = 0.4f,
                [EUnitActionState.Rush] = 0.3f,
                [EUnitActionState.Throw] = 0.4f,
            };
            
            
        }

        // public class EnemyGenrateRule
        // {
        //     public Dictionary<int, int> RoundGenerateUnitCount = new Dictionary<int, int>();
        //     public List<int> LevelCounts = new List<int>();
        //     public List<int> LevelTypeCounts = new List<int>();
        //     public List<int> AttackTypes = new List<int>();
        //     public int EachRoundUnitCount = 0;
        //     public int GlobalDebuffCount;
        // }

        public static class Enemy
        {
            // public static Dictionary<EGameDifficulty, Dictionary<int, EnemyGenrateRule>> EnemyGenerateRules = new()
            // {
            //     
            //     [EGameDifficulty.Difficulty1] = new Dictionary<int, EnemyGenrateRule>()
            //     {
            //         [0] = new EnemyGenrateRule()
            //         {
            //             RoundGenerateUnitCount = new Dictionary<int, int>()
            //             {
            //                 [0] = 2,
            //                 [2] = 2,
            //                 [4] = 2,
            //             },
            //             LevelCounts = {6, 0, 0},
            //             LevelTypeCounts = {3, 0, 0},
            //             EachRoundUnitCount = 2,
            //             GlobalDebuffCount = 0,
            //         },
            //         [1] = new EnemyGenrateRule()
            //         {
            //             RoundGenerateUnitCount = new Dictionary<int, int>()
            //             {
            //                 [0] = 2,
            //                 [2] = 2,
            //                 [4] = 2,
            //             },
            //             LevelCounts = {4, 2, 0},
            //             LevelTypeCounts = {2, 1, 0},
            //             EachRoundUnitCount = 2,
            //             GlobalDebuffCount = 0,
            //         },
            //         [2] = new EnemyGenrateRule()
            //         {
            //             RoundGenerateUnitCount = new Dictionary<int, int>()
            //             {
            //                 [0] = 2,
            //                 [2] = 2,
            //                 [4] = 2,
            //             },
            //             LevelCounts = {2, 4, 0},
            //             LevelTypeCounts = {1, 2, 0},
            //             EachRoundUnitCount = 2,
            //             GlobalDebuffCount = 0,
            //         },
            //         [3] = new EnemyGenrateRule()
            //         {
            //             RoundGenerateUnitCount = new Dictionary<int, int>()
            //             {
            //                 [0] = 2,
            //                 [2] = 2,
            //                 [4] = 3,
            //             },
            //             LevelCounts = {0, 7, 0},
            //             LevelTypeCounts = {0, 3, 0},
            //             EachRoundUnitCount = 2,
            //             GlobalDebuffCount = 0,
            //         },
            //         [4] = new EnemyGenrateRule()
            //         {
            //             RoundGenerateUnitCount = new Dictionary<int, int>()
            //             {
            //                 [0] = 2,
            //                 [2] = 2,
            //                 [4] = 3,
            //             },
            //             LevelCounts = {2, 5, 0},
            //             LevelTypeCounts = {1, 2, 0},
            //             EachRoundUnitCount = 2,
            //             GlobalDebuffCount = 0,
            //         },
            //         [5] = new EnemyGenrateRule()
            //         {
            //             RoundGenerateUnitCount = new Dictionary<int, int>()
            //             {
            //                 [0] = 2,
            //                 [2] = 2,
            //                 [4] = 3,
            //             },
            //             LevelCounts = {0, 7, 0},
            //             LevelTypeCounts = {0, 3, 0},
            //             EachRoundUnitCount = 2,
            //             GlobalDebuffCount = 0,
            //         },
            //         [6] = new EnemyGenrateRule()
            //         {
            //             RoundGenerateUnitCount = new Dictionary<int, int>()
            //             {
            //                 [0] = 2,
            //                 [2] = 2,
            //                 [4] = 3,
            //             },
            //             LevelCounts = {0, 5, 2},
            //             LevelTypeCounts = {0, 2, 1},
            //             EachRoundUnitCount = 2,
            //             GlobalDebuffCount = 0,
            //         },
            //         [7] = new EnemyGenrateRule()
            //         {
            //             RoundGenerateUnitCount = new Dictionary<int, int>()
            //             {
            //                 [0] = 2,
            //                 [2] = 2,
            //                 [4] = 3,
            //             },
            //             LevelCounts = {0, 0, 7},
            //             LevelTypeCounts = {0, 0, 3},
            //             EachRoundUnitCount = 2,
            //             GlobalDebuffCount = 0,
            //         },
            //         [8] = new EnemyGenrateRule()
            //         {
            //             RoundGenerateUnitCount = new Dictionary<int, int>()
            //             {
            //                 [0] = 2,
            //                 [2] = 2,
            //                 [4] = 3,
            //             },
            //             LevelCounts = {0, 2, 5},
            //             LevelTypeCounts = {0, 1, 2},
            //             EachRoundUnitCount = 2,
            //             GlobalDebuffCount = 0,
            //         },
            //         [9] = new EnemyGenrateRule()
            //         {
            //             RoundGenerateUnitCount = new Dictionary<int, int>()
            //             {
            //                 [0] = 2,
            //                 [2] = 2,
            //                 [4] = 3,
            //             },
            //             LevelCounts = {0, 0, 7},
            //             LevelTypeCounts = {0, 0, 3},
            //             EachRoundUnitCount = 2,
            //             GlobalDebuffCount = 0,
            //         },
            //         [10] = new EnemyGenrateRule()
            //         {
            //             RoundGenerateUnitCount = new Dictionary<int, int>()
            //             {
            //                 [0] = 2,
            //                 [2] = 3,
            //                 [4] = 3,
            //             },
            //             LevelCounts = {0, 0, 8},
            //             LevelTypeCounts = {0, 0, 3},
            //             EachRoundUnitCount = 2,
            //             GlobalDebuffCount = 0,
            //         },
            //         [11] = new EnemyGenrateRule()
            //         {
            //             RoundGenerateUnitCount = new Dictionary<int, int>()
            //             {
            //                 [0] = 3,
            //                 [2] = 3,
            //                 [4] = 3,
            //             },
            //             LevelCounts = {0, 0, 7},
            //             LevelTypeCounts = {0, 0, 3},
            //             EachRoundUnitCount = 2,
            //             GlobalDebuffCount = 0,
            //         },
            //         
            //     }
            //     
            //     // [EGameDifficulty.Difficulty2] = new EnemyGenrateRule()
            //     // {
            //     //     RoundGenerateUnitCount = new Dictionary<int, int>()
            //     //     {
            //     //         [0] = 2,
            //     //         [2] = 2,
            //     //         [4] = 2,
            //     //     },
            //     //     LevelCounts = {6, 0, 0},
            //     //     LevelTypeCounts = {3, 0, 0},
            //     //     EachRoundUnitCount = 2,
            //     //     // NormalUnitCount = 2,    
            //     //     // EliteUnitCount = 0,
            //     //     // NormalUnitTypeCount = 2,
            //     //     // EliteUnitTypeCount = 0,
            //     //     GlobalDebuffCount = 0,
            //     // },
            //     // [EGameDifficulty.Difficulty3] = new EnemyGenrateRule()
            //     // {
            //     //     RoundGenerateUnitCount = new Dictionary<int, int>()
            //     //     {
            //     //         [0] = 2,
            //     //         [2] = 2,
            //     //         [4] = 2,
            //     //     },
            //     //     LevelCounts = {6, 0, 0},
            //     //     LevelTypeCounts = {3, 0, 0},
            //     //     EachRoundUnitCount = 2,
            //     //     // NormalUnitCount = 2,    
            //     //     // EliteUnitCount = 0,
            //     //     // NormalUnitTypeCount = 2,
            //     //     // EliteUnitTypeCount = 0,
            //     //     GlobalDebuffCount = 0,
            //     // },
            //     // [EGameDifficulty.Difficulty4] = new EnemyGenrateRule()
            //     // {
            //     //     RoundGenerateUnitCount = new Dictionary<int, int>()
            //     //     {
            //     //         [0] = 2,
            //     //         [2] = 2,
            //     //         [4] = 2,
            //     //     },
            //     //     LevelCounts = {6, 0, 0},
            //     //     LevelTypeCounts = {3, 0, 0},
            //     //     EachRoundUnitCount = 2,
            //     //     GlobalDebuffCount = 0,
            //     // },
            //     // [EGameDifficulty.Difficulty5] = new EnemyGenrateRule()
            //     // {
            //     //     RoundGenerateUnitCount = new Dictionary<int, int>()
            //     //     {
            //     //         [0] = 2,
            //     //         [2] = 2,
            //     //         [4] = 2,
            //     //     },
            //     //     LevelCounts = {6, 0, 0},
            //     //     LevelTypeCounts = {3, 0, 0},
            //     //     EachRoundUnitCount = 2,
            //     //     GlobalDebuffCount = 0,
            //     // },
            // };

            public static int EnemyActionCount = 3;

            public static float MoveTime = 0.7f;
            
        }

        public static class Map
        {
            public static int MapCount = 3;
            public static int StageCount = 3;

            public static int RouteCount = 4;
            public static int StepCount = 5;
            
            // public static Dictionary<int, int> StepRangeRatio = new ()
            // {
            //     [5] = 100,
            // };

            public static Dictionary<EMapSite, int> MapSiteRatio = new ()
            {
                [EMapSite.NormalBattle] = 50,
                [EMapSite.EliteBattle] = 8,
                [EMapSite.Store] = 8,
                [EMapSite.Rest] = 4,
                [EMapSite.Treasure] = 15,
                [EMapSite.Event] = 15,
            };

            public static Dictionary<EMapSite, int> MapSiteGuarantee = new()
            {
                [EMapSite.NormalBattle] = 8,
                [EMapSite.EliteBattle] = 1,
                [EMapSite.Store] = 1,
                [EMapSite.Rest] = 1,
                [EMapSite.Treasure] = 2,
                [EMapSite.Event] = 2,
            };
        }

        public static class Store
        {
            public static Vector2Int CardPriceRange = new Vector2Int(85, 115);
            public static Vector2Int BlessPriceRange = new Vector2Int(85, 115);
            public static Vector2Int FunePriceRange = new Vector2Int(85, 115);
        }

        public static class Rest
        {
            public static int AddMaxEnergy = 2;
            public static int AddHeart = 1;
        }
        

        public static class GridProp
        {
            public static int GridPropRoundEffectCount = 2;
        }
        
        public static class ThirdUnit
        {
            
        }

        public enum CardType
        {
            Fight,
            Tactic,
        }

        public class InitCardData
        {
            public int CardID;
            public List<int> FuneIDs = new List<int>();
        }
        
        public class InitData
        {
            public int Coin = 350;
            public List<EBlessID> InitBlesses = new List<EBlessID>();
            public List<int> InitFunes = new List<int>();
            public List<InitCardData> InitCards = new();
        }

        public static class Hero
        {
            public static List<EItemType> AttributeItemTypes = new List<EItemType>()
            {
                EItemType.Coin,
                EItemType.AddMaxHP,
                //EItemType.Heart,
            };
            
            public static List<EItemType> CommonItemTypes = new List<EItemType>()
            {
                EItemType.Bless,
                EItemType.Fune,
                EItemType.AddCardFuneSlot,
                EItemType.AddMaxHP,
                EItemType.RemoveCard,
            };
            
            public static Dictionary<EUnitCamp, InitData> InitDatas = new()
            {
                [EUnitCamp.Player1] = new InitData()
                {
                    InitBlesses = new List<EBlessID>()
                    {
                        //EBlessID.ShuffleCardAddCurHP,
                        //EBlessID.PassCardAcquireCard,
                        // 0,
                        // 1,
                        // 2,
                        //3,
                    },
                    
                    InitFunes = new List<int>()
                    {
                        //0,
                        //1,
                        //2,
                        //2,
                        // EFuneID.EachRound_AddCurHP, //0
                        // EFuneID.AddCurHP, //1
                        // EFuneID.EachRound_AddCurHP, //2
                    },
                    InitCards = new()
                    {
                        new()
                        {
                            CardID = 0,
                            FuneIDs = new List<int>()
                            {
                                1
                            }
                        },
                        new()
                        {
                            CardID = 0,
                            FuneIDs = new List<int>()
                            {
                                //1,2
                            }
                        },
                        new()
                        {
                            CardID = 0,
                            FuneIDs = new List<int>()
                            {
                                //3
                            }
                        },
                        new()
                        {
                            CardID = 10000,
                            FuneIDs = new List<int>()
                            {
                            }
                        },

                        new()
                        {
                            CardID = 10000,
                            FuneIDs = new List<int>()
                            {
                            }
                        },
                        new()
                        {
                            CardID = 10000,
                            FuneIDs = new List<int>()
                            {
                            }
                        },
                        
                        new()
                        {
                            CardID = 1,
                            FuneIDs = new List<int>()
                            {
                            }
                        },
                        new()
                        {
                            CardID = 1,
                            FuneIDs = new List<int>()
                            {
                            }
                        },
                        new()
                        {
                            CardID = 1,
                            FuneIDs = new List<int>()
                            {
                            }
                        },
                        
                    },
                },
                
                [EUnitCamp.Player2] = new InitData()
                {
                    InitFunes = new List<int>()
                    {
                        // EFuneID.EachRound_AddCurHP, //0
                        // EFuneID.AddCurHP, //1
                        // EFuneID.EachRound_AddCurHP, //2
                    },
                    InitCards = new()
                    {

                        new()
                        {
                            CardID = 0,
                            FuneIDs = new List<int>()
                            {
                            }
                        },
                        new()
                        {
                            CardID = 0,
                            FuneIDs = new List<int>()
                            {
                            }
                        },
                        new()
                        {
                            CardID = 0,
                            FuneIDs = new List<int>()
                            {
                            }
                        },
                        new()
                        {
                            CardID = 0,
                            FuneIDs = new List<int>()
                            {
                            }
                        },
                        new()
                        {
                            CardID = 0,
                            FuneIDs = new List<int>()
                            {
                            }
                        },
                        new()
                        {
                            CardID = 0,
                            FuneIDs = new List<int>()
                            {
                            }
                        },
                        new()
                        {
                            CardID = 0,
                            FuneIDs = new List<int>()
                            {
                            }
                        },

                    },
                },
            };
            
            
            // public static List<EFuneID> InitFunes = new List<EFuneID>()
            // {
            //     // EFuneID.EachRound_AddCurHP, //0
            //     // EFuneID.AddCurHP, //1
            //     // EFuneID.EachRound_AddCurHP, //2
            // };
            //
            //
            //
            // public static List<InitCardData> Player1InitCards = new ()
            // {
            //     // new ()
            //     // {
            //     //     CardID = ECardID.UnRemove,
            //     //     FuneIDs = new List<int>()
            //     //     {
            //     //         // EFuneID.AddCurHP,
            //     //         // EFuneID.Link_Receive_Around_Us,
            //     //     }
            //     // },
            //     // new ()
            //     // {
            //     //     CardID = ECardID.UnRemove,
            //     //     FuneIDs = new List<int>()
            //     //     {
            //     //         0,
            //     //     }
            //     // },
            //     // new ()
            //     // {
            //     //     CardID = ECardID.UnRemove,
            //     //     FuneIDs = new List<int>()
            //     //     {
            //     //         1,
            //     //     }
            //     // },
            //     new ()
            //     {
            //         CardID = ECardID.MoveUnEnemy,
            //         FuneIDs = new List<int>()
            //         {
            //         }
            //     },
            //     new ()
            //     {
            //         CardID = ECardID.MoveUnEnemy,
            //         FuneIDs = new List<int>()
            //         {
            //         }
            //     },
            //     new ()
            //     {
            //         CardID = ECardID.MoveUnEnemy,
            //         FuneIDs = new List<int>()
            //         {
            //         }
            //     },
            //     new ()
            //     {
            //         CardID = ECardID.MoveGrid,
            //         FuneIDs = new List<int>()
            //         {
            //         }
            //     },
            //
            //     new ()
            //     {
            //         CardID = ECardID.MoveGrid,
            //         FuneIDs = new List<int>()
            //         {
            //         }
            //     },
            //     new ()
            //     {
            //         CardID = ECardID.MoveGrid,
            //         FuneIDs = new List<int>()
            //         {
            //         }
            //     },
            //     new ()
            //     {
            //         CardID = ECardID.Move_Attack_BePass,
            //         FuneIDs = new List<int>()
            //         {
            //         }
            //     },
            //     new ()
            //     {
            //         CardID = ECardID.Move_Attack_BePass,
            //         FuneIDs = new List<int>()
            //         {
            //         }
            //     },
            //     new ()
            //     {
            //         CardID = ECardID.Move_Attack_BePass,
            //         FuneIDs = new List<int>()
            //         {
            //         }
            //     },
            //     // new ()
            //     // {
            //     //     CardID = ECardID.MoveGrid,
            //     //     FuneIDs = new List<int>()
            //     //     {
            //     //     }
            //     // },
            //     // new ()
            //     // {
            //     //     CardID = ECardID.MoveGrid,
            //     //     FuneIDs = new List<int>()
            //     //     {
            //     //     }
            //     // },
            //     // new ()
            //     // {
            //     //     CardID = ECardID.MoveGrid,
            //     //     FuneIDs = new List<int>()
            //     //     {
            //     //     }
            //     // },
            // };
            //
            // public static List<InitCardData> Player2InitCards = new ()
            // {
            //     new ()
            //     {
            //         CardID = ECardID.MoveUnEnemy,
            //         FuneIDs = new List<int>()
            //         {
            //         }
            //     },
            //     new ()
            //     {
            //         CardID = ECardID.MoveUnEnemy,
            //         FuneIDs = new List<int>()
            //         {
            //         }
            //     },
            //     new ()
            //     {
            //         CardID = ECardID.MoveUnEnemy,
            //         FuneIDs = new List<int>()
            //         {
            //         }
            //     },
            //     new ()
            //     {
            //         CardID = ECardID.MoveGrid,
            //         FuneIDs = new List<int>()
            //         {
            //         }
            //     },
            //
            //     new ()
            //     {
            //         CardID = ECardID.MoveGrid,
            //         FuneIDs = new List<int>()
            //         {
            //         }
            //     },
            //     new ()
            //     {
            //         CardID = ECardID.MoveGrid,
            //         FuneIDs = new List<int>()
            //         {
            //         }
            //     },
            //     new ()
            //     {
            //         CardID = ECardID.Move_Attack_BePass,
            //         FuneIDs = new List<int>()
            //         {
            //         }
            //     },
            //     new ()
            //     {
            //         CardID = ECardID.Move_Attack_BePass,
            //         FuneIDs = new List<int>()
            //         {
            //         }
            //     },
            //     new ()
            //     {
            //         CardID = ECardID.Move_Attack_BePass,
            //         FuneIDs = new List<int>()
            //         {
            //         }
            //     },
            //     // new ()
            //     // {
            //     //     CardID = ECardID.MoveGrid,
            //     //     FuneIDs = new List<int>()
            //     //     {
            //     //     }
            //     // },
            //     // new ()
            //     // {
            //     //     CardID = ECardID.MoveGrid,
            //     //     FuneIDs = new List<int>()
            //     //     {
            //     //     }
            //     // },
            //     // new ()
            //     // {
            //     //     CardID = ECardID.MoveGrid,
            //     //     FuneIDs = new List<int>()
            //     //     {
            //     //     }
            //     // },
            // };
            
            public static int MaxHP = 5;
            public static int MaxHeart = 3;
            public static int MaxEnergy = 5;
            public static int RecoverEnergy = 5;
            
        }
        
        public static class Card
        {
            public static List<EBuffID> EffectMultiUnitsBuffIDs = new ()
            {
                // EBuffID.DeBuffCountDamage,
                // EBuffID.BuffUsAddCurHP,
                // EBuffID.FullHPUsAddDamage,
                // EBuffID.HurtEachMove_HurtRoundStart,
                // EBuffID.HurtRoundStart_HurtEachMove,
                // EBuffID.UnMoveAroundHeroUnit,
                // EBuffID.UnAttackAroundHeroUnit,
                // EBuffID.LessHalfHPEnemyHurtAddDamge,
                // EBuffID.FullHPUsAddDamage,
                // EBuffID.MoreHalfHPEnemySubDamge,
                // EBuffID.AttackPassEnemyAddDamage_AttackPassUsAddDamage,
                // EBuffID.HurtSubDamageAddHeroCurHP,
                // EBuffID.RoundCounterAttackAddDamage,
                // //ECardID.CurHP1UsDodgeShield,
                // EBuffID.RoundDeBuffUnEffect,
                // EBuffID.RoundCurseUnEffect,
                
            };
            
            // public static List<ECardID> DamageTacticCard = new List<ECardID>()
            // {
            //     ECardID.UnitCountDamage,
            //     
            //     
            // };
            public static int MaxFuneCount = 3;
            public static int InitFuneCount = 2;
        }

        public static class BattleEvent
        {
            public static int RandomItemCount = 3;
            
            public static Dictionary<EBattleEventExpressionType, int> EventExpressionTypes = new()
            {
                [EBattleEventExpressionType.Selection] = 100,
                [EBattleEventExpressionType.Game] = 0,

            };
            
            public static Dictionary<ERandomType, int> ERandomTypes = new()
            {
                [ERandomType.Event] = 70,
                [ERandomType.Store] = 10,
                [ERandomType.Rest] = 10,
                [ERandomType.NormalBattle] = 10,

            };
            
            public static Dictionary<EBattleEventYNType, int> EEventYNTypes = new()
            {
                [EBattleEventYNType.Y] = 15,
                [EBattleEventYNType.N] = 15,
                [EBattleEventYNType.YN] = 70,

            };

            public static Dictionary<EBattleEventYNType, List<EBattleEvent>> BattleGameEventYNTypes = new()
            {
                [EBattleEventYNType.Y] = new List<EBattleEvent>()
                {
                    EBattleEvent.Line_Y_O_Y,
                    EBattleEvent.Line_Y_N_Y,
                    EBattleEvent.QuickClick_O_Y_O,
                    EBattleEvent.QuickClick_Y_Y_Y,
                    EBattleEvent.QuickSelect_Y_O,
                },
                [EBattleEventYNType.N] = new List<EBattleEvent>()
                {
                    EBattleEvent.Line_N_O_N,
                    EBattleEvent.Line_N_Y_N,
                    EBattleEvent.QuickClick_N_Y_N,
                    EBattleEvent.QuickClick_N_O_N,
                    EBattleEvent.QuickSelect_O_N,
                },
                [EBattleEventYNType.YN] = new List<EBattleEvent>()
                {
                    EBattleEvent.Line_YN_O_YN,
                    EBattleEvent.Time_O_N_Y_O,
                    EBattleEvent.QuickClick_YN_YN_YN,
                    EBattleEvent.QuickSelect_Y_N,

                },
            };
            
            
            public static Dictionary<EBattleEventYNType, List<EBattleEvent>> BattleTreasureTypes = new()
            {

                [EBattleEventYNType.YN] = new List<EBattleEvent>()
                {
                    EBattleEvent.Select_Y_Y_O,

                },
            };
            public static Dictionary<EBattleEventYNType, List<EBattleEvent>> BattleSelectEventYNTypes = new()
            {
                [EBattleEventYNType.Y] = new List<EBattleEvent>()
                {

                    EBattleEvent.Select_Y_Y_O,
                },
                [EBattleEventYNType.N] = new List<EBattleEvent>()
                {

                    EBattleEvent.Select_N_N,
                },
                [EBattleEventYNType.YN] = new List<EBattleEvent>()
                {
                    EBattleEvent.Select_YN_YN_O,

                },
            };

            public static Dictionary<EBattleEventYNType, Dictionary<EEventType, Vector2Int>> BattleEventValues =
                new ()
                {
                    [EBattleEventYNType.Y] = new ()
                    {
                        [EEventType.AddCoin] = new Vector2Int(50, 100),
                        [EEventType.AddHeroMaxHP] = new Vector2Int(1, 2),
                        [EEventType.AddHeroCurHP] = new Vector2Int(2, 4),
                    },

                    [EBattleEventYNType.N] = new ()
                    {
                        [EEventType.SubCoin] = new Vector2Int(-100, -50),
                        [EEventType.SubHeroMaxHP] = new Vector2Int(-2, -1),
                        [EEventType.SubHeroCurHP] = new Vector2Int(-4, -2),
                    },

                    [EBattleEventYNType.YN] = new ()
                    {
                        [EEventType.AddCoin] = new Vector2Int(100, 150),
                        [EEventType.AddHeroMaxHP] = new Vector2Int(2, 4),
                        [EEventType.AddHeroCurHP] = new Vector2Int(4, 8),

                        [EEventType.SubCoin] = new Vector2Int(-150, -100),
                        [EEventType.SubHeroMaxHP] = new Vector2Int(-4, -2),
                        [EEventType.SubHeroCurHP] = new Vector2Int(-8, -4),
                    }
                };
            
            public static Dictionary<EBattleEventYNType, List<EEventType>> BattleEventYNTypes = new()
            {
                [EBattleEventYNType.Y] = new List<EEventType>()
                {
                    EEventType.Card_Remove,
                    EEventType.Card_Change,
                    EEventType.Card_Copy,
                    
                    EEventType.Random_UnitCard,
                    EEventType.Random_TacticCard,
                    EEventType.Random_Fune,
                    EEventType.Random_Bless,
                    
                    EEventType.Appoint_UnitCard,
                    EEventType.Appoint_TacticCard,
                    EEventType.Appoint_Fune,
                    EEventType.Appoint_Bless,
                    
                    EEventType.AddCoin,
                    EEventType.AddHeroMaxHP,
                    EEventType.AddHeroCurHP,
                },
                [EBattleEventYNType.N] = new List<EEventType>()
                {
                    EEventType.NegativeCard,
                    EEventType.SubCoin,
                    EEventType.SubHeroMaxHP,
                    EEventType.SubHeroCurHP,
                },
                [EBattleEventYNType.YN] = new List<EEventType>()
                {
                    EEventType.Card_Remove,
                    EEventType.Card_Change,
                    EEventType.Card_Copy,

                    EEventType.Appoint_UnitCard,
                    EEventType.Appoint_TacticCard,
                    EEventType.Appoint_Fune,
                    EEventType.Appoint_Bless,
                    
                    EEventType.AddCoin,
                    EEventType.AddHeroMaxHP,
                    EEventType.AddHeroCurHP,
                    
                    EEventType.NegativeCard,
                    EEventType.SubCoin,
                    EEventType.SubHeroMaxHP,
                    EEventType.SubHeroCurHP,
                },
                

            };

            public static Dictionary<EBattleEventYNType, List<EEventType>>
                BattleEventYNTypes2 = new()
                {
                    [EBattleEventYNType.Y] = new List<EEventType>()
                    {
                        EEventType.Card_Remove,
                        EEventType.Card_Change,
                        EEventType.Card_Copy,

                        EEventType.Appoint_UnitCard,
                        EEventType.Appoint_TacticCard,
                        EEventType.Appoint_Fune,
                        EEventType.Appoint_Bless,

                        EEventType.AddCoin,
                        EEventType.AddHeroMaxHP,
                        EEventType.AddHeroCurHP,
                    },

                    [EBattleEventYNType.N] = new List<EEventType>()
                    {
                        EEventType.NegativeCard,
                        EEventType.SubCoin,
                        EEventType.SubHeroMaxHP,
                        EEventType.SubHeroCurHP,
                    },

                };
            
            public static Dictionary<EEventSubType, List<EEventType>> BattleEventSubTypes = new()
            {
                [EEventSubType.Random] = new List<EEventType>()
                {
                    EEventType.Random_UnitCard,
                    EEventType.Random_TacticCard,
                    EEventType.Random_Fune,
                    EEventType.Random_Bless,
                },
                [EEventSubType.Appoint] = new List<EEventType>()
                {

                    EEventType.Appoint_UnitCard,
                    EEventType.Appoint_TacticCard,
                    EEventType.Appoint_Fune,
                    EEventType.Appoint_Bless,

                },
                [EEventSubType.Value] = new List<EEventType>()
                {
                    EEventType.AddCoin,
                    EEventType.AddHeroMaxHP,
                    EEventType.AddHeroCurHP,
                    EEventType.SubCoin,
                    EEventType.SubHeroMaxHP,
                    EEventType.SubHeroCurHP,
                },
            };

            public static Dictionary<EItemType, List<EEventType>> ItemTypeAppointEventTypeMap = new()
            {
                [EItemType.TacticCard] = new List<EEventType>()
                {
                    //EEventType.Random_UnitCard,
                    //EEventType.Random_TacticCard,
                    EEventType.Appoint_TacticCard,
                    EEventType.NegativeCard,
                },
                [EItemType.UnitCard] = new List<EEventType>()
                {
                    //EEventType.Random_UnitCard,
                    //EEventType.Random_TacticCard,
                    EEventType.Appoint_UnitCard,
                    EEventType.NegativeCard,
                },
                [EItemType.Fune] = new List<EEventType>()
                {
                    //EEventType.Random_Fune,
                    EEventType.Appoint_Fune,

                },
                [EItemType.Bless] = new List<EEventType>()
                {
                    //EEventType.Random_Bless,
                    EEventType.Appoint_Bless,

                },
                


            };
            
            public static Dictionary<EItemType, List<EEventType>> ItemTypeEventTypeMap = new()
            {
                [EItemType.TacticCard] = new List<EEventType>()
                {
                    
                    EEventType.Random_TacticCard,
                    EEventType.Appoint_TacticCard,
                    EEventType.NegativeCard,
                    EEventType.Card_Change,
                    EEventType.Card_Copy,
                    

                },
                [EItemType.UnitCard] = new List<EEventType>()
                {
                    EEventType.Random_UnitCard,
                    EEventType.Appoint_UnitCard,
                    EEventType.NegativeCard,
                    EEventType.Card_Change,
                    EEventType.Card_Copy,
                    

                },
                [EItemType.Fune] = new List<EEventType>()
                {
                    EEventType.Random_Fune,
                    EEventType.Appoint_Fune,

                },
                [EItemType.Bless] = new List<EEventType>()
                {
                    EEventType.Random_Bless,
                    EEventType.Appoint_Bless,

                },
                [EItemType.Coin] = new List<EEventType>()
                {
                    EEventType.AddCoin,
                    EEventType.SubCoin,
                },
                [EItemType.AddMaxHP] = new List<EEventType>()
                {
                    //EEventType.Random_Bless,
                    EEventType.AddHeroCurHP,
                    EEventType.AddHeroMaxHP,
                    EEventType.SubHeroCurHP,
                    EEventType.SubHeroMaxHP,
                },
                


            };
            
            // public static Dictionary<EItemType, List<EEventType>> ItemTypeEventTypeMap2 = new()
            // {
            //     [EItemType.Card] = new List<EEventType>()
            //     {
            //         EEventType.Random_UnitCard,
            //         EEventType.Random_TacticCard,
            //         EEventType.Appoint_UnitCard,
            //         EEventType.Appoint_TacticCard,
            //         EEventType.NegativeCard,
            //     },
            //     [EItemType.Fune] = new List<EEventType>()
            //     {
            //         EEventType.Random_Fune,
            //         EEventType.Appoint_Fune,
            //
            //     },
            //     [EItemType.Bless] = new List<EEventType>()
            //     {
            //         EEventType.Random_Bless,
            //         EEventType.Appoint_Bless,
            //
            //     },
            //
            //
            // };
            
            public static Dictionary<EEventType, EItemType> AppointEventTypeItemTypeMap = new();
            public static Dictionary<EEventType, EItemType> EventTypeItemTypeMap = new();
        }

        public class EGridPropEffectValueTemplate
        {
            public List<int> Values;
            public List<EUnitState> UnitStates;
        }

        public static List<EGridPropEffectValueTemplate> GridPropEffectValues = new List<EGridPropEffectValueTemplate>()
        {
            new EGridPropEffectValueTemplate()
            {
                Values = new List<int>()
                {
                    4,2,1
                },
                UnitStates = new List<EUnitState>()
                {
                    EUnitState.HurtRoundStart,//点燃：回合伤害
                    EUnitState.HurtEachMove,//中毒：移动伤害
                    EUnitState.AtkPassUs,//错乱：经过撞击、撞击经过的己方单位
                    EUnitState.HurtAddDmg,//虚弱：受击增加伤害
                    EUnitState.SubDmg,//无力：减少输出
                    EUnitState.AtkPassEnemy,//愤怒：经过撞击、撞击经过的敌方单位
                    EUnitState.HurtSubDmg,//格挡：受击减少伤害
                    EUnitState.AddDmg,//强化：增加输出
                    EUnitState.CounterAtk,//反击：
                    EUnitState.UnEffectLink,//干扰：阻断周围敌军的主动、被动联动，
                    EUnitState.UnBePass,//巨大化：无法被经过
                    EUnitState.CollideUnHurt,//坚硬：碰撞不会受伤
                }
            },
            new EGridPropEffectValueTemplate()
            {
                Values = new List<int>()
                {
                    2,1,1
                },
                UnitStates = new List<EUnitState>()
                {
                    EUnitState.UnMove,//缠绕：无法移动
                    EUnitState.UnAtk,//沉默：无法攻击
                    EUnitState.UnHurt,//无敌：不受伤
                    EUnitState.DoubleDmg,//暴击：双倍伤害
                    EUnitState.AtkAddSelfHP,//吸血：攻击时，恢复生命
                    EUnitState.BuffUnEffect,//压制：正面Buff不生效
                    EUnitState.DeBuffUnEffect,//隔离：负面Buff不生效
                }
            }

        };

        
        
        public static void Init()
        {
            foreach (var kv in BattleEvent.ItemTypeAppointEventTypeMap)
            {
                foreach (var eventType in kv.Value)
                {
                    if(!BattleEvent.AppointEventTypeItemTypeMap.ContainsKey(eventType))
                        BattleEvent.AppointEventTypeItemTypeMap.Add(eventType, kv.Key);
                }
            }
            
            foreach (var kv in BattleEvent.ItemTypeEventTypeMap)
            {
                foreach (var eventType in kv.Value)
                {
                    if(!BattleEvent.EventTypeItemTypeMap.ContainsKey(eventType))
                        BattleEvent.EventTypeItemTypeMap.Add(eventType, kv.Key);
                }
            }
        }
    }
}