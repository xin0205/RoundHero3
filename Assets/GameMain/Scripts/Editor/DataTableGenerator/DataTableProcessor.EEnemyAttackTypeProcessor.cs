using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EEnemyAttackTypeProcessor : GenericDataProcessor<EEnemyAttackType>
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
                    return "EEnemyAttackType";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EEnemyAttackType",
                };
            }

            public override EEnemyAttackType Parse(string value)
            {
                return Enum.Parse<EEnemyAttackType>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}