using System.Collections.Generic;
using UnityEngine;

namespace RoundHero
{
    public static partial class Constant
    {
        public static class Area
        {
            public static Vector2Int GridSize = new Vector2Int(7, 7);
            public static int ObstacleCount = 5;
            public static int MaxRoleInGrid = 1;

            public static Vector2 GridInterval = new Vector2()
            {
                x = 0.25f,
                y = 0.25f,
            };

            public static Vector2 GridLength = new Vector2()
            {
                x = 2f,
                y = 2f,
            };

            public static Vector2 GridRange = new Vector2()
            {
                x = GridLength.x + GridInterval.x,
                y = GridLength.y + GridInterval.y,
            };

        }

        public static class Battle
        {
            public static List<ERelativeCamp> AllRelativeCamps = new List<ERelativeCamp>()
            {
                ERelativeCamp.Enemy,
                ERelativeCamp.Us
            };
            
            public static float CardPosInterval = 165;
            public static float ViewMaxHandCardCount = 10;
            public static int EachHardCardCount = 5;

            public static int InitCardMaxCount = 12;

            public static Dictionary<EUnitStateEffectType, List<EUnitState>> EffectUnitStates =
                new ()
                {
                    [EUnitStateEffectType.Positive] = new()
                    {
                        EUnitState.DeBuffUnEffect,
                        //EUnitState.Dodge,
                        EUnitState.AtkPassEnemy,
                        EUnitState.CounterAtk,
                        EUnitState.AddDmg,
                        EUnitState.HurtSubDmg,
                    },
                    [EUnitStateEffectType.Negative] = new()
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
                            new Vector2Int(-1, 0),
                            new Vector2Int(-2, 0),
                            new Vector2Int(-3, 0),
                            new Vector2Int(-4, 0),
                            new Vector2Int(-5, 0),
                            new Vector2Int(-6, 0),
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
                            new Vector2Int(0, 1),
                            new Vector2Int(0, 2),
                            new Vector2Int(0, 3),
                            new Vector2Int(0, 4),
                            new Vector2Int(0, 5),
                            new Vector2Int(0, 6),
                        },
                    },
                    
                    [EActionType.Cross_Short] = new List<List<Vector2Int>>()
                    {
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 0),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(-1, 0),

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
                            new Vector2Int(0, 1),

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
                            new Vector2Int(-1, 0),
                            new Vector2Int(-2, 0),
                            new Vector2Int(-3, 0),
                            new Vector2Int(-4, 0),
                            new Vector2Int(-5, 0),
                            new Vector2Int(-6, 0),
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
                            new Vector2Int(0, 1),
                            new Vector2Int(0, 2),
                            new Vector2Int(0, 3),
                            new Vector2Int(0, 4),
                            new Vector2Int(0, 5),
                            new Vector2Int(0, 6),
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
                            new Vector2Int(-1, 0),
                            new Vector2Int(-2, 0),
                            new Vector2Int(-3, 0),
                            new Vector2Int(-4, 0),
                            new Vector2Int(-5, 0),
                            new Vector2Int(-6, 0),
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
                            new Vector2Int(0, 1),
                            new Vector2Int(0, 2),
                            new Vector2Int(0, 3),
                            new Vector2Int(0, 4),
                            new Vector2Int(0, 5),
                            new Vector2Int(0, 6),
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
                
                    [EActionType.Around] = new List<List<Vector2Int>>()
                    {
                        new List<Vector2Int>()
                        {
                            new Vector2Int(0, 0),
                        },
                        new List<Vector2Int>()
                        {
                            new Vector2Int(-1, 0),
                            new Vector2Int(-1, 1),
                            new Vector2Int(0, 1),
                            new Vector2Int(1, 1),
                            new Vector2Int(1, 0),
                            new Vector2Int(1, -1),
                            new Vector2Int(0, -1),
                            new Vector2Int(-1, -1),
                        },
                    },
                    
                    [EActionType.Direct8] = new List<List<Vector2Int>>()
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
                            new Vector2Int(-1, 0),
                            new Vector2Int(-2, 0),
                            new Vector2Int(-3, 0),
                            new Vector2Int(-4, 0),
                            new Vector2Int(-5, 0),
                            new Vector2Int(-6, 0),
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
                            new Vector2Int(0, 1),
                            new Vector2Int(0, 2),
                            new Vector2Int(0, 3),
                            new Vector2Int(0, 4),
                            new Vector2Int(0, 5),
                            new Vector2Int(0, 6),
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
                            new Vector2Int(1, 1),
                            new Vector2Int(2, 2),
                            new Vector2Int(3, 3),
                            new Vector2Int(4, 4),
                            new Vector2Int(5, 5),
                            new Vector2Int(6, 6),
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
                        new List<Vector2Int>()
                        {
                            new Vector2Int(1, -1),
                            new Vector2Int(2, -2),
                            new Vector2Int(3, -3),
                            new Vector2Int(4, -4),
                            new Vector2Int(5, -5),
                            new Vector2Int(6, -6),
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
                    {

                    },
                    
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
                [EUnitActionState.Run] = 0.7f,
                [EUnitActionState.Fly] = 0.7f,

            };
        }

        public static class Enemy
        {
            public static Dictionary<EEnemyType, int> EachTurnGenerateEnemyCounts = new()
            {
                [EEnemyType.Normal] = 3,
                [EEnemyType.Elite] = 1,
                [EEnemyType.Boss] = 1,

            };

            public static Dictionary<EEnemyType, int> EnemyGenerateTurns = new()
            {
                [EEnemyType.Normal] = 1,
                [EEnemyType.Elite] = 1,
                [EEnemyType.Boss] = 3,

            };

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
            public List<int> InitBlesses = new List<int>();
            public List<int> InitFunes = new List<int>();
            public List<InitCardData> InitCards = new();
        }

        public static class Hero
        {
            public static Dictionary<EUnitCamp, InitData> InitDatas = new()
            {
                [EUnitCamp.Player1] = new InitData()
                {
                    InitBlesses = new List<int>()
                    {
                        0,
                        1,
                        2,
                        3,
                    },
                    
                    InitFunes = new List<int>()
                    {
                        0,
                        1,
                        2,
                        2,
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
                                //0
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
                        [EEventType.SubHeroMaxHP] = new Vector2Int(-1, -2),
                        [EEventType.SubHeroCurHP] = new Vector2Int(-2, -4),
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
                    EUnitState.SubHPAddSelfHP,//吸血：攻击时，恢复生命
                    EUnitState.BuffUnEffect,//压制：正面Buff不生效
                    EUnitState.DeBuffUnEffect,//隔离：负面Buff不生效
                }
            }

        };
    }
}