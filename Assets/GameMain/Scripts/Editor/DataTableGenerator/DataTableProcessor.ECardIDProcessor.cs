// using System;
// using System.IO;
//
// namespace RoundHero.Editor.DataTableTools
// {
//     public sealed partial class DataTableProcessor
//     {
//         private sealed class ECardIDProcessor : GenericDataProcessor<ECardID>
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
//                     return "ECardID";
//                 }
//             }
//
//             public override string[] GetTypeStrings()
//             {
//                 return new string[]
//                 {
//                     "ECardID",
//                 };
//             }
//
//             public override ECardID Parse(string value)
//             {
//                 return Enum.Parse<ECardID>(value);
//             }
//
//             public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
//             {
//                 binaryWriter.Write((int)Parse(value));
//             }
//         }
//     }
// }