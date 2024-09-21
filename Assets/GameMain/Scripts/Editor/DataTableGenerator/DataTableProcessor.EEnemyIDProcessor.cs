// using System;
// using System.IO;
//
// namespace RoundHero.Editor.DataTableTools
// {
//     public sealed partial class DataTableProcessor
//     {
//         private sealed class EEnemyIDProcessor : GenericDataProcessor<EMonsterID>
//         {
//             public override bool IsSystem
//             {
//                 get
//                 {
//                     return false;
//                 }
//             }
//             
//             public override bool IsEnum
//             {
//                 get
//                 {
//                     return true;
//                 }
//             }
//
//             public override string LanguageKeyword
//             {
//                 get
//                 {
//                     return "EMonsterID";
//                 }
//             }
//
//             public override string[] GetTypeStrings()
//             {
//                 return new string[]
//                 {
//                     "EMonsterID",
//                 };
//             }
//
//             public override EMonsterID Parse(string value)
//             {
//                 return Enum.Parse<EMonsterID>(value);
//             }
//
//             public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
//             {
//                 binaryWriter.Write((int)Parse(value));
//             }
//         }
//     }
// }