//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：__DATA_TABLE_CREATE_TIME__
//------------------------------------------------------------

using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    /// <summary>
    /// 敌人。
    /// </summary>
    public class DREnemyGenerateRule : DataRowBase
    {
        private int m_Id = 0;

        /// <summary>
        /// 获取Id。
        /// </summary>
        public override int Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// 获取游戏难度。
        /// </summary>
        public EGameDifficulty GameDifficulty
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取战斗场次。
        /// </summary>
        public int BattleSession
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取回合生成敌人数量。
        /// </summary>
        public string RoundGenerateUnitCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取不同等级敌人数量。
        /// </summary>
        public string EnemyLevelCounts
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取不同类型敌人数量。
        /// </summary>
        public string EnemyTypeCounts
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取每回合敌人数量。
        /// </summary>
        public int EachRoundUnitCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取全局DeBuff数量。
        /// </summary>
        public int GlobalDebuffCount
        {
            get;
            private set;
        }

        public override bool ParseDataRow(string dataRowString, object userData)
        {
            string[] columnStrings = dataRowString.Split(DataTableExtension.DataSplitSeparators);
            for (int i = 0; i < columnStrings.Length; i++)
            {
                columnStrings[i] = columnStrings[i].Trim(DataTableExtension.DataTrimSeparators);
            }

            int index = 0;
            index++;
            m_Id = int.Parse(columnStrings[index++]);
            index++;
			GameDifficulty = Enum.Parse<EGameDifficulty>(columnStrings[index++]);
            BattleSession = int.Parse(columnStrings[index++]);
            RoundGenerateUnitCount = columnStrings[index++];
            EnemyLevelCounts = columnStrings[index++];
            EnemyTypeCounts = columnStrings[index++];
            EachRoundUnitCount = int.Parse(columnStrings[index++]);
            GlobalDebuffCount = int.Parse(columnStrings[index++]);

            GeneratePropertyArray();
            return true;
        }

        public override bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length, object userData)
        {
            using (MemoryStream memoryStream = new MemoryStream(dataRowBytes, startIndex, length, false))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
                {
                    m_Id = binaryReader.Read7BitEncodedInt32();
                    GameDifficulty = Enum.Parse<EGameDifficulty>(binaryReader.ReadString());
                    BattleSession = binaryReader.Read7BitEncodedInt32();
                    RoundGenerateUnitCount = binaryReader.ReadString();
                    EnemyLevelCounts = binaryReader.ReadString();
                    EnemyTypeCounts = binaryReader.ReadString();
                    EachRoundUnitCount = binaryReader.Read7BitEncodedInt32();
                    GlobalDebuffCount = binaryReader.Read7BitEncodedInt32();
                }
            }

            GeneratePropertyArray();
            return true;
        }

        private void GeneratePropertyArray()
        {

        }
    }
}
