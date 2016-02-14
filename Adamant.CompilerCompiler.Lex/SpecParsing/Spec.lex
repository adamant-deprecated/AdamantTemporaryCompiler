/*
This is the lexical definition for the lexer spec format .lex
*/

@lexer SpecLexer;

// Comments
Comment: "/*" ~"*/" | "//" ~\R -> @skip;
WhiteSpace: \s+ -> @skip;

// Functions
Capture: "@capture";
Decode: "@decode";
Substitute: "@sub";

// Commands
Skip: "@skip"; // Also an action
More: "@more";
Mode: "@mode";
PushMode: "@pushMode";
PopMode: "@popMode";
Type: "@type";
Channel: "@channel";
Error: "@error";
Action: @capture("<%" ~"%>");

// Keywords
Lexer = "@lexer";
Modes = "@modes";
Channels = "@channels";
InvalidKeyword = "@" @capture(Identifier) -> @error;

// Expression Operators
Definition: ":";
Alternation: "|";
BeginCharClass: "[" @capture("^"?) -> @pushMode(CharacterClass);
AnyChar: ".";
Optional: "?";
Complement: "!";
Repetition: "*";
Intersection: "&";
Subtraction: "-";
Upto: "~";
BeginGroup: "(";
EndGroup: ")";
BeginningOfLine: "^";
EndOfLine: "$";
BeginQuantifier: "{";
EndQuantifier: "}";
BeginCommands: "->";
Terminator: ";";
Comma: ",";

// Terminals
Number: @capture("0" | [1-9]\d*);
Identifier: @capture([a-zA-Z][a-zA-Z0-9_]*);  // simplified from \p{Letter}[\p{Letter}\d_]*
Literal:
	\" @capture(literalChar+) \"
	| escapeChar
	;

literalChar:
	escapeChar
	| [^\\"]		// TODO control chars and newlines // Simplified around Unicode
	;

Category:
	"\\R"
	| "\\s"
	| "\\d"
//	"\p{" ~"}"	// Removed to simplify for now
//	| "\P{" ~"}"
	;

// Fragments
escapeChar:				// These string escapes match Adamant
	  @sub("\\t", \t)
	| @sub("\\n" \n)
	| @sub("\\r", \r)
	| @sub("\\b", \b)
	| @sub("\\f", \f)
	| @sub("\\0", \0)
	| @sub("\\a", \a)
	| @sub("\\v", \v)
	| @sub(\\\", \")
	| @sub(\\\{, \{)
	| @sub(\\\', \')
	| @sub(\\\\, \\)
	| "\\x" @decode(hexDigit{2}, 16)
	| "\\u" @decode(hexDigit{4}, 16)
	| "\\U" @decode(hexDigit{6}, 16)
	| "\\u{" @decode(hexDigit{1,6}, 16) "}"
	;

hexDigit: [a-fA-F0-9];

// Fallback
UnexpectedCodePoint: [^] -> @error;

// Character Classes
@modes CharacterClass;
Char: escapeChar | @capture([^\\\-\]]) | @sub("\\-", "-") |  @sub("\\]", "]"); // TODO control chars and newlines // Simplified around Unicode
CharRange: @capture(Char "-" Char);
EndCharClass: "]" -> @popMode(CharacterClass);