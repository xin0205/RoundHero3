using System;
using System.IO;
using UnityEngine;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EAttackTypeProcessor : GenericDataProcessor<EAttackType>
        {
            public override bool IsSystem
            {
                get
                {
                    return false;
                }
            }
            
            public override bool IsEnum
            {
                get
                {
                    return true;
                }
            }

            public override string LanguageKeyword
            {
                get
                {
                    return "EAttackType";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EAttackType",
                };
            }

            public override EAttackType Parse(string value)
            {
                return Enum.Parse<EAttackType>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}