using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EUnitAttributeProcessor : GenericDataProcessor<EUnitAttribute>
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
                    return "EUnitAttribute";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EUnitAttribute",
                };
            }

            public override EUnitAttribute Parse(string value)
            {
                return Enum.Parse<EUnitAttribute>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}