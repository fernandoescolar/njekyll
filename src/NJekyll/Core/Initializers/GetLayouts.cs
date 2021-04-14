using System.Linq;
using NJekyll.Utilities;

namespace NJekyll.Core.Initializers
{
	public class GetLayouts : IProcessor
	{
		private IFileFinder _fileFinder;
		private readonly IFileFactory _fileFactory;

		public GetLayouts(IFileFinder fileFinder, IFileFactory fileFactory)
		{
			_fileFinder = fileFinder;
			_fileFactory = fileFactory;
		}

		public void Process(PipelineContext context)
		{
			var layouts = _fileFinder.GetLayouts()
									 .Select(x => _fileFactory.Create(x))
									 .AsParallel()
									 .Select(x => _fileFactory.AddMetadata(x))
									 .ToDictionary(x => System.IO.Path.GetFileNameWithoutExtension(x.Path), x => x);

			context.Layouts = layouts;
		}
	}
}
