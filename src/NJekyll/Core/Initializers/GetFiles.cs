using System.Linq;
using NJekyll.Utilities;

namespace NJekyll.Core.Initializers
{
	public class GetFiles : IProcessor
	{
		private IFileFinder _fileFinder;
		private readonly IFileFactory _fileFactory;

		public GetFiles(IFileFinder fileFinder, IFileFactory fileFactory)
		{
			_fileFinder = fileFinder;
			_fileFactory = fileFactory;
		}

		public void Process(PipelineContext context)
		{
			var files = _fileFinder.GetFiles()
								   .Select(x => _fileFactory.Create(x))
								   .ToList();

			context.StaticFiles = files.Where(x => x.Header == null)
									   .ToList();

			context.NonStaticFiles = files.Where(x => x.Header != null)
										  .AsParallel()
										  .Select(x => _fileFactory.AddMetadata(x))
										  .ToList();

		}
	}
}
