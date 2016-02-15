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

			var defaultChannel = new Channel("Default");
			var whiteSpaceChannel = new Channel("WhiteSpace");
			var initial = new Mode("Default");
			var startCharClass = new Mode("StartCharacterClass");
			var charClass = new Mode("CharacterClass");

			SpecLexerSpec = new LexerSpec("SpecLexer", "Adamant.CompilerCompiler.Lex.SpecParsing", new[]
			{
				// Comments
				new RuleSpec(initial, "Comment",("/*" + ~R("*/")) |("//" + ~newline), Command.SetChannel(whiteSpaceChannel)),
				new RuleSpec(initial, "WhiteSpace", whitespace.Repeat(), Command.SetChannel(whiteSpaceChannel)),
	
				// Commands
				new RuleSpec(initial, "Mode", "@mode"),
				new RuleSpec(initial, "PushMode", "@pushMode"),
				new RuleSpec(initial, "PopMode", "@popMode"),
				new RuleSpec(initial, "Skip", "@skip"),
				new RuleSpec(initial, "More", "@more"),
				new RuleSpec(initial, "Type", "@type"),
				new RuleSpec(initial, "Channel", "@channel"),
				new RuleSpec(initial, "Error", "@error"),
				new RuleSpec(initial, "Capture", "@capture"),
				new RuleSpec(initial, "Decode", "@decode"),
				new RuleSpec(initial, "Text", "@text"),
				new RuleSpec(initial, "Action", "<%" + ~R("%>"), Command.Capture),

				// Keywords
				new RuleSpec(initial, "Lexer", "@lexer"),
				new RuleSpec(initial, "Namespace", "@namespace"),
				new RuleSpec(initial, "Modes", "@modes"),
				new RuleSpec(initial, "Channels", "@channels"),
				new RuleSpec(initial, "InvalidKeyword", "@" + new RuleReferenceSpec("Identifier"), Command.Capture, Command.FlagError),

				// Expression Operators
				new RuleSpec(initial, "Definition", ":"),
				new RuleSpec(initial, "Alternation", "|"),
				new RuleSpec(initial, "BeginCharClass", "[", Command.Capture,  Command.PushMode(startCharClass)),
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
				new RuleSpec(initial, "Number", "0" | (Class('1','9')+digit), Command.Capture),
				new RuleSpec(initial, "Identifier", (Class('a','z')|Class('A','Z'))+(Class('a','z')|Class('A','Z')|digit).Repeat(), Command.Capture),
				new RuleSpec(initial, "Literal", R("\"")+new RuleReferenceSpec("literalChar").RepeatAtLeast(1)+R("\"") | new RuleReferenceSpec("escapeChar"), Command.Capture),
				new RuleSpec(initial, "literalChar", new RuleReferenceSpec("escapeChar") | !(R("\\")|"\"")),

				new RuleSpec(initial, "Category", R("\\R")|"\\s"|"\\d"),

				// Fragments
				new RuleSpec(initial, "escapeChar",
					R("\\t")
					| R("\\n")
					| R("\\r")
					| R("\\b")
					| R("\\f")
					| R("\\0")
					| R("\\a")
					| R("\\v")
					| R("\\\"")
					| R("\\{")
					| R("\\'")
					| R("\\\\")
					| (R("\\x")+hexDigit.Repeat(2))
					| (R("\\u")+hexDigit.Repeat(4))
					| (R("\\U")+hexDigit.Repeat(6))
					| (R("\\u{")+hexDigit.Repeat(1,6)+R("}"))),

				// Fallback
				new RuleSpec(initial, "UnexpectedCodePoint", new CharClassSpec(InversionListCodePointSet.All), Command.Capture, Command.FlagError),

				// Character Classes
				new RuleSpec(startCharClass, "NegateCharClass", "^", Command.SetMode(charClass)),
				new RuleSpec(charClass, "Char", new RuleReferenceSpec("escapeChar")|charClassChar, Command.Capture, Command.SetMode(charClass)),
				new RuleSpec(charClass, "EscapeDash", R("\\-"), Command.Text("-"), Command.SetType("Char"), Command.SetMode(charClass)),
				new RuleSpec(charClass, "EscapeRightBracket", R("\\]"), Command.Text("-"), Command.SetType("Char"), Command.SetMode(charClass)),
				new RuleSpec(charClass, "CharRange", "-", Command.SetMode(charClass)),
				new RuleSpec(charClass, "EndCharClass", "]", Command.PopMode),
			},
			new[] { defaultChannel, whiteSpaceChannel },
			defaultChannel,
			new[] { initial, startCharClass, charClass },
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
