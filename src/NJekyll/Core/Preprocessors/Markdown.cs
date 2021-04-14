using System;
using NJekyll.Model;
using NJekyll.Utilities;

namespace NJekyll.Core.Preprocessors
{
	public class Markdown : IFileProcessor
	{
		private readonly Config _config;
		private readonly IMarkdownRenderer _renderer;

		public Markdown(Config config, IMarkdownRenderer renderer)
		{
			_config = config;
			_renderer = renderer;
		}

		public void Process(File file, PipelineContext context)
		{
			if (file is not FileWithMetadata m) return;

			var isMarkdown = System.IO.Path.GetExtension(m.Path).Equals(_config.MarkdownExtension, StringComparison.InvariantCultureIgnoreCase);
			if (!isMarkdown) return;

			m.LocalPath = System.IO.Path.ChangeExtension(m.LocalPath, _config.HtmlExtension);
			m.Content = _renderer.Render(m.Content);
		}
	}
}
