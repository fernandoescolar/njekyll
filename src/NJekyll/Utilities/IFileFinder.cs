using System.Collections.Generic;

namespace NJekyll.Utilities
{
	public interface IFileFinder
	{
		IEnumerable<string> GetFiles();
		IEnumerable<string> GetIncludes();
		IEnumerable<string> GetLayouts();
	}
}