using SharpScss;
using System;
using NJekyll.Model;
using NJekyll.Utilities;

namespace NJekyll.Core.Preprocessors
{
	public class Sass : IFileProcessor
	{
		private readonly Config _config;

		public Sass(Config config, IMarkdownRenderer renderer)
		{
			_config = config;
		}

		public void Process(File file, PipelineContext context)
		{
			if (file is not FileWithMetadata m) return;

			var isSass = System.IO.Path.GetExtension(m.Path).Equals(_config.SassExtension, StringComparison.InvariantCultureIgnoreCase);
			if (!isSass) return;

			m.LocalPath = System.IO.Path.ChangeExtension(m.LocalPath, _config.CssExtension);
			var result = Scss.ConvertToCss(m.Content, new ScssOptions
			{
				InputFile = System.IO.Path.GetFileName(m.Path),
				OutputFile = System.IO.Path.ChangeExtension(System.IO.Path.GetFileName(m.Path), _config.CssExtension),
				TryImport = (ref string file, string path, out string scss, out string map) =>
				{
					map = null;
					scss = null;

					var paths = new[]
					{
						System.IO.Path.Combine(_config.SitePath, _config.SassFolder,  file + _config.SassExtension),
						System.IO.Path.Combine(System.IO.Path.GetDirectoryName(m.Path), file + _config.SassExtension),
					};

					foreach (var p in paths)
					{
						if (System.IO.File.Exists(p))
						{
							path = p;
							scss = System.IO.File.ReadAllText(p);
							return true;
						}
					}

					return false;
				}
			});

			m.Content = result.Css;
		}
	}
}
