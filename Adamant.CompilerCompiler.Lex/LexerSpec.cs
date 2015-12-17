using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.CompilerCompiler.Lex.Services;
using Adamant.CompilerCompiler.Lex.Spec;

namespace Adamant.CompilerCompiler.Lex
{
	public class LexerSpec
	{
		public readonly string LexerName;
		public readonly string LexerNamespace;
		public readonly Mode InitialMode;
		public readonly RuleSpecs Rules;
		public readonly ISet<Mode> Modes;
		public readonly bool HasBeenSimplified;

		public LexerSpec(string lexerName, string lexerNamespace, IEnumerable<RuleSpec> rules, IEnumerable<Mode> modes, Mode initialMode)
			: this(lexerName, lexerNamespace, rules, modes, initialMode, false)
		{
		}

		private LexerSpec(string lexerName, string lexerNamespace, IEnumerable<RuleSpec> rules, IEnumerable<Mode> modes, Mode initialMode, bool hasBeenSimplified)
		{
			LexerName = lexerName;
			Rules = new RuleSpecs(rules);
			Modes = new HashSet<Mode>(modes);
			InitialMode = initialMode;
			Modes.Add(InitialMode);
			HasBeenSimplified = hasBeenSimplified;
			LexerNamespace = lexerNamespace;
		}

		/// <summary>
		/// Validates that
		///		* Rules don't reference rules not in the spec
		///		* Rules don't reference modes not in the spec
		///
		/// TODO really should return a collection of errors rather than just throwing exception
		/// </summary>
		public void Validate()
		{
			var duplicateRules = Rules.GroupBy(r => r.Name).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
			if(duplicateRules.Any())
				throw new Exception($"Rules {string.Join(", ", duplicateRules)} defined more than once");

			foreach(var rule in Rules)
				rule.Validate(this);
		}

		/// <summary>
		/// Simplifies a spec.  This includes:
		///		* Combining character classes and singlar code point literals
		///		* Removing unused modes
		///		* Converts Intersection to Complement
		///		* Converts Subtraction to Complement
		///		* Converts Upto to Complement
		///		* Expands rule references
		///		* Removes fragments
		///		* Removes unreachable rules
		/// </summary>
		/// <returns></returns>
		public LexerSpec Simplify()
		{
			var reachableModes = ReachableModes();
			var simplifiedRules = Rules.Select(r => r.Simplify(reachableModes, this)).Where(r => !r.IsFragment && r.Modes.Any());

			return new LexerSpec(LexerName, LexerNamespace, simplifiedRules, reachableModes, InitialMode, true);
		}

		/// <summary>
		/// Finds all the modes that can be reached by the application of commands
		/// </summary>
		/// <returns></returns>
		private HashSet<Mode> ReachableModes()
		{
			var reachableModes = new HashSet<Mode> { InitialMode };
			int modeCount;
			do
			{
				modeCount = reachableModes.Count;
				foreach(var rule in Rules.Where(r => r.Modes.Overlaps(reachableModes)))
					reachableModes.UnionWith(rule.ModesEntered());
			} while(reachableModes.Count != modeCount);
			return reachableModes;
		}

		public LexerNFA ConvertToNFA()
		{
			return LexerTransformer.Instance.ConvertToNFA(this);
		}
	}
}
