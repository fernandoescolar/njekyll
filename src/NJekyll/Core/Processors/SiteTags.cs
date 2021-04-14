using System.Collections.Generic;
using System.Linq;
using NJekyll.Model;

namespace NJekyll.Core.Processors
{
	public class SiteTags : IProcessor
	{
		private readonly Config _config;

		public SiteTags(Config config)
		{
			_config = config;
		}

		public void Process(PipelineContext context)
		{
			var tagsWeight = context.NonStaticFiles
							   .Where(x => x.IsPost)
							   .SelectMany(x => x.Tags ?? new string[0])
							   .GroupBy(x => x)
							   .Select(x => new { tag = x.Key, count = x.Count() })
							   .OrderBy(x => x.tag)
							   .ToList();
			

			var tags = tagsWeight.ToDictionary(t => t.tag, t => context.NonStaticFiles.Where(x => x.IsPost && (x.Tags?.Any(y => y == t.tag) ?? false)).ToList());
			context.Site["tags"] = tags;

			var tags_weight = tagsWeight.Select(x => new Dictionary<string, object> { { "tag", x.tag }, { "count", x.count } }).ToList();
			context.Site["tags_weight"] = tags_weight;
		}
	}
}
