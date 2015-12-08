using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Adamant.CompilerCompiler.Lex.Spec
{
	public class RuleSpecs : KeyedCollection<string, RuleSpec>
	{
		public RuleSpecs(IEnumerable<RuleSpec> rules)
		{
			foreach(var rule in rules)
				Add(rule);
		}

		protected override string GetKeyForItem(RuleSpec rule)
		{
			return rule.Name;
		}
	}
}
