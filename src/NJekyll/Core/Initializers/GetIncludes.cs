using System.Linq;
using NJekyll.Utilities;

namespace NJekyll.Core.Initializers
{
	public class GetIncludes : IProcessor
	{
		private IFileFinder _fileFinder;
		private readonly IFileFactory _fileFactory;

		public GetIncludes(IFileFinder fileFinder, IFileFactory fileFactory)
		{
			_fileFinder = fileFinder;
			_fileFactory = fileFactory;
		}

		public void Process(PipelineContext context)
		{
			var includes = _fileFinder.GetIncludes()
									  .Select(x => _fileFactory.Create(x))
									  .Where(x => x.Header == null)
									  .AsParallel()
									  .Select(x => _fileFactory.AddMetadata(x))
									  .ToDictionary(x => System.IO.Path.GetFileName(x.Path), x => x);

			context.Includes = includes;
		}
	}
}
