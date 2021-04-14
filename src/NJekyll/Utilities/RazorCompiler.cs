using RazorEngineCore;
using System.Collections.Generic;
using NJekyll.Model;

namespace NJekyll.Utilities
{
	public class RazorCompiler : ITemplateCompiler
	{
		private readonly RazorEngine _engine;
		private readonly Dictionary<string, IRazorEngineCompiledTemplate<RazorCompilerTemplateBase>> _layouts;
		private readonly Dictionary<string, IRazorEngineCompiledTemplate<RazorCompilerTemplateBase>> _includes;

		public RazorCompiler(RazorEngine engine, Dictionary<string, IRazorEngineCompiledTemplate<RazorCompilerTemplateBase>> layouts, Dictionary<string, IRazorEngineCompiledTemplate<RazorCompilerTemplateBase>> includes)
		{
			_engine = engine;
			_layouts = layouts;
			_includes = includes;
		}

		public string Compile(FileWithMetadata file, object site, object paginator = null)
		{
			var template = _engine.Compile(file);
			var compiler = new TemplateCompiler(template, _layouts, _includes);
			return compiler.Run(file.Variables.ToDynamic(), site);
		}

		class TemplateCompiler
		{
			private readonly IRazorEngineCompiledTemplate<RazorCompilerTemplateBase> _compiledTemplate;
			private readonly Dictionary<string, IRazorEngineCompiledTemplate<RazorCompilerTemplateBase>> _layouts;
			private readonly Dictionary<string, IRazorEngineCompiledTemplate<RazorCompilerTemplateBase>> _includes;

			public TemplateCompiler(IRazorEngineCompiledTemplate<RazorCompilerTemplateBase> compiledTemplate, Dictionary<string, IRazorEngineCompiledTemplate<RazorCompilerTemplateBase>> layouts, Dictionary<string, IRazorEngineCompiledTemplate<RazorCompilerTemplateBase>> includes)
			{
				_compiledTemplate = compiledTemplate;
				_layouts = layouts;
				_includes = includes;
			}

			public string Run(object model, object site)
			{
				return Run(_compiledTemplate, model, site);
			}

			public string Run(IRazorEngineCompiledTemplate<RazorCompilerTemplateBase> template, object model, object site, string previousRendered = null)
			{
				RazorCompilerTemplateBase templateReference = null;

				var result = template.Run(instance =>
				{
					instance.Model = model;
					instance.Site = site;
					instance.IncludeCallback = (key, includeModel) => Run(_includes[key], includeModel ?? model, site);
					instance.RenderBodyCallback = () => previousRendered;
					templateReference = instance;
				});

				if (string.IsNullOrWhiteSpace(templateReference.Layout))
				{
					return result;
				}

				return Run(_layouts[templateReference.Layout], model, site, result);
			}
		}
	}
}
