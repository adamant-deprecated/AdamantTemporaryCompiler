using Adamant.CompilerCompiler.Lex.Spec;
using Adamant.CompilerCompiler.Lex.Spec.Regexes;
using Sys.Text;

namespace Adamant.CompilerCompiler.Lex.SpecParsing
{
	public static class ThrowAway
	{
		public static readonly LexerSpec SpecLexerSpec;

		static ThrowAway()
		{
			var newline = new AlternationSpec(new RegexSpec[] { "\r", "\n", "\u2028", "\u2029", "\u000B", "\u000C", "\u0085", "\r\n" });
			var whitespace = new AlternationSpec(new RegexSpec[] { newline, " ", "\t", "\f", "\v" });
			var digit = Class('0', '9');
			var hexDigit = new AlternationSpec(new[] { digit, Class('a', 'f'), Class('A', 'F') });

			var charClassChar = new CharClassSpec(new InversionListCodePointSet('\\')
				.Union(new InversionListCodePointSet('-'))
				.Union(new InversionListCodePointSet(']')).Complement());

			var initial = new Mode("Default");
			var charClass = new Mode("CharacterClass");

			SpecLexerSpec = new LexerSpec("SpecLexer", "Adamant.CompilerCompiler.Lex.SpecParsing", new[]
			{
				// Comments
				new RuleSpec(initial, "Comment",("/*" + ~R("*/")) |("//" + ~newline), Command.Skip),
				new RuleSpec(initial, "WhiteSpace", whitespace.Repeat(), Command.Skip),
	
				// Functions
				new RuleSpec(initial, "Skip", "@skip"), // Also an action
				new RuleSpec(initial, "Substitute", "@sub"),
				new RuleSpec(initial, "Decode", "@decode"),
				new RuleSpec(initial, "Capture", "@capture"),

				// Commands
				new RuleSpec(initial, "More", "@more"),
				new RuleSpec(initial, "Mode", "@mode"),
				new RuleSpec(initial, "PushMode", "@pushMode"),
				new RuleSpec(initial, "PopMode", "@popMode"),

				// Keywords
				new RuleSpec(initial, "Lexer", "@lexer"),
				new RuleSpec(initial, "Modes", "@modes"),
				new RuleSpec(initial, "Channels", "@channels"),
				new RuleSpec(initial, "InvalidKeyword", "@" + new RuleReferenceSpec("Identifier"), Command.FlagError),

				// Expression Operators
				new RuleSpec(initial, "Definition", ":"),
				new RuleSpec(initial, "Alternation", "|"),
				new RuleSpec(initial, "BeginCharClass", "[" + R("^").Optional(), Command.PushMode(charClass)),
				new RuleSpec(initial, "AnyChar", "."),
				new RuleSpec(initial, "Optional", "?"),
				new RuleSpec(initial, "Complement", "!"),
				new RuleSpec(initial, "Repetition", "*"),
				new RuleSpec(initial, "Intersection", "&"),
				new RuleSpec(initial, "Subtraction", "-"),
				new RuleSpec(initial, "Upto", "~"),
				new RuleSpec(initial, "BeginGroup", "("),
				new RuleSpec(initial, "EndGroup", ")"),
				new RuleSpec(initial, "BeginningOfLine", "^"),
				new RuleSpec(initial, "EndOfLine", "$"),
				new RuleSpec(initial, "BeginQuantifier", "{"),
				new RuleSpec(initial, "EndQuantifier", "}"),
				new RuleSpec(initial, "BeginCommands", "->"),
				new RuleSpec(initial, "Terminator", ";"),
				new RuleSpec(initial, "Comma", ","),

				// Terminals
				new RuleSpec(initial, "Number", "0" | (Class('1','9')+digit)),
				new RuleSpec(initial, "Identifier", (Class('a','z')|Class('A','Z'))+(Class('a','z')|Class('A','Z')|digit).Repeat()),
				new RuleSpec(initial, "Literal", R("\"")+new RuleReferenceSpec("literalChar").RepeatAtLeast(1).Capture()+R("\"") | new RuleReferenceSpec("escapeChar")),
				new RuleSpec(initial, "literalChar", new RuleReferenceSpec("escapeChar") | !(R("\\")|"\"")),

				new RuleSpec(initial, "Category", R("\\R")|"\\s"|"\\d"),

				// Fragments
				new RuleSpec(initial, "escapeChar",
					R("\\t").Sub("\t")
					| R("\\n").Sub("\n")
					| R("\\r").Sub("\r")
					| R("\\b").Sub("\b")
					| R("\\f").Sub("\f")
					| R("\\0").Sub("\0")
					| R("\\a").Sub("\a")
					| R("\\v").Sub("\v")
					| R("\\\"").Sub("\"")
					| R("\\{").Sub("{")
					| R("\\'").Sub("'")
					| R("\\\\").Sub("\\")
					| (R("\\x")+hexDigit.Repeat(2).Decode(16))
					| (R("\\u")+hexDigit.Repeat(4).Decode(16))
					| (R("\\U")+hexDigit.Repeat(6).Decode(16))
					| (R("\\u{")+hexDigit.Repeat(1,6).Decode(16)+R("}"))),

				// Fallback
				new RuleSpec(initial, "UnexpectedCodePoint", new CharClassSpec(InversionListCodePointSet.All), Command.FlagError),

				// Character Classes
				new RuleSpec(charClass, "Char", new RuleReferenceSpec("escapeChar")|charClassChar |R("\\-").Sub("-")|R("\\]").Sub("]")),
				new RuleSpec(charClass, "CharRange", new RuleReferenceSpec("Char")+"-"+new RuleReferenceSpec("Char")),
				new RuleSpec(charClass, "EndCharClass", "]", Command.PopMode),
			},
			new[] { initial, charClass },
			initial);
		}

		private static RegexSpec R(RegexSpec regex)
		{
			return regex;
		}

		private static RegexSpec Class(char from, char to)
		{
			return new CharClassSpec(new InversionListCodePointSet(from, to));
		}
	}
}
