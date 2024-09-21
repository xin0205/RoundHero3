using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class ECardTriggerTypeProcessor : GenericDataProcessor<ECardTriggerType>
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
                    return "ECardTriggerType";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "ECardTriggerType",
                };
            }

            public override ECardTriggerType Parse(string value)
            {
                return Enum.Parse<ECardTriggerType>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}