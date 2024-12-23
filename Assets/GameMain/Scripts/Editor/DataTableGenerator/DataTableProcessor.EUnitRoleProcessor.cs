﻿using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EUnitRoleProcessor : GenericDataProcessor<EUnitRole>
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
                    return "EUnitRole";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EUnitRole",
                };
            }

            public override EUnitRole Parse(string value)
            {
                return Enum.Parse<EUnitRole>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}