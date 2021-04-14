using System.Collections.Generic;
using NJekyll.Model;

namespace NJekyll.Utilities
{
	public interface ITemplateCompilerFactory
	{
		ITemplateCompiler Create(Dictionary<string, FileWithMetadata> layouts, Dictionary<string, FileWithMetadata> includes);
	}
}