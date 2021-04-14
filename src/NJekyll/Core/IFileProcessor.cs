using NJekyll.Model;

namespace NJekyll.Core
{
	public interface IFileProcessor
	{
		void Process(File file, PipelineContext context);
	}
}