using System;
using System.IO;
using Adamant.CompilerCompiler.Lex.CodeGen;
using Adamant.CompilerCompiler.Lex.SpecParsing;
using NDesk.Options;

namespace Adamant.CompilerCompiler.Lex.CLI
{
	public class Program
	{
		public static int Main(string[] args)
		{
			var options = ParseOptions(args);

			if(options == null) return 1;

			// can't really read the input yet, just going to work from our spec object

			var spec = ThrowAway.SpecLexerSpec;

			spec.Validate();
			spec = spec.Simplify();
			var nfa = spec.ConvertToNFA();
			var dfa = nfa.ToDFA();
			var minDFA = dfa.Minimize();
			var codeGen = minDFA.ConvertToCodeGenerator();
			var skeleton = new CSharpSkeleton();
			var code = codeGen.GenerateCode(skeleton, "0.1.0-alpha-*");
			var targetPath = Path.Combine(Path.GetDirectoryName(options.FilePath), codeGen.FileName(skeleton));
			File.WriteAllText(targetPath, code);
			return 0;
		}

		internal static LexerOptions ParseOptions(string[] args)
		{
			var options = new OptionSet();
			var help = false;
			options.Add("h|?|help", "output this help information", o => help = true);
			var version = false;
			options.Add("V|version", "report version", o => version = true);
			var operands = options.Parse(args);
			if(help)
			{
				PrintHelp(options);
				return null;
			}
			if(version)
			{
				PrintVersion();
				return null;
			}
			if(operands.Count != 1)
			{
				Console.WriteLine("Must specify a single lexer file");
				PrintHelp(options);
				return null;
			}
			return new LexerOptions(operands[0]);
		}

		internal static void PrintHelp(OptionSet options)
		{
			Console.WriteLine("Usage: adamant-cc-lex [options] <input file>");
			Console.WriteLine("Generates lexer code to tokenize text.");
			Console.WriteLine();
			options.WriteOptionDescriptions(Console.Out);
		}

		internal static void PrintVersion()
		{
			Console.WriteLine("adamant-cc-lex 0.1.0-alpha-*"); // TODO have a way to access what was in the forge project
			Console.WriteLine($"Unicode v?");
		}
	}
}
