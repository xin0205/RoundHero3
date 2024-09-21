using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class ELinkTypeProcessor : GenericDataProcessor<ELinkType>
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
                    return "ELinkType";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "ELinkType",
                };
            }

            public override ELinkType Parse(string value)
            {
                return Enum.Parse<ELinkType>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}