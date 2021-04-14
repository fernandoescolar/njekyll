using NJekyll.Model;

namespace NJekyll.Utilities
{
	public interface ITemplateCompiler
	{
		string Compile(FileWithMetadata file, object site, object paginator = null);
	}
}