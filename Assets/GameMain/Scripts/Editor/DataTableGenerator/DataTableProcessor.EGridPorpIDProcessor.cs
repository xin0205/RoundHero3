using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EGridPropIDProcessor : GenericDataProcessor<EGridPropID>
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
                    return "EGridPropID";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EGridPropID",
                };
            }

            public override EGridPropID Parse(string value)
            {
                return Enum.Parse<EGridPropID>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}