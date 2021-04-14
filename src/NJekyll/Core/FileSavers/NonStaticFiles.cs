using System.Linq;
using System.Text;
using NJekyll.Model;

namespace NJekyll.Core.FileSavers
{
	public class NonStaticFiles : IProcessor
	{
		private readonly Config _config;

		public NonStaticFiles(Config config)
		{
			_config = config;
		}

		public void Process(PipelineContext context)
		{
			context.NonStaticFiles.AsParallel().ForAll(item =>
			{
				var fileSystemPath = System.IO.Path.Combine(_config.OutputPath, item.LocalPath);
				Helper.EnsureDirectoryExists(fileSystemPath);
				System.IO.File.WriteAllText(fileSystemPath, item.Content, new UTF8Encoding(false));
			});
		}
	}
}
