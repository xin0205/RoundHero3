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
    /// Hero表。
    /// </summary>
    public class DRHero : DataRowBase
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
        /// 获取HeroID。
        /// </summary>
        public EHeroID HeroID
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取HP。
        /// </summary>
        public int HP
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取Heart。
        /// </summary>
        public int Heart
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取Buffs。
        /// </summary>
        public List<string> Buffs
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取Values1。
        /// </summary>
        public List<string> Values1
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取Values2。
        /// </summary>
        public List<string> Values2
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取ActionType。
        /// </summary>
        public EActionType ActionType
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取武器持有类型。
        /// </summary>
        public EWeaponHoldingType WeaponHoldingType
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取武器类型。
        /// </summary>
        public EWeaponType WeaponType
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取武器ID。
        /// </summary>
        public int WeaponID
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取EnergyBuffIntervals。
        /// </summary>
        public List<int> EnergyBuffIntervals
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取EnergyBuffIDs。
        /// </summary>
        public List<string> EnergyBuffIDs
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取EnergyBuffValues。
        /// </summary>
        public List<string> EnergyBuffValues
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
			HeroID = Enum.Parse<EHeroID>(columnStrings[index++]);
            HP = int.Parse(columnStrings[index++]);
            Heart = int.Parse(columnStrings[index++]);
			Buffs = DataTableExtension.ParseStringList(columnStrings[index++]);
			Values1 = DataTableExtension.ParseStringList(columnStrings[index++]);
			Values2 = DataTableExtension.ParseStringList(columnStrings[index++]);
			ActionType = Enum.Parse<EActionType>(columnStrings[index++]);
			WeaponHoldingType = Enum.Parse<EWeaponHoldingType>(columnStrings[index++]);
			WeaponType = Enum.Parse<EWeaponType>(columnStrings[index++]);
            WeaponID = int.Parse(columnStrings[index++]);
			EnergyBuffIntervals = DataTableExtension.ParseInt32List(columnStrings[index++]);
			EnergyBuffIDs = DataTableExtension.ParseStringList(columnStrings[index++]);
			EnergyBuffValues = DataTableExtension.ParseStringList(columnStrings[index++]);

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
                    HeroID = Enum.Parse<EHeroID>(binaryReader.ReadString());
                    HP = binaryReader.Read7BitEncodedInt32();
                    Heart = binaryReader.Read7BitEncodedInt32();
					Buffs = binaryReader.ReadStringList();
					Values1 = binaryReader.ReadStringList();
					Values2 = binaryReader.ReadStringList();
                    ActionType = Enum.Parse<EActionType>(binaryReader.ReadString());
                    WeaponHoldingType = Enum.Parse<EWeaponHoldingType>(binaryReader.ReadString());
                    WeaponType = Enum.Parse<EWeaponType>(binaryReader.ReadString());
                    WeaponID = binaryReader.Read7BitEncodedInt32();
					EnergyBuffIntervals = binaryReader.ReadInt32List();
					EnergyBuffIDs = binaryReader.ReadStringList();
					EnergyBuffValues = binaryReader.ReadStringList();
                }
            }

            GeneratePropertyArray();
            return true;
        }

        private KeyValuePair<int, List<string>>[] m_Values = null;

        public int ValuesCount
        {
            get
            {
                return m_Values.Length;
            }
        }

        public List<string> GetValues(int id)
        {
            foreach (KeyValuePair<int, List<string>> i in m_Values)
            {
                if (i.Key == id)
                {
                    return i.Value;
                }
            }

            throw new GameFrameworkException(Utility.Text.Format("GetValues with invalid id '{0}'.", id));
        }

        public List<string> GetValuesAt(int index)
        {
            if (index < 0 || index >= m_Values.Length)
            {
                throw new GameFrameworkException(Utility.Text.Format("GetValuesAt with invalid index '{0}'.", index));
            }

            return m_Values[index].Value;
        }

        private void GeneratePropertyArray()
        {
            m_Values = new KeyValuePair<int, List<string>>[]
            {
                new KeyValuePair<int, List<string>>(1, Values1),
                new KeyValuePair<int, List<string>>(2, Values2),
            };
        }
    }
}
