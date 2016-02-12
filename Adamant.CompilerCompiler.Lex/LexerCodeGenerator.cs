using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Adamant.CompilerCompiler.Lex.CodeGen;
using Adamant.CompilerCompiler.Lex.Spec;
using Adamant.FiniteAutomata;

namespace Adamant.CompilerCompiler.Lex
{
	public class LexerCodeGenerator
	{
		private readonly LexerSpec lexerSpec;
		private readonly IReadOnlyDictionary<Mode, State> modeMap;
		private readonly int errorState;
		private readonly IDictionary<int, int[]> planeOffsets;
		private readonly IList<Input> equivalenceTable;
		private readonly int[] rowMap;
		private readonly int[] transitions;

		public LexerCodeGenerator(LexerSpec lexerSpec, IReadOnlyDictionary<Mode, State> modeMap, int errorState, IDictionary<int, int[]> planeOffsets, IList<Input> equivalenceTable, int[] rowMap, int[] transitions)
		{
			this.lexerSpec = lexerSpec;
			this.modeMap = modeMap;
			this.errorState = errorState;
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
			var template = skeleton.Template;
			var equivalenceClassDataType = GenDataType(skeleton, equivalenceTable.Max(ec => ec.Value));

			template = Replace(template, "EquivalenceClassType", equivalenceClassDataType);
			template = Replace(template, "GeneratorVersion", generatorVersion);
			template = Replace(template, "ClassName", lexerSpec.LexerName ?? "Lexer");
			template = Replace(template, "ClassNamespace", lexerSpec.LexerNamespace);
			template = Replace(template, "TokenTypes", GenTokenTypes());
			template = Replace(template, "Modes", GetModes());
			template = Replace(template, "InitialMode", lexerSpec.InitialMode.ToString());
			template = Replace(template, "ErrorState", errorState.ToString());
			template = Replace(template, "PlaneOffsets", GenPlaneOffsets(skeleton).ToList());
			template = Replace(template, "EquivalenceClassCases", GenEquivalenceClassCases().ToList());
			template = Replace(template, "EquivalenceTable", GenEquivalenceTable());
			template = Replace(template, "RowMapType", GenDataType(skeleton, rowMap.Max()));
			template = Replace(template, "RowMap", GenInts(rowMap));
			template = Replace(template, "StateType", GenDataType(skeleton, transitions.Max()));
			// TODO need to create an error state so that we don't need to put negative values in the table
			template = Replace(template, "Transitions", GenInts(transitions));
			return template;
		}

		private static string Replace(string template, string expression, string value)
		{
			var expTemplate = new Regex(@"<%=\s*" + Regex.Escape(expression) + @"\s*%>");
			return expTemplate.Replace(template, value);
		}

		private static string Replace(string template, string expression, ICollection<string> lines)
		{
			// Had some problems with this regex becuase \s matches newlines.  It still isn't actually fully unicode correct
			var expTemplate = new Regex(@"^(?<leadingSpace>[ \t]*)<%=\s*" + Regex.Escape(expression) + @"\s*%>[ \t]*(?<newline>\r\n|\r|\n|\f|\u0085|\u2028|\u2029)", RegexOptions.Multiline);
			// Replace with lines, each with that much leading space and that newline
			template = expTemplate.Replace(template, match => string.Concat(lines.Select(l => match.Groups["leadingSpace"] + l + match.Groups["newline"])));
			// Now replace instances not on their own line
			return Replace(template, expression, string.Join(" ", lines));
		}

		private static IEnumerable<string> GenCommaSeparatedLines(IEnumerable<string> values)
		{
			var line = new StringBuilder();
			foreach(var value in values)
			{
				if(line.Length + value.Length + 2 > 65)
				{
					// start a new line
					yield return line.ToString();
					line.Clear();
				}
				if(line.Length != 0)
					line.Append(' ');
				line.Append(value);
				line.Append(',');
			}

			line.Length -= 1; // remove the last comma
			yield return line.ToString();
		}

		private static ICollection<string> GenInts(int[] values)
		{
			return GenCommaSeparatedLines(values.Select(v => v.ToString())).ToList();
		}

		private ICollection<string> GetModes()
		{
			return GenCommaSeparatedLines(lexerSpec.Modes.Select(mode => $"{mode} = {modeMap[mode].Index}")).ToList();
		}

		private IEnumerable<string> GenPlaneOffsets(Skeleton skeleton)
		{
			foreach(var planeOffset in planeOffsets)
			{
				if(planeOffset.Value.Length == 1) continue;
				var dataType = GenDataType(skeleton, planeOffset.Value.Max());
				yield return $"private static readonly {dataType}[] plane{planeOffset.Key}Offsets =";
				yield return "{";
				foreach(var line in GenCommaSeparatedLines(planeOffset.Value.Select(v => v.ToString())))
					yield return "\t" + line;
				yield return "};";
			}
		}

		private static string GenDataType(Skeleton skeleton, int value)
		{
			return skeleton.DataTypes.OrderBy(dt => dt.Bits).First(dt => value <= Math.Pow(2, dt.Bits)).Name;
		}

		private ICollection<string> GenEquivalenceTable()
		{
			return GenCommaSeparatedLines(equivalenceTable.Select(input => input.Value.ToString())).ToList();
		}

		private IEnumerable<string> GenEquivalenceClassCases()
		{
			var defaultClass = DefaultClass();
			foreach(var planeOffset in planeOffsets)
			{
				if(planeOffset.Value.Length == 1)
				{
					if(planeOffset.Value[0] == defaultClass) continue;
					yield return $"case 0x{planeOffset.Key << 16:X}:";
					yield return $"\treturn {planeOffset.Value[0]};";
					continue;
				}
				yield return $"case 0x{planeOffset.Key << 16:X}:";
				yield return $"\treturn equivalenceTable[plane{planeOffset.Key}Offsets[(codePoint & 0xFF00) >> 8] + (codePoint & 0xFF)];";
			}

			yield return "default:";
			yield return $"\treturn {defaultClass ?? -1};";
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

		private ICollection<string> GenTokenTypes()
		{
			return GenCommaSeparatedLines(lexerSpec.Rules.OrderBy(r => r.Name).Select(r => r.Name)).ToList();
		}
	}
}
