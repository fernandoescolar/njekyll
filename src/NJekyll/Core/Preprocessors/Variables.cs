using NJekyll.Model;
using NJekyll.Utilities;

namespace NJekyll.Core.Preprocessors
{
	public class Variables : IFileProcessor
	{
		private readonly Config _config;
		private readonly IYamlDeserializer _yamlDeserializer;

		public Variables(Config config, IYamlDeserializer yamlDeserializer)
		{
			_config = config;
			_yamlDeserializer = yamlDeserializer;
		}

		public void Process(File file, PipelineContext context)
		{
			if (file is not FileWithMetadata m) return;
			if (m.Header == null) return;

			m.Variables = _yamlDeserializer.Parse(m.Header);
		}
	}
}
