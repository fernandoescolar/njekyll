using System.Collections.Generic;
using System.Linq;
using NJekyll.Model;

namespace NJekyll.Core.Preprocessors
{
	public class Tags : IFileProcessor
	{
		private readonly Config _config;

		public Tags(Config config)
		{
			_config = config;
		}

		public void Process(File file, PipelineContext context)
		{
			if (file is not FileWithMetadata m) return;
			if (m.Variables == null) return;
			if (!m.Variables.ContainsKey(_config.TagsKey)) return;

			if (m.Variables[_config.TagsKey] is string s)
			{
				m.Variables[_config.TagsKey] = new List<string>(s.Split(" "));
			}
			else
			{
				m.Variables[_config.TagsKey] = ((List<object>)m.Variables[_config.TagsKey]).Cast<string>().ToList();
			}

			m.Tags = (List<string>)m.Variables[_config.TagsKey];
		}
	}
}
