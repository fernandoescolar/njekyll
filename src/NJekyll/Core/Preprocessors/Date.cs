using System;
using NJekyll.Model;

namespace NJekyll.Core.Preprocessors
{
	public class Date : IFileProcessor
	{
		private readonly Config _config;

		public Date(Config config)
		{
			_config = config;
		}

		public void Process(File file, PipelineContext context)
		{
			if (file is not FileWithMetadata m) return;
			if (m.Variables == null) return;
			if (!m.Variables.ContainsKey(_config.DateKey)) return;

			m.Variables["date"] = DateTime.TryParse((string)m.Variables[_config.DateKey], out var d) ? d : new DateTime(1900, 1, 1);
			m.Date = (DateTime)m.Variables["date"];
		}
	}
}
