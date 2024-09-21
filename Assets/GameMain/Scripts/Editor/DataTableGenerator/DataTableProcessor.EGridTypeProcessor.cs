using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EGridTypeProcessor : GenericDataProcessor<EGridType>
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
                    return "EGridType";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EGridType",
                };
            }

            public override EGridType Parse(string value)
            {
                return Enum.Parse<EGridType>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}