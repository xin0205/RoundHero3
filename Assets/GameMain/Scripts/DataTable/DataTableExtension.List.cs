using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
namespace RoundHero
{
	public static partial class DataTableExtension
	{
		public static List<bool> ParseBooleanList(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split(',');
			List<bool> list = new List<bool>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(Boolean.Parse(splitValue[i]));
			}
			return list;
		}
		public static List<byte> ParseByteList(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split(',');
			List<byte> list = new List<byte>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(Byte.Parse(splitValue[i]));
			}
			return list;
		}
		public static List<char> ParseCharList(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split(',');
			List<char> list = new List<char>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(Char.Parse(splitValue[i]));
			}
			return list;
		}
		public static List<Color32> ParseColor32List(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split('|');
			List<Color32> list = new List<Color32>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(ParseColor32(splitValue[i]));
			}
			return list;
		}
		public static List<Color> ParseColorList(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split('|');
			List<Color> list = new List<Color>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(ParseColor(splitValue[i]));
			}
			return list;
		}
		public static List<DateTime> ParseDateTimeList(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split(',');
			List<DateTime> list = new List<DateTime>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(DateTime.Parse(splitValue[i]));
			}
			return list;
		}
		public static List<decimal> ParseDecimalList(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split(',');
			List<decimal> list = new List<decimal>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(Decimal.Parse(splitValue[i]));
			}
			return list;
		}
		public static List<double> ParseDoubleList(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split(',');
			List<double> list = new List<double>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(Double.Parse(splitValue[i]));
			}
			return list;
		}
		public static List<float> ParseSingleList(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split(',');
			List<float> list = new List<float>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(Single.Parse(splitValue[i]));
			}
			return list;
		}
		public static List<int> ParseInt32List(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split(',');
			List<int> list = new List<int>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(Int32.Parse(splitValue[i]));
			}
			return list;
		}
		public static List<long> ParseInt64List(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split(',');
			List<long> list = new List<long>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(Int64.Parse(splitValue[i]));
			}
			return list;
		}
		public static List<Quaternion> ParseQuaternionList(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split('|');
			List<Quaternion> list = new List<Quaternion>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(ParseQuaternion(splitValue[i]));
			}
			return list;
		}
		public static List<Rect> ParseRectList(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split('|');
			List<Rect> list = new List<Rect>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(ParseRect(splitValue[i]));
			}
			return list;
		}
		public static List<sbyte> ParseSByteList(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split(',');
			List<sbyte> list = new List<sbyte>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(SByte.Parse(splitValue[i]));
			}
			return list;
		}
		public static List<short> ParseInt16List(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split(',');
			List<short> list = new List<short>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(Int16.Parse(splitValue[i]));
			}
			return list;
		}
		public static List<string> ParseStringList(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split(',');
			List<string> list = new List<string>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(splitValue[i]);
			}
			return list;
		}
		public static List<uint> ParseUInt32List(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split(',');
			List<uint> list = new List<uint>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(UInt32.Parse(splitValue[i]));
			}
			return list;
		}
		public static List<ulong> ParseUInt64List(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split(',');
			List<ulong> list = new List<ulong>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(UInt64.Parse(splitValue[i]));
			}
			return list;
		}
		public static List<ushort> ParseUInt16List(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split(',');
			List<ushort> list = new List<ushort>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(UInt16.Parse(splitValue[i]));
			}
			return list;
		}
		public static List<Vector2> ParseVector2List(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split('|');
			List<Vector2> list = new List<Vector2>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(ParseVector2(splitValue[i]));
			}
			return list;
		}
		
		public static List<Vector2Int> ParseVector2IntList(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split('|');
			List<Vector2Int> list = new List<Vector2Int>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(ParseVector2Int(splitValue[i]));
			}
			return list;
		}
		
		public static List<Vector3> ParseVector3List(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split('|');
			List<Vector3> list = new List<Vector3>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(ParseVector3(splitValue[i]));
			}
			return list;
		}
		
		public static List<EBuffID> ParseEBuffIDList(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split(',');
			var list = new List<EBuffID>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(Enum.Parse<EBuffID>(splitValue[i]));
			}
			return list;
		}
		
		// public static List<ECardID> ParseEFightCardIDList(string value)
		// {
		// 	if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
		// 		return null;
		// 	string[] splitValue = value.Split(',');
		// 	var list = new List<ECardID>(splitValue.Length);
		// 	for (int i = 0; i < splitValue.Length; i++)
		// 	{
		// 		list.Add(Enum.Parse<ECardID>(splitValue[i]));
		// 	}
		// 	return list;
		// }
		
		public static List<EActionType> ParseEActionTypeList(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split(',');
			var list = new List<EActionType>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(Enum.Parse<EActionType>(splitValue[i]));
			}
			return list;
		}
		
		public static List<EUnitRole> ParseEUnitRoleList(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split(',');
			var list = new List<EUnitRole>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(Enum.Parse<EUnitRole>(splitValue[i]));
			}
			return list;
		}
		
		public static List<EUnitCamp> ParseEUnitCampList(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split(',');
			var list = new List<EUnitCamp>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(Enum.Parse<EUnitCamp>(splitValue[i]));
			}
			return list;
		}
		
		public static List<ERelativeCamp> ParseERelativeCampList(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split(',');
			var list = new List<ERelativeCamp>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(Enum.Parse<ERelativeCamp>(splitValue[i]));
			}
			return list;
		}
		
		public static List<ETriggerTarget> ParseETriggerTargetList(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split(',');
			var list = new List<ETriggerTarget>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(Enum.Parse<ETriggerTarget>(splitValue[i]));
			}
			return list;
		}
		
		public static List<EBuffType> ParseEBuffTypeList(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split(',');
			var list = new List<EBuffType>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(Enum.Parse<EBuffType>(splitValue[i]));
			}
			return list;
		}
		
		
		
		// public static List<ECardID> ParseECardIDList(string value)
		// {
		// 	if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
		// 		return null;
		// 	string[] splitValue = value.Split(',');
		// 	var list = new List<ECardID>(splitValue.Length);
		// 	for (int i = 0; i < splitValue.Length; i++)
		// 	{
		// 		list.Add(Enum.Parse<ECardID>(splitValue[i]));
		// 	}
		// 	return list;
		// }
		
		public static List<EAttackTarget> ParseEAttackTargetList(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split(',');
			var list = new List<EAttackTarget>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(Enum.Parse<EAttackTarget>(splitValue[i]));
			}
			return list;
		}
		
		// public static List<Test.TestEnum> ParseTestTestEnumList(string Value)
		// {
		// 	if (string.IsNullOrEmpty(Value) || Value.ToLowerInvariant().Equals("null"))
		// 		return null;
		// 	string[] splitValue = Value.Split(',');
		// 	List<Test.TestEnum> list = new List<Test.TestEnum>(splitValue.Length);
		// 	for (int i = 0; i < splitValue.Length; i++)
		// 	{
		// 		list.Add(EnumParse<Test.TestEnum>(splitValue[i]));
		// 	}
		// 	return list;
		// }
	}
}
