using System.Linq;
using NJekyll.Model;

namespace NJekyll.Core.FileSavers
{
	public class StaticFiles : IProcessor
	{
		private readonly Config _config;

		public StaticFiles(Config config)
		{
			_config = config;
		}

		public void Process(PipelineContext context)
		{
			context.StaticFiles.AsParallel().ForAll(item =>
			{
				var localPath = item.Path.Substring(_config.SitePath.Length + 1);
				var fileSystemPath = System.IO.Path.Combine(_config.OutputPath, localPath);
				Helper.EnsureDirectoryExists(fileSystemPath);
				System.IO.File.Copy(item.Path, fileSystemPath, true);
			});
		}
	}
}
