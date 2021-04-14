using System.Linq;
using NJekyll.Model;

namespace NJekyll.Core.Processors
{
	public class SitePosts : IProcessor
	{
		private readonly Config _config;

		public SitePosts(Config config)
		{
			_config = config;
		}

		public void Process(PipelineContext context)
		{
			var posts = context.NonStaticFiles
							   .Where(x => x.IsPost)
							   .OrderByDescending(x => x.Date)
							   .ThenByDescending(x => x.LocalPath)
							   .Select(x => x.Variables)
							   .ToList();

			context.Site["posts"] = posts;
			context.Site["related_posts"] = posts;
		}
	}
}
