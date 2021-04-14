using NUglify;
using NUglify.Html;
using System;
using NJekyll.Model;

namespace NJekyll.Core.Postprocessors
{
	public class Html : IFileProcessor
	{
		private readonly Config _config;

		public Html(Config config)
		{
			_config = config;
		}

		public void Process(File file, PipelineContext context)
		{
			if (file is not FileWithMetadata m) return;
			if (!System.IO.Path.GetExtension(m.LocalPath).Equals(_config.HtmlExtension, StringComparison.InvariantCultureIgnoreCase)) return;

			var htmlSettings = HtmlSettings.Pretty();
			htmlSettings.Indent = "\t";
			m.Content = Uglify.Html(m.Content, htmlSettings).Code;
		}
	}
}
