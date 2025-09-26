using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class ETriggerDataTypeProcessor : GenericDataProcessor<ETriggerDataType>
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
                    return "ETriggerDataType";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "ETriggerDataType",
                };
            }

            public override ETriggerDataType Parse(string value)
            {
                return Enum.Parse<ETriggerDataType>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}