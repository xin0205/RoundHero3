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
    /// 卡表。
    /// </summary>
    public class DRTacticCard : DataRowBase
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
        /// 获取CardID。
        /// </summary>
        public string CardID
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取值。
        /// </summary>
        public List<string> Values1
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取值2。
        /// </summary>
        public List<string> Values2
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取能量。
        /// </summary>
        public int Energy
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取生命。
        /// </summary>
        public int HP
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取移动类型。
        /// </summary>
        public EActionType MoveType
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
            CardID = columnStrings[index++];
			Values1 = DataTableExtension.ParseStringList(columnStrings[index++]);
			Values2 = DataTableExtension.ParseStringList(columnStrings[index++]);
            Energy = int.Parse(columnStrings[index++]);
            HP = int.Parse(columnStrings[index++]);
			MoveType = Enum.Parse<EActionType>(columnStrings[index++]);

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
                    CardID = binaryReader.ReadString();
					Values1 = binaryReader.ReadStringList();
					Values2 = binaryReader.ReadStringList();
                    Energy = binaryReader.Read7BitEncodedInt32();
                    HP = binaryReader.Read7BitEncodedInt32();
                    MoveType = Enum.Parse<EActionType>(binaryReader.ReadString());
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
