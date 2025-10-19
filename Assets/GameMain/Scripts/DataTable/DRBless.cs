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
    /// 祝福表。
    /// </summary>
    public class DRBless : DataRowBase
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
        /// 获取BlessID。
        /// </summary>
        public EBlessID BlessID
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取值。
        /// </summary>
        public List<string> Values0
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取叠加。
        /// </summary>
        public bool Overlay
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取ExplainItems。
        /// </summary>
        public List<string> ExplainItems
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
			BlessID = Enum.Parse<EBlessID>(columnStrings[index++]);
			Values0 = DataTableExtension.ParseStringList(columnStrings[index++]);
            Overlay = bool.Parse(columnStrings[index++]);
			ExplainItems = DataTableExtension.ParseStringList(columnStrings[index++]);

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
                    BlessID = Enum.Parse<EBlessID>(binaryReader.ReadString());
					Values0 = binaryReader.ReadStringList();
                    Overlay = binaryReader.ReadBoolean();
					ExplainItems = binaryReader.ReadStringList();
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
                new KeyValuePair<int, List<string>>(0, Values0),
            };
        }
    }
}
