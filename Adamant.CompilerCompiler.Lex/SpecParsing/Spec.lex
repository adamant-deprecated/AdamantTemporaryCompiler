/*
This is the lexical definition for the lexer spec format .lex
*/

@lexer SpecLexer;
@channels WhiteSpace;

// Comments
Comment: "/*" ~"*/" | "//" ~\R -> @channel(WhiteSpace);
WhiteSpace: \s+ -> @channel(WhiteSpace);

// Commands
Mode: "@mode";
PushMode: "@pushMode";
PopMode: "@popMode";
Skip: "@skip";
More: "@more";
Type: "@type";
Channel: "@channel";
Error: "@error";
Capture: "@capture";
Decode: "@decode";
Text: "@text";
Action: "<%" ~"%>" -> @capture;

// Keywords
Lexer = "@lexer";
Namespace = "@namespace";
Modes = "@modes";
Channels = "@channels";
InvalidKeyword = "@" Identifier -> @capture, @error;

// Expression Operators
Definition: ":";
Alternation: "|";
BeginCharClass: "[" -> @pushMode(CharacterClass);
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
Number: "0" | [1-9]\d* -> @capture;
Identifier: [a-zA-Z][a-zA-Z0-9_]* -> @capture;  // simplified from \p{Letter}[\p{Letter}\d_]*
Literal: \" literalChar+ \" | escapeChar -> @capture;

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
	  "\\t"
	| "\\n"
	| "\\r"
	| "\\b"
	| "\\f"
	| "\\0"
	| "\\a"
	| "\\v"
	| "\\\""
	| "\\\{"
	| "\\\'"
	| "\\\\"
	| "\\x" hexDigit{2}
	| "\\u" hexDigit{4}
	| "\\U" hexDigit{6}
	| "\\u{" hexDigit{1,6} "}"
	;

hexDigit: [a-fA-F0-9];

// Fallback
UnexpectedCodePoint: [^] -> @capture, @error;

// Character Classes
@modes StartCharacterClass;
NegateCharClass: "^" -> @mode(CharacterClass);

@modes CharacterClass;
Char: escapeChar | [^\\\-\]] -> @capture, @mode(CharacterClass); // TODO control chars and newlines // Simplified around Unicode
EscapeDash: "\\-" -> @text("-"), @token(Char), @mode(CharacterClass);
EscapeRightBracket: "\\]" -> @text("]"), @token(Char), @mode(CharacterClass);
CharRange: "-"-> @mode(CharacterClass);
EndCharClass: "]" -> @popMode;