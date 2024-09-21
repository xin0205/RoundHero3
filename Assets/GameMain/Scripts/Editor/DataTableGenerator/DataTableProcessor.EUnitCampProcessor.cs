using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EUnitCampProcessor : GenericDataProcessor<EUnitCamp>
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
                    return "EUnitCamp";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EUnitCamp",
                };
            }

            public override EUnitCamp Parse(string value)
            {
                return Enum.Parse<EUnitCamp>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}