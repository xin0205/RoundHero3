using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EFlyTypeProcessor : GenericDataProcessor<EFlyType>
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
                    return "EFlyType";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EFlyType",
                };
            }

            public override EFlyType Parse(string value)
            {
                return Enum.Parse<EFlyType>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}