using NJekyll.Model;

namespace NJekyll.Core.Preprocessors
{
	public class Layout : IFileProcessor
	{
		private readonly Config _config;

		public Layout(Config config)
		{
			_config = config;
		}

		public void Process(File file, PipelineContext context)
		{
			if (file is not FileWithMetadata m) return;
			if (m.Variables == null) return;
			if (!m.Variables.ContainsKey(_config.LayoutKey)) return;

			m.Layout = (string)m.Variables[_config.LayoutKey];
		}
	}
}
