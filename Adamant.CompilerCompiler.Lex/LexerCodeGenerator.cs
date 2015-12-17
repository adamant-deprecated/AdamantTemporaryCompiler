using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Adamant.CompilerCompiler.Lex.CodeGen;
using Adamant.CompilerCompiler.Lex.Spec;
using Adamant.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex
{
	public class LexerCodeGenerator
	{
		private readonly LexerSpec lexerSpec;
		private readonly IReadOnlyDictionary<Mode, State> modeMap;
		private readonly IDictionary<int, int[]> planeOffsets;
		private readonly IList<Input> equivalenceTable;
		private readonly int[] rowMap;
		private readonly int[] transitions;
		private string fileName;

		public LexerCodeGenerator(LexerSpec lexerSpec, IReadOnlyDictionary<Mode, State> modeMap, IDictionary<int, int[]> planeOffsets, IList<Input> equivalenceTable, int[] rowMap, int[] transitions)
		{
			this.lexerSpec = lexerSpec;
			this.modeMap = modeMap;
			this.planeOffsets = planeOffsets;
			this.equivalenceTable = equivalenceTable;
			this.rowMap = rowMap;
			this.transitions = transitions;
		}

		public string FileName(Skeleton skeleton)
		{
			return Path.ChangeExtension(lexerSpec.LexerName, skeleton.Extension);
		}

		public string GenerateCode(Skeleton skeleton, string generatorVersion)
		{
			var equivalenceClassDataType = GenDataType(skeleton, equivalenceTable.Max(ec => ec.Value));
			var builder = new StringBuilder(skeleton.Template);
			builder.Replace("<%= EquivalenceClassType %>", equivalenceClassDataType);
			builder.Replace("<%= GeneratorVersion %>", generatorVersion);
			builder.Replace("<%= ClassName %>", lexerSpec.LexerName ?? "Lexer");
			builder.Replace("<%= ClassNamespace %>", lexerSpec.LexerNamespace);
			builder.Replace("<%= TokenTypes %>", GenTokenTypes());
			builder.Replace("<%= Modes %>", GetModes());
			builder.Replace("<%= InitialMode %>", lexerSpec.InitialMode.ToString());
			builder.Replace("<%= PlaneOffsets %>", GenPlaneOffsets(skeleton));
			builder.Replace("<%= EquivalenceClassCases %>", GenEquivalenceClassCases());
			builder.Replace("<%= EquivalenceTable %>", GenEquivalenceTable());
			builder.Replace("<%= RowMapType %>", GenDataType(skeleton, rowMap.Max()));
			builder.Replace("<%= RowMap %>", string.Join(", ", rowMap));
			builder.Replace("<%= StateType %>", GenDataType(skeleton, transitions.Max()));
			// TODO need to create an error state so that we don't need to put negative values in the table
			builder.Replace("<%= Transitions %>", string.Join(", ", transitions));
			return builder.ToString();
		}

		private string GetModes()
		{
			return string.Join(", ", lexerSpec.Modes.Select(mode => $"{mode} = {modeMap[mode].Index}"));
		}

		private string GenPlaneOffsets(Skeleton skeleton)
		{

			var builder = new StringBuilder();
			foreach(var planeOffset in planeOffsets)
			{
				if(planeOffset.Value.Length == 1) continue;
				var dataType = GenDataType(skeleton, planeOffset.Value.Max());
				builder.AppendLine($"\t\tprivate static readonly {dataType}[] plane{planeOffset.Key}Offsets = {{{string.Join(", ", planeOffset.Value)}}};");
			}

			return builder.ToString();
		}

		private static string GenDataType(Skeleton skeleton, int value)
		{
			return skeleton.DataTypes.OrderBy(dt => dt.Bits).First(dt => value <= Math.Pow(2, dt.Bits)).Name;
		}

		private string GenEquivalenceTable()
		{
			return string.Join(", ", equivalenceTable.Select(input => input.Value));
		}

		private string GenEquivalenceClassCases()
		{
			var defaultClass = DefaultClass();
			var builder = new StringBuilder();

			foreach(var planeOffset in planeOffsets)
			{
				if(planeOffset.Value.Length == 1)
				{
					if(planeOffset.Value[0] == defaultClass) continue;
					builder.AppendLine($"\t\t\t\tcase 0x{planeOffset.Key << 16:X}:");
					builder.AppendLine($"\t\t\t\t	return {planeOffset.Value[0]};");
					continue;
				}
				builder.AppendLine($"\t\t\t\tcase 0x{planeOffset.Key << 16:X}:");
				builder.AppendLine($"\t\t\t\t	return equivalenceTable[plane{planeOffset.Key}Offsets[(codePoint&0xFF00)>>8] + (codePoint&0xFF)];");
			}

			builder.AppendLine("\t\t\t\tdefault:");
			builder.AppendLine($"\t\t\t\t	return {defaultClass ?? -1};");

			return builder.ToString();
		}

		private int? DefaultClass()
		{
			var defaultClass = planeOffsets.Select(item => item.Value)
				.Where(e => e.Length == 1)
				.Select(e => e[0])
				.GroupBy(e => e, (ec, planes) => new { Class = ec, Count = planes.Count() })
				.OrderBy(ec => ec.Count)
				.Select(ec => (int?)ec.Class)
				.FirstOrDefault();
			return defaultClass;
		}

		private string GenTokenTypes()
		{
			return string.Join(", ", lexerSpec.Rules.OrderBy(r => r.Name).Select(r => r.Name));
		}
	}
}
