using System;
using NJekyll.Model;

namespace NJekyll.Core.Preprocessors
{
	public class PostLocalPath : IFileProcessor
	{
		private readonly Config _config;

		public PostLocalPath(Config config)
		{
			_config = config;
		}

		public void Process(File file, PipelineContext context)
		{
			if (file is not FileWithMetadata m) return;
			if (!m.IsPost) return;

			var localPath = m.LocalPath;
			var date = localPath.Substring(0, 10);
			if (DateTime.TryParse(date, out var d))
			{
				localPath = localPath.Substring(11);
				localPath = System.IO.Path.Combine(d.Year.ToString("0000"), d.Month.ToString("00"), d.Day.ToString("00"), System.IO.Path.GetFileNameWithoutExtension(localPath), _config.IndexHtml);
			}

			if (m.Category != null)
			{
				localPath = System.IO.Path.Combine(m.Category, localPath);
			}

			m.LocalPath = localPath;
		}
	}
}
