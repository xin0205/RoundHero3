//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.IO;
using UnityEngine;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class Vector2IntProcessor : GenericDataProcessor<Vector2Int>
        {
            public override bool IsSystem
            {
                get
                {
                    return false;
                }
            }

            public override string LanguageKeyword
            {
                get
                {
                    return "Vector2Int";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "vector2int",
                    "unityengine.vector2int"
                };
            }

            public override Vector2Int Parse(string value)
            {
                string[] splitedValue = value.Split(',');
                return new Vector2Int(int.Parse(splitedValue[0]), int.Parse(splitedValue[1]));
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                Vector2Int vector2 = Parse(value);
                binaryWriter.Write(vector2.x);
                binaryWriter.Write(vector2.y);
            }
        }
    }
}
