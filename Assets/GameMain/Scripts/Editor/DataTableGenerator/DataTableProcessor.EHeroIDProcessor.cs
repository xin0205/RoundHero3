using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EHeroIDProcessor : GenericDataProcessor<EHeroID>
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
                    return "EHeroID";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EHeroID",
                };
            }

            public override EHeroID Parse(string value)
            {
                return Enum.Parse<EHeroID>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}