using System.Collections.Generic;

namespace NJekyll.Utilities
{
	public interface IYamlDeserializer
	{
		Dictionary<string, object> Parse(string content);
	}
}