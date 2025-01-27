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
    public class DREnemy : DataRowBase
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
        /// 获取移动类型。
        /// </summary>
        public EActionType MoveType
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
        /// 获取OwnBuffs。
        /// </summary>
        public List<string> OwnBuffs
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取Values1。
        /// </summary>
        public List<string> OwnBuffValues1
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取SecondaryBuffs。
        /// </summary>
        public List<string> SecondaryBuffs
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取Values2。
        /// </summary>
        public List<string> SecondaryValues
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取PassiveBuffs。
        /// </summary>
        public List<string> PassiveBuffs
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取PassiveBuffValues。
        /// </summary>
        public List<string> PassiveBuffValues
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
        /// 获取攻击表现类型。
        /// </summary>
        public EAttackCastType AttackCastType
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取AttackTargets。
        /// </summary>
        public List<EAttackTarget> AttackTargets
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取AttackType。
        /// </summary>
        public EEnemyAttackType AttackType
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
			MoveType = Enum.Parse<EActionType>(columnStrings[index++]);
            HP = int.Parse(columnStrings[index++]);
			OwnBuffs = DataTableExtension.ParseStringList(columnStrings[index++]);
			OwnBuffValues1 = DataTableExtension.ParseStringList(columnStrings[index++]);
			SecondaryBuffs = DataTableExtension.ParseStringList(columnStrings[index++]);
			SecondaryValues = DataTableExtension.ParseStringList(columnStrings[index++]);
			PassiveBuffs = DataTableExtension.ParseStringList(columnStrings[index++]);
			PassiveBuffValues = DataTableExtension.ParseStringList(columnStrings[index++]);
			WeaponHoldingType = Enum.Parse<EWeaponHoldingType>(columnStrings[index++]);
			WeaponType = Enum.Parse<EWeaponType>(columnStrings[index++]);
            WeaponID = int.Parse(columnStrings[index++]);
			AttackCastType = Enum.Parse<EAttackCastType>(columnStrings[index++]);
			AttackTargets = DataTableExtension.ParseEAttackTargetList(columnStrings[index++]);
			AttackType = Enum.Parse<EEnemyAttackType>(columnStrings[index++]);

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
                    MoveType = Enum.Parse<EActionType>(binaryReader.ReadString());
                    HP = binaryReader.Read7BitEncodedInt32();
					OwnBuffs = binaryReader.ReadStringList();
					OwnBuffValues1 = binaryReader.ReadStringList();
					SecondaryBuffs = binaryReader.ReadStringList();
					SecondaryValues = binaryReader.ReadStringList();
					PassiveBuffs = binaryReader.ReadStringList();
					PassiveBuffValues = binaryReader.ReadStringList();
                    WeaponHoldingType = Enum.Parse<EWeaponHoldingType>(binaryReader.ReadString());
                    WeaponType = Enum.Parse<EWeaponType>(binaryReader.ReadString());
                    WeaponID = binaryReader.Read7BitEncodedInt32();
                    AttackCastType = Enum.Parse<EAttackCastType>(binaryReader.ReadString());
					AttackTargets = binaryReader.ReadEAttackTargetList();
                    AttackType = Enum.Parse<EEnemyAttackType>(binaryReader.ReadString());
                }
            }

            GeneratePropertyArray();
            return true;
        }

        private KeyValuePair<int, List<string>>[] m_OwnBuffValues = null;

        public int OwnBuffValuesCount
        {
            get
            {
                return m_OwnBuffValues.Length;
            }
        }

        public List<string> GetOwnBuffValues(int id)
        {
            foreach (KeyValuePair<int, List<string>> i in m_OwnBuffValues)
            {
                if (i.Key == id)
                {
                    return i.Value;
                }
            }

            throw new GameFrameworkException(Utility.Text.Format("GetOwnBuffValues with invalid id '{0}'.", id));
        }

        public List<string> GetOwnBuffValuesAt(int index)
        {
            if (index < 0 || index >= m_OwnBuffValues.Length)
            {
                throw new GameFrameworkException(Utility.Text.Format("GetOwnBuffValuesAt with invalid index '{0}'.", index));
            }

            return m_OwnBuffValues[index].Value;
        }

        private void GeneratePropertyArray()
        {
            m_OwnBuffValues = new KeyValuePair<int, List<string>>[]
            {
                new KeyValuePair<int, List<string>>(1, OwnBuffValues1),
            };
        }
    }
}
