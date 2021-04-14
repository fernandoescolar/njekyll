using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace NJekyll.Utilities
{
	public static class RazorVariablesExtensions
	{
		public static DynamicObject ToDynamic(this Dictionary<string, object> dictionary)
		{
			var newDictionary = new Dictionary<string, object>();
			foreach (var kvp in dictionary)
			{
				var key = kvp.Key.ToPascalCase();
				var value = kvp.Value is Dictionary<string, object> d ? d.ToDynamic()
						  : kvp.Value is IEnumerable<object> e ? e.ToDynamic().ToList()
						  : kvp.Value;
				newDictionary.Add(key, value);
			}

			return new ExpandoObjectWithoutExceptions(newDictionary);
		}

		public static IEnumerable<object> ToDynamic(this IEnumerable<object> enumerable)
		{
			foreach (var item in enumerable)
			{
				if (item is Dictionary<string, object> d)
				{
					yield return d.ToDynamic();
				}
				else
				{
					yield return item;
				}
			}
		}

		class ExpandoObjectWithoutExceptions : DynamicObject
		{
			private readonly Dictionary<string, object> _innerDictionary;

			public ExpandoObjectWithoutExceptions() : this(new Dictionary<string, object>())
			{
			}

			public ExpandoObjectWithoutExceptions(Dictionary<string, object> dictionary)
			{
				_innerDictionary = dictionary;
			}

			public override bool TryGetMember(GetMemberBinder binder, out object result)
			{
				_innerDictionary.TryGetValue(binder.Name, out result);
				return true;
			}

			public override bool TrySetMember(SetMemberBinder binder, object value)
			{
				_innerDictionary[binder.Name] = value;
				return true;
			}
		}
	}
}
