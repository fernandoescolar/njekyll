using System.Collections.Generic;
using System.Linq;
using NJekyll.Model;
using NJekyll.Utilities;

namespace NJekyll.Core.Initializers
{
	public class Configure : IProcessor
	{
		private readonly Config _config;
		private readonly IYamlDeserializer _yamlDeserializer;

		public Configure(Config config, IYamlDeserializer yamlDeserializer)
		{
			_config = config;
			_yamlDeserializer = yamlDeserializer;
		}

		public void Process(PipelineContext context)
		{
			var site = GetSiteConfig();
			AssignProperties(_config, site);

			context.Site = site;
		}

		private Dictionary<string, object> GetSiteConfig()
		{
			var siteConfigPath = System.IO.Path.Combine(_config.SitePath, _config.ConfigFile);
			var siteConfigContent = System.IO.File.ReadAllText(siteConfigPath);
			return _yamlDeserializer.Parse(siteConfigContent);
		}

		private static void AssignProperties(Config config, Dictionary<string, object> dictionary)
		{
			config.GetType()
				  .GetProperties()
				  .Where(x => x.CanWrite && x.PropertyType == typeof(string))
				  .ToList()
				  .ForEach(property =>
				  {
					  var name = property.Name.ToSnakeCase();
					  if (dictionary.ContainsKey(name) && dictionary[name] is string s)
					  {
						  property.SetValue(config, s);
					  }
				  });

		}
	}
}
