using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EGameDifficultyProcessor : GenericDataProcessor<EGameDifficulty>
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
                    return "EGameDifficulty";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EGameDifficulty",
                };
            }

            public override EGameDifficulty Parse(string value)
            {
                return Enum.Parse<EGameDifficulty>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}