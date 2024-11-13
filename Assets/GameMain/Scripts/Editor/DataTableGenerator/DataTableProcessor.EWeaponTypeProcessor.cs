using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EWeaponTypeProcessor : GenericDataProcessor<EWeaponType>
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
                    return "EWeaponType";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EWeaponType",
                };
            }

            public override EWeaponType Parse(string value)
            {
                return Enum.Parse<EWeaponType>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}