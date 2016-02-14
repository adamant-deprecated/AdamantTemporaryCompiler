using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.CompilerCompiler.Lex.Spec;

namespace Adamant.CompilerCompiler.Lex
{
	public class LexerSpec
	{
		public readonly string LexerName;
		public readonly string LexerNamespace;
		public readonly RuleSpecs Rules;
		public readonly ISet<Channel> Channels;
		public readonly Channel DefaultChannel;
		public readonly ISet<Mode> Modes;
		public readonly Mode InitialMode;
		public readonly bool HasBeenSimplified;

		public LexerSpec(string lexerName, string lexerNamespace, IEnumerable<RuleSpec> rules, IEnumerable<Channel> channels, Channel defaultChannel, IEnumerable<Mode> modes, Mode initialMode)
			: this(lexerName, lexerNamespace, rules, channels, defaultChannel, modes, initialMode, false)
		{
		}

		private LexerSpec(string lexerName, string lexerNamespace, IEnumerable<RuleSpec> rules, IEnumerable<Channel> channels, Channel defaultChannel, IEnumerable<Mode> modes, Mode initialMode, bool hasBeenSimplified)
		{
			LexerName = lexerName;
			LexerNamespace = lexerNamespace;
			Rules = new RuleSpecs(rules);
			Channels = new HashSet<Channel>(channels);
			DefaultChannel = defaultChannel;
			Channels.Add(DefaultChannel);
			Modes = new HashSet<Mode>(modes);
			InitialMode = initialMode;
			Modes.Add(InitialMode);
			HasBeenSimplified = hasBeenSimplified;
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
		///		* Removes unused channels
		/// </summary>
		/// <returns></returns>
		public LexerSpec Simplify()
		{
			var reachableModes = ReachableModes();
			var simplifiedRules = Rules.Select(r => r.Simplify(reachableModes, this)).Where(r => !r.IsFragment && r.Modes.Any()).ToList();
			var usedChannels = UsedChannels(simplifiedRules);

			return new LexerSpec(LexerName, LexerNamespace, simplifiedRules, usedChannels, DefaultChannel, reachableModes, InitialMode, true);
		}

		private HashSet<Channel> UsedChannels(IEnumerable<RuleSpec> simplifiedRules)
		{
			var usedChannels = new HashSet<Channel> { DefaultChannel };
			foreach(var rule in simplifiedRules)
				usedChannels.UnionWith(rule.ChannelsUsed());
			return usedChannels;
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
	}
}
