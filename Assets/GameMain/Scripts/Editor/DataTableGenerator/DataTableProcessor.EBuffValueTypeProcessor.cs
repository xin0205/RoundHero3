using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EBuffValueTypeProcessor : GenericDataProcessor<EBuffValueType>
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
                    return "EBuffValueType";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EBuffValueType",
                };
            }

            public override EBuffValueType Parse(string value)
            {
                return Enum.Parse<EBuffValueType>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}