using Fluid;
using Fluid.Values;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NJekyll.Model;

namespace NJekyll.Utilities
{
	public class LiquidCompiler : ITemplateCompiler
	{
		private readonly Dictionary<string, FileWithMetadata> _layouts;
		private readonly Dictionary<string, FileWithMetadata> _includes;

		public LiquidCompiler(Dictionary<string, FileWithMetadata> layouts, Dictionary<string, FileWithMetadata> includes)
		{
			_layouts = layouts;
			_includes = includes;
		}

		public string Compile(FileWithMetadata file, object site, object paginator = null)
		{
			var model = new Dictionary<string, object>
			{
				{ "page", file.Variables },
				{ "site", site },
				{ "paginator", paginator }
			};

			var currentFile = file;
			var content = Compile(file.Content, model);
			while (true)
			{
				if (string.IsNullOrEmpty(currentFile.Layout)) break;

				var currentLayout = _layouts[currentFile.Layout];
				model["content"] = content;
				content = Compile(currentLayout.Content, model);
				currentFile = currentLayout;
			}

			return content;
		}

		private string Compile(string content, Dictionary<string, object> model)
		{
			var parser = new FluidParser();
			var template = parser.Parse(content);
			var context = new TemplateContext();
			context.CultureInfo = new CultureInfo("es-ES");
			context.Options.Filters.AddFilter("date_to_string", DateToString);
			context.Options.Filters.AddFilter("remove_chars", RemoveChars);

			model.ToList().ForEach(kvp => context.SetValue(kvp.Key, kvp.Value));
			context.Options.FileProvider = new IncludesProvider(_includes);

			return template.Render(context);
		}

		static ValueTask<FluidValue> DateToString(FluidValue input, FilterArguments arguments, TemplateContext context)
		{
			if (input.IsNil()) return new StringValue("");

			var date = (DateTimeOffset)input.ToObjectValue();
			var culture = context.CultureInfo;
			return new StringValue(date.ToString("dd MMM yyyy", culture));
		}

		static ValueTask<FluidValue> RemoveChars(FluidValue input, FilterArguments arguments, TemplateContext context)
		{
			if (input.IsNil()) return new StringValue("");

			var str = input.ToStringValue();
			str = System.Web.HttpUtility.HtmlDecode(str);
			var tempBytes = Encoding.GetEncoding("ISO-8859-8").GetBytes(str);
			str = Encoding.UTF8.GetString(tempBytes);
			var sb = new StringBuilder();
			foreach (char c in str)
			{
				var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
				if (unicodeCategory != UnicodeCategory.NonSpacingMark)
				{
					if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ')
						sb.Append(c);
				}
			}

			return new StringValue(sb.ToString());
		}

		class IncludesProvider : IFileProvider
		{
			private readonly Dictionary<string, FileWithMetadata> _includes;

			public IncludesProvider(Dictionary<string, FileWithMetadata> includes)
			{
				_includes = includes;
			}

			public IDirectoryContents GetDirectoryContents(string subpath)
			{
				throw new NotImplementedException();
			}

			public IFileInfo GetFileInfo(string subpath)
			{
				if (subpath.EndsWith(".liquid"))
					subpath = subpath.Substring(0, subpath.Length - 7);

				if (_includes.ContainsKey(subpath))
				{
					return new FileInfo { Exists = true, Content = _includes[subpath].Content };
				}

				return new FileInfo { Exists = false };
			}

			public IChangeToken Watch(string filter)
			{
				throw new NotImplementedException();
			}
		}

		class FileInfo : IFileInfo
		{
			public bool Exists { get; set; }

			public long Length => Content?.Length ?? 0;

			public string PhysicalPath { get; set; }

			public string Name { get; set; }

			public DateTimeOffset LastModified { get; set; }

			public bool IsDirectory { get; set; }

			public string Content { get; set; }

			public Stream CreateReadStream()
			{
				var stream = new MemoryStream();
				var writer = new StreamWriter(stream);
				writer.Write(Content);
				writer.Flush();
				stream.Position = 0;
				return stream;
			}
		}
	}
}
