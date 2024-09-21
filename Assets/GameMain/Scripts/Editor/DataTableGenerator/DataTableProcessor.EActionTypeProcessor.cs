using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EActionTypeProcessor : GenericDataProcessor<EActionType>
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
                    return "EActionType";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EActionType",
                };
            }

            public override EActionType Parse(string value)
            {
                return Enum.Parse<EActionType>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}