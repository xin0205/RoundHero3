using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EEnemyTypeProcessor : GenericDataProcessor<EEnemyType>
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
                    return "EEnemyType";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EEnemyType",
                };
            }

            public override EEnemyType Parse(string value)
            {
                return Enum.Parse<EEnemyType>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}