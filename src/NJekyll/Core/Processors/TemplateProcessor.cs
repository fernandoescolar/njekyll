using System.Linq;
using NJekyll.Utilities;

namespace NJekyll.Core.Processors
{
	public class TemplateProcessor : IProcessor
	{
		private readonly ITemplateCompilerFactory _templateCompilerFactory;

		public TemplateProcessor(ITemplateCompilerFactory razorFactory)
		{
			_templateCompilerFactory = razorFactory;
		}
		public void Process(PipelineContext context)
		{
			var templateCompiler = _templateCompilerFactory.Create(context.Layouts, context.Includes);
			object site = templateCompiler is RazorCompiler ? context.Site.ToDynamic() : context.Site;

			context.NonStaticFiles
				   .Where(x => !x.Paginate.HasValue)
				   .AsParallel().ForAll(item =>
				   {
						item.Content = templateCompiler.Compile(item, site);
				   });
		}
	}
}
