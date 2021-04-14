using System;
using System.Linq;
using NJekyll.Model;

namespace NJekyll.Core.Preprocessors
{
	public class Excerpt : IFileProcessor
	{
		private readonly Config _config;

		public Excerpt(Config config)
		{
			_config = config;
		}

		public void Process(File file, PipelineContext context)
		{
			if (file is not FileWithMetadata m) return;
			if (m.Variables == null) return;
			if (!m.IsPost) return;

			m.Variables[_config.ExcerptKey] = file.Content.Split(new[] { _config.ExcerptSeparator }, StringSplitOptions.None).First();
		}
	}
}
