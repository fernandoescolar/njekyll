using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NJekyll.Model;

namespace NJekyll.Utilities
{
	public class FileFinder : IFileFinder
	{
		private readonly Config _config;

		public FileFinder(Config config)
		{
			_config = config;
		}

		public IEnumerable<string> GetLayouts()
		{
			return Directory.EnumerateFiles(Path.Combine(_config.SitePath, _config.LayoutsFolder));
		}

		public IEnumerable<string> GetIncludes()
		{
			return Directory.EnumerateFiles(Path.Combine(_config.SitePath, _config.IncludesFolder));
		}

		public IEnumerable<string> GetFiles()
		{

			return EnumerateFiles(_config.SitePath)
					.Concat(EnumerateFiles(_config.PostsPath));
		}

		private static IEnumerable<string> EnumerateFiles(string path)
		{
			foreach (var item in Directory.EnumerateFiles(path).Where(IgnoreSystemFiles))
			{
				yield return item;
			}

			foreach (var d in Directory.EnumerateDirectories(path).Where(IgnoreSystemFiles))
			{
				foreach (var item in EnumerateFiles(d))
				{
					yield return item;
				}
			}
		}

		private static bool IgnoreSystemFiles(string path) =>
			   !Path.GetFileName(path).StartsWith(".", StringComparison.Ordinal)
			&& !Path.GetFileName(path).StartsWith("_", StringComparison.Ordinal);
	}
}
