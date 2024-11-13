using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EWeaponHoldingTypeProcessor : GenericDataProcessor<EWeaponHoldingType>
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
                    return "EWeaponHoldingType";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EWeaponHoldingType",
                };
            }

            public override EWeaponHoldingType Parse(string value)
            {
                return Enum.Parse<EWeaponHoldingType>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }

}