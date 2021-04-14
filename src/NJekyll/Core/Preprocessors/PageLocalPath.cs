using System;
using NJekyll.Model;

namespace NJekyll.Core.Preprocessors
{
	public class PageLocalPath : IFileProcessor
	{
		private readonly Config _config;

		public PageLocalPath(Config config)
		{
			_config = config;
		}

		public void Process(File file, PipelineContext context)
		{
			if (file is not FileWithMetadata m) return;
			if (m.IsPost) return;
			if (!System.IO.Path.GetExtension(m.LocalPath).Equals(_config.HtmlExtension, StringComparison.InvariantCultureIgnoreCase)) return;
			if (m.LocalPath.EndsWith(_config.IndexHtml, StringComparison.InvariantCultureIgnoreCase)) return;
			if (m.LocalPath.EndsWith("404.html", StringComparison.InvariantCultureIgnoreCase)) return;

			m.LocalPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(m.LocalPath), System.IO.Path.GetFileNameWithoutExtension(m.LocalPath), _config.IndexHtml);
		}
	}
}
