using System.Collections.Generic;
using NJekyll.Model;

namespace NJekyll.Utilities
{
	public class LiquidCompilerFactory : ITemplateCompilerFactory
	{
		public ITemplateCompiler Create(Dictionary<string, FileWithMetadata> layouts, Dictionary<string, FileWithMetadata> includes)
		{
			return new LiquidCompiler(layouts, includes);
		}
	}
}
