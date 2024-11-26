using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EAttackCastTypeProcessor : GenericDataProcessor<EAttackCastType>
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
                    return "EAttackCastType";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EAttackCastType",
                };
            }

            public override EAttackCastType Parse(string value)
            {
                return Enum.Parse<EAttackCastType>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}