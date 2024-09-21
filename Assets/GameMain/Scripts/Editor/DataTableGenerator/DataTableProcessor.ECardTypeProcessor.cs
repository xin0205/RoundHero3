using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class ECardTypeProcessor : GenericDataProcessor<ECardType>
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
                    return "ECardType";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "ECardType",
                };
            }

            public override ECardType Parse(string value)
            {
                return Enum.Parse<ECardType>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}