using System;
using System.Linq;
using NJekyll.Model;
using NJekyll.Utilities;

namespace NJekyll.Core.Processors
{
	public class SitePages : IProcessor
	{
		private readonly Config _config;

		public SitePages(Config config)
		{
			_config = config;
		}

		public void Process(PipelineContext context)
		{
			var pages = context.NonStaticFiles
							   .Where(x => !x.IsPost)
							   .Where(x => System.IO.Path.GetExtension(x.LocalPath).Equals(_config.HtmlExtension, StringComparison.Ordinal))
							   .OrderBy(x => x.LocalPath)
							   .Select(x => x.Variables)
							   .ToList();

			context.Site["pages"] = pages;
		}
	}
}
