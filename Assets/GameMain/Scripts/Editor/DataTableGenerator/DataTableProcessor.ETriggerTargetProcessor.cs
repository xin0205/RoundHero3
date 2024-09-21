using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class ETriggerTargetProcessor : GenericDataProcessor<ETriggerTarget>
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
                    return "ETriggerTarget";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "ETriggerTarget",
                };
            }

            public override ETriggerTarget Parse(string value)
            {
                return Enum.Parse<ETriggerTarget>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}