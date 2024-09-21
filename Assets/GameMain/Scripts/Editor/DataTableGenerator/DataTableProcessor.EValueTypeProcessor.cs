using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EValueTypeProcessor : GenericDataProcessor<EValueType>
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
                    return "EValueType";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EValueType",
                };
            }

            public override EValueType Parse(string value)
            {
                return Enum.Parse<EValueType>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}