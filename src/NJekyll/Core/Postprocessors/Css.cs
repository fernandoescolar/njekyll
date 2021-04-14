using NUglify;
using System;
using NJekyll.Model;

namespace NJekyll.Core.Postprocessors
{
	public class Css : IFileProcessor
	{
		private readonly Config _config;

		public Css(Config config)
		{
			_config = config;
		}

		public void Process(File file, PipelineContext context)
		{
			if (file is not FileWithMetadata m) return;
			if (!System.IO.Path.GetExtension(m.LocalPath).Equals(_config.CssExtension, StringComparison.InvariantCultureIgnoreCase)) return;

			m.Content = Uglify.Css(m.Content).Code;
		}
	}
}
