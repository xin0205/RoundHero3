using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EUnitStateProcessor : GenericDataProcessor<EUnitState>
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
                    return "EUnitState";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EUnitState",
                };
            }

            public override EUnitState Parse(string value)
            {
                return Enum.Parse<EUnitState>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}