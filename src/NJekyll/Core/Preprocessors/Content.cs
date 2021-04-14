using NJekyll.Model;

namespace NJekyll.Core.Preprocessors
{
	public class Content : IFileProcessor
	{
		private readonly Config _config;

		public Content(Config config)
		{
			_config = config;
		}

		public void Process(File file, PipelineContext context)
		{
			if (file is not FileWithMetadata m) return;
			if (m.Variables == null) return;

			m.Variables[_config.ContentKey] = file.Content;
		}
	}
}
