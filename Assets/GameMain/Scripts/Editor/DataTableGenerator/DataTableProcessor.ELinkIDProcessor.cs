using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class ELinkIDProcessor : GenericDataProcessor<ELinkID>
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
                    return "ELinkID";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "ELinkID",
                };
            }

            public override ELinkID Parse(string value)
            {
                return Enum.Parse<ELinkID>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}