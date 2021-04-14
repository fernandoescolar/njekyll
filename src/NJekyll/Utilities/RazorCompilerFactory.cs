using RazorEngineCore;
using System.Collections.Generic;
using System.Linq;
using NJekyll.Model;

namespace NJekyll.Utilities
{
	public class RazorCompilerFactory : ITemplateCompilerFactory
	{
		private RazorEngine _engine;

		public RazorCompilerFactory(RazorEngine engine)
		{
			_engine = engine;
		}

		public ITemplateCompiler Create(Dictionary<string, FileWithMetadata> layouts, Dictionary<string, FileWithMetadata> includes)
		{
			var compiledLayouts = layouts.ToDictionary(x => x.Key, x => _engine.Compile(x.Value));
			var compiledIncludes = includes.ToDictionary(x => x.Key, x => _engine.Compile(x.Value));
			return new RazorCompiler(_engine, compiledLayouts, compiledIncludes);
		}
	}
}
