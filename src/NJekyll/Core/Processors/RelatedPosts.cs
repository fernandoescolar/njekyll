using System.Collections.Generic;
using System.Linq;

namespace NJekyll.Core.Processors
{
	public class RelatedPosts : IProcessor
	{
		public void Process(PipelineContext context)
		{
			context.Site["related_posts"] = context.Site["posts"] as IEnumerable<object>;

			var posts = context.NonStaticFiles.Where(x => x.IsPost).ToList();
			foreach (var post in posts)
			{
				var tags = post.Tags ?? new string[0];
				var related = posts.Where(x => x != post)
								   .Select(x => new { Post = x, Count = (x.Tags ?? new string[0]).Count(x => tags.Contains(x)) })
								   .OrderByDescending(x => x.Count)
								   .ThenByDescending(x => x.Post.Date)
								   .Select(x => x.Post.Variables)
								   .ToList();
				post.Variables["related_posts"] = related;
			}
		}
	}
}
