using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EBuffIDProcessor : GenericDataProcessor<EBuffID>
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
                    return "EBuffID";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EBuffID",
                };
            }

            public override EBuffID Parse(string value)
            {
                return Enum.Parse<EBuffID>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}