using NJekyll.Model;

namespace NJekyll.Core.Preprocessors
{
	public class Category : IFileProcessor
	{
		private readonly Config _config;

		public Category(Config config)
		{
			_config = config;
		}

		public void Process(File file, PipelineContext context)
		{
			if (file is not FileWithMetadata m) return;
			if (m.Variables == null) return;
			if (!m.Variables.ContainsKey(_config.CategoryKey)) return;

			m.Category = (string)m.Variables[_config.CategoryKey];
		}
	}
}
