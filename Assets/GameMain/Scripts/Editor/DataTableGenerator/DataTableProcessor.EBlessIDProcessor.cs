
using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EBlessIDProcessor : GenericDataProcessor<EBlessID>
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
                    return "EBlessID";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EBlessID",
                };
            }

            public override EBlessID Parse(string value)
            {
                return Enum.Parse<EBlessID>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}