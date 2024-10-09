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
    /// 符文表。
    /// </summary>
    public class DRBuff : DataRowBase
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
        /// 获取BuffIDs。
        /// </summary>
        public List<string> BuffIDs
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取BuffValues。
        /// </summary>
        public List<string> BuffValues
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取BuffTypes。
        /// </summary>
        public List<EBuffType> BuffTypes
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
			BuffIDs = DataTableExtension.ParseStringList(columnStrings[index++]);
			BuffValues = DataTableExtension.ParseStringList(columnStrings[index++]);
			BuffTypes = DataTableExtension.ParseEBuffTypeList(columnStrings[index++]);

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
					BuffIDs = binaryReader.ReadStringList();
					BuffValues = binaryReader.ReadStringList();
					BuffTypes = binaryReader.ReadEBuffTypeList();
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
