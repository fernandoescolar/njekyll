using NJekyll.Model;

namespace NJekyll.Core.Preprocessors
{
	public class Paginate : IFileProcessor
	{
		private readonly Config _config;

		public Paginate(Config config)
		{
			_config = config;
		}

		public void Process(File file, PipelineContext context)
		{
			if (file is not FileWithMetadata m) return;
			if (m.Variables == null) return;

			if (m.Variables.ContainsKey(_config.PaginateKey) && int.TryParse(m.Variables[_config.PaginateKey].ToString(), out var p))
			{
				m.Paginate = p;
			}
		}
	}
}
