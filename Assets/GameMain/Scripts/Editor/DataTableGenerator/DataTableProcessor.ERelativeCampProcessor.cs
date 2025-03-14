﻿using System;
using System.IO;
using RoundHero;


namespace RoundHero.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class ERelativeCampProcessor : GenericDataProcessor<ERelativeCamp>
        {
            public override bool IsSystem
            {
                get { return false; }
            }

            public override bool IsEnum
            {
                get { return true; }
            }

            public override string LanguageKeyword
            {
                get { return "ERelativeCamp"; }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "ERelativeCamp",
                };
            }

            public override ERelativeCamp Parse(string value)
            {
                return Enum.Parse<ERelativeCamp>(value);
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter,
                string value)
            {
                binaryWriter.Write((int) Parse(value));
            }
        }

    }
}