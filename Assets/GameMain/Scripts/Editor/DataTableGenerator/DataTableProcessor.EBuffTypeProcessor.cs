using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EBuffTypeProcessor : GenericDataProcessor<EBuffType>
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
                    return "EBuffType";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EBuffType",
                };
            }

            public override EBuffType Parse(string value)
            {
                return Enum.Parse<EBuffType>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}