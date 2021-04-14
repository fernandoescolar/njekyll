using System;
using NJekyll.Model;

namespace NJekyll.Core.Preprocessors
{
	public class Url : IFileProcessor
	{
		private readonly Config _config;

		public Url(Config config)
		{
			_config = config;
		}

		public void Process(File file, PipelineContext context)
		{
			if (file is not FileWithMetadata m) return;
			if (m.Variables == null) return;

			var url = System.IO.Path.GetExtension(m.LocalPath).Equals(_config.HtmlExtension, StringComparison.Ordinal)
				? System.IO.Path.ChangeExtension(m.LocalPath, null)
				: m.LocalPath;
			url = url.Replace(System.IO.Path.DirectorySeparatorChar.ToString(), _config.UrlSeparator);
			url = url.EndsWith(_config.IndexFile, StringComparison.Ordinal)
				? url.Substring(0, Math.Max(url.Length - (_config.IndexFile.Length + 1), 0))
				: url;
			url = $"{_config.UrlSeparator}{url}";

			m.Variables.Add(_config.UrlKey, url);
		}
	}
}
