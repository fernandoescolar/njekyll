using SharpYaml.Serialization;
using System.Collections.Generic;

namespace NJekyll.Utilities
{
	public class YamlDeserializer : IYamlDeserializer
	{
		private Serializer _deserializer;

		public YamlDeserializer(Serializer deserializer)
		{
			_deserializer = deserializer;
		}

		public Dictionary<string, object> Parse(string content)
		{
			var data = _deserializer.Deserialize<Dictionary<object, object>>(content);
			return data != null
				? AsPropertiesDictionary(data)
				: new Dictionary<string, object>();
		}

		private Dictionary<string, object> AsPropertiesDictionary(Dictionary<object, object> dictionary)
		{
			var result = new Dictionary<string, object>();
			foreach (var item in dictionary)
			{
				var key = (string)item.Key;
				switch (item.Value)
				{
					case Dictionary<object, object> d:
						result.Add(key, AsPropertiesDictionary(d));
						break;
					default:
						result.Add(key, item.Value);
						break;
				}
			}

			return result;
		}
	}
}
