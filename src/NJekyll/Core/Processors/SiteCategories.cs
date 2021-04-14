using System.Linq;
using NJekyll.Model;

namespace NJekyll.Core.Processors
{
	public class SiteCategories : IProcessor
	{
		private readonly Config _config;

		public SiteCategories(Config config)
		{
			_config = config;
		}

		public void Process(PipelineContext context)
		{
			var tagsWeight = context.NonStaticFiles
							   .Where(x => x.IsPost && !string.IsNullOrEmpty(x.Category))
							   .GroupBy(x => x.Category)
							   .OrderBy(x => x.Key)
							   .ToDictionary(x => x.Key, x => x.Select(y => y.Variables).ToList());

			context.Site["categories"] = tagsWeight;
		}
	}
}
