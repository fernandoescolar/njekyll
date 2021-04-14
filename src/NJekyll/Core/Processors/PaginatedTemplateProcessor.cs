using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NJekyll.Model;
using NJekyll.Utilities;

namespace NJekyll.Core.Processors
{
	public class PaginatedTemplateProcessor : IProcessor
	{
		private readonly Config _config;
		private readonly ITemplateCompilerFactory _templateCompilerFactory;

		public PaginatedTemplateProcessor(Config config, ITemplateCompilerFactory templateCompilerFactory)
		{
			_config = config;
			_templateCompilerFactory = templateCompilerFactory;
		}
		public void Process(PipelineContext context)
		{
			var files = new List<FileWithMetadata>();
			var templateCompiler = _templateCompilerFactory.Create(context.Layouts, context.Includes);
			object site = templateCompiler is RazorCompiler ? context.Site.ToDynamic() : context.Site;

			context.NonStaticFiles
				   .Where(x => x.Paginate.HasValue)
				   .AsParallel().ForAll(item =>
				   {
					   var pages = ((IEnumerable)context.Site["posts"]).Cast<object>().Window(item.Paginate.Value).ToList();
					   var content = item.Content;
					   for (var i = 0; i < pages.Count; i++)
					   {
						   var file = i == 0 ? item : new FileWithMetadata
						   {
							   LocalPath = item.LocalPath.Replace(_config.IndexHtml, $"page{i + 1}{_config.UrlSeparator}{_config.IndexHtml}"),
							   Layout = item.Layout,
							   Variables = item.Variables,
							   Tags = item.Tags,
							   Category = item.Category,
							   Path = item.Path,
							   IsPost = item.IsPost,
							   Content = content
						   };

						   var paginator = new Dictionary<string, object>
						   {
							   { "page", i + 1},
							   { "posts", pages[i]},
							   { "next_page", i < pages.Count - 1 ? $"{i + 2}" : null},
							   { "previous_page",i > 0 ? $"{i}" : null }
						   };

						   file.Content = templateCompiler.Compile(file, site, paginator);

						   if (i > 0)
						   {
							   files.Add(file);
						   }
							
					   }
				   });

			if (files.Count > 0)
			{
				var list = context.NonStaticFiles.ToList();
				list.AddRange(files);
				context.NonStaticFiles = list;
			}
		}
	}
}
