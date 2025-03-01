﻿using System;
using System.IO;

namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EBuffTriggerTypeProcessor : GenericDataProcessor<EBuffTriggerType>
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
                    return "EBuffTriggerType";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EBuffTriggerType",
                };
            }

            public override EBuffTriggerType Parse(string value)
            {
                return Enum.Parse<EBuffTriggerType>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write((int)Parse(value));
            }
        }
    }
}