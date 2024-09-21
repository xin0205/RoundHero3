using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EAttackTargetProcessor : GenericDataProcessor<EAttackTarget>
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
                    return "EAttackTarget";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EAttackTarget",
                };
            }

            public override EAttackTarget Parse(string value)
            {
                return Enum.Parse<EAttackTarget>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}