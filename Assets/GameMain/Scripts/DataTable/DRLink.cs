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
    /// Link表。
    /// </summary>
    public class DRLink : DataRowBase
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
        /// 获取LinkID。
        /// </summary>
        public ELinkID LinkID
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取联动类型。
        /// </summary>
        public ELinkType LinkType
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取联动范围。
        /// </summary>
        public EActionType LinkRange
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取LinkUnitCamps。
        /// </summary>
        public List<ERelativeCamp> LinkUnitCamps
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
			LinkID = Enum.Parse<ELinkID>(columnStrings[index++]);
			LinkType = Enum.Parse<ELinkType>(columnStrings[index++]);
			LinkRange = Enum.Parse<EActionType>(columnStrings[index++]);
			LinkUnitCamps = DataTableExtension.ParseERelativeCampList(columnStrings[index++]);

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
                    LinkID = Enum.Parse<ELinkID>(binaryReader.ReadString());
                    LinkType = Enum.Parse<ELinkType>(binaryReader.ReadString());
                    LinkRange = Enum.Parse<EActionType>(binaryReader.ReadString());
					LinkUnitCamps = binaryReader.ReadERelativeCampList();
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
