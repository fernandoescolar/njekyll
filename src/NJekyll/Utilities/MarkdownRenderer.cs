using Highlight;
using Highlight.Engines;
using Highlight.Patterns;
using Markdig;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.ClearScript.V8;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace NJekyll.Utilities
{
	public class MarkdownRenderer : IMarkdownRenderer
	{
		private readonly MarkdownPipelineBuilder _builder;
		private MarkdownPipeline _pipeline;

		public MarkdownRenderer()
		{
			_builder = new MarkdownPipelineBuilder()
				.UseEmojiAndSmiley()
				.UseEmphasisExtras()
				.UseAdvancedExtensions()
				.UseSyntaxHighlighting()
				.UsePipeTables();

			_builder.DocumentProcessed += PostRenderMarkdown;
			_pipeline = _builder.Build();
		}

		public string Render(string content)
		{
			try
			{

				using (var sw = new StringWriter())
				{
					var html = new HtmlRenderer(sw);
					html.ObjectRenderers.AddIfNotAlready<RawInlineRenderer>();
					Markdown.Convert(content, html, _pipeline);
					sw.Flush();
					return sw.ToString();

				}
			}
			catch (System.Exception ex)
			{

				throw ex;
			}
		}

		private void PostRenderMarkdown(MarkdownDocument document)
		{
			foreach (var item in document.Descendants())
			{
				switch (item)
				{
					case LiteralInline inline:
						{
							var newText = inline.ToString();
							inline.ReplaceBy(new RawInline(newText), true);
							break;
						}
					case HtmlInline inline:
						{
							var tag = inline.Tag;
							tag = Regex.Replace(tag, @"<(T[a-zA-Z0-9]*)>", "&lt;$1&gt;");
							inline.ReplaceBy(new RawInline(tag), true);
							break;
						}
					default:
						continue;
				}
			}
		}

		class RawInline : LeafInline
		{
			public string Text { get; }

			public RawInline(string text)
			{
				Text = text;
			}

		}

		class RawInlineRenderer : HtmlObjectRenderer<RawInline>
		{
			protected override void Write(HtmlRenderer renderer, RawInline obj)
			{
				renderer.Write(obj.Text);
			}
		}
	}

	public static class SyntaxHighlightingExtensions
	{
		public static MarkdownPipelineBuilder UseSyntaxHighlighting(this MarkdownPipelineBuilder pipeline)
		{
			pipeline.Extensions.Add(new SyntaxHighlightingExtension());
			return pipeline;
		}
	}

	public class SyntaxHighlightingExtension : IMarkdownExtension
	{
		public SyntaxHighlightingExtension()
		{
		}

		public void Setup(MarkdownPipelineBuilder pipeline) { }

		public void Setup(MarkdownPipeline pipeline, Markdig.Renderers.IMarkdownRenderer renderer)
		{
			if (renderer == null)
			{
				throw new ArgumentNullException(nameof(renderer));
			}

			var htmlRenderer = renderer as TextRendererBase<HtmlRenderer>;
			if (htmlRenderer == null)
			{
				return;
			}

			var originalCodeBlockRenderer = htmlRenderer.ObjectRenderers.FindExact<CodeBlockRenderer>();
			if (originalCodeBlockRenderer != null)
			{
				htmlRenderer.ObjectRenderers.Remove(originalCodeBlockRenderer);
			}

			htmlRenderer.ObjectRenderers.AddIfNotAlready(
				new SyntaxHighlightingCodeBlockRenderer(originalCodeBlockRenderer));
		}
	}

	public class SyntaxHighlightingCodeBlockRenderer : HtmlObjectRenderer<CodeBlock>
	{
		private static readonly Dictionary<string, string> Languages = new Dictionary<string, string>
		{
			{ "csharp", "C#" },
			{ "c#", "C#" },
			{ "c", "C" },
			{ "cpp", "C++" },
			{ "asp", "ASPX" },
			{ "aspx", "ASPX" },
			{ "cobol", "COBOL" },
			{ "eiffel", "Eiffel" },
			{ "fortran", "Fortran" },
			{ "haskell", "Haskell" },
			{ "hs", "Haskell" },
			{ "html", "HTML" },
			{ "atom", "HTML" },
			{ "rss", "HTML" },
			{ "java", "Java" },
			{ "js", "JavaScript" },
			{ "javascript", "JavaScript" },
			{ "mercury", "Mercury" },
			{ "msil", "MSIL" },
			{ "pascal", "Pascal" },
			{ "objectpascal", "Pascal" },
			{ "perl", "Perl" },
			{ "php", "PHP" },
			{ "python", "Python" },
			{ "py", "Python" },
			{ "ruby", "Ruby" },
			{ "rb", "Ruby" },
			{ "sql", "SQL" },
			{ "vb", "Visual Basic" },
			{ "vba", "Visual Basic" },
			{ "vbs", "VBScript" },
			{ "vbnet", "VB.NET" },
			{ "xml", "XML" },
			{ "svg", "XML" },
			{ "ssml", "XML" },
			{ "markup", "XML" },
			{ "mathml", "XML" },
		};

		private readonly CodeBlockRenderer _underlyingRenderer;

		public SyntaxHighlightingCodeBlockRenderer(CodeBlockRenderer underlyingRenderer = null)
		{
			_underlyingRenderer = underlyingRenderer ?? new CodeBlockRenderer();
		}

		private static readonly object EngineLocker = new object();
		private static readonly Lazy<V8ScriptEngine> Engine = new Lazy<V8ScriptEngine>(() =>
		{
			var runtime = new V8Runtime();
			var engine = runtime.CreateScriptEngine();
			//var script = runtime.Compile(File.ReadAllText("highlight.pack.js"));
			var script = runtime.Compile(File.ReadAllText("prism.js"));
			engine.Evaluate(script);

			return engine;
		});

		protected override void Write(HtmlRenderer renderer, CodeBlock obj)
		{
			var fencedCodeBlock = obj as FencedCodeBlock;
			var parser = obj.Parser as FencedCodeBlockParser;
			if (fencedCodeBlock == null || parser == null)
			{
				_underlyingRenderer.Write(renderer, obj);
				return;
			}

			var attributes = obj.TryGetAttributes() ?? new HtmlAttributes();

			var languageMoniker = fencedCodeBlock.Info.Replace(parser.InfoPrefix, string.Empty);
			if (string.IsNullOrEmpty(languageMoniker))
			{
				_underlyingRenderer.Write(renderer, obj);
				return;
			}

			attributes.AddClass($"highlight");
			//attributes.Classes.Remove($"language-{languageMoniker}");

			var code = GetCode(obj);
			code = code.Trim();

			//			var testCode = $@"
			//code = `{code.Replace("`", "\\`").Replace("$", "\\$")}`;
			//var html = hljs.highlight(code, {{language: '{languageMoniker}'}}).value";
			//			try
			//			{
			//				Engine.Value.Execute(testCode);
			//				code = Engine.Value.Script.html;
			//			}
			//			catch (Exception ex)
			//			{

			//			}

			var testCode = $@"
			code = `{code.Replace("`", "\\`").Replace("$", "\\$")}`;
			var html = Prism.highlight(code, Prism.languages['{languageMoniker}'], '{languageMoniker}');";
			lock (EngineLocker)
			{
				try

				{

					Engine.Value.Execute(testCode);
					code = Engine.Value.Script.html;

				}
				catch (Exception ex)
				{

				}
			}



			//if (Languages.ContainsKey(languageMoniker))
			//{
			//	var highlighter = new Highlighter(new HtmlCssEngine());
			//	code = highlighter.Highlight(Languages[languageMoniker], code);
			//}
			//else
			//{
			//	code = HttpUtility.HtmlEncode(code);
			//}

			renderer
				.Write("<pre")
				.WriteAttributes(attributes)
				.WriteLine(">");
			renderer.Write("<code>");
			renderer.Write(code);
			renderer.WriteLine("</code>");
			renderer.WriteLine("</pre>");
		}

		private static string GetCode(LeafBlock obj)
		{
			var code = new StringBuilder();
			foreach (var line in obj.Lines.Lines)
			{
				var slice = line.Slice;
				if (slice.Text == null)
				{
					continue;
				}

				var lineText = slice.Text.Substring(slice.Start, slice.Length);
				code.AppendLine();
				code.Append(lineText);
			}
			return code.ToString();
		}
	}

	public class HtmlCssEngine : Engine
	{
		private const string CLASS_SPAN_FORMAT = "<span class=\"{0}\">{1}</span>";

		protected override string PostHighlight(Definition definition, string input)
		{
			if (definition == null)
				throw new ArgumentNullException(nameof(definition));

			//var cssClassName = HtmlEngineHelper.CreateCssClassName(definition.Name, null);
			//return string.Format(CLASS_SPAN_FORMAT, cssClassName, input);
			return input;
		}

		protected override string PreHighlight(Definition definition, string input)
		{
			if (definition == null)
				throw new ArgumentNullException(nameof(definition));

			return HttpUtility.HtmlEncode(input);
		}

		protected override string ProcessBlockPatternMatch(Definition definition, BlockPattern pattern, Match match)
		{
			if (definition == null)
				throw new ArgumentNullException(nameof(definition));

			if (pattern == null)
				throw new ArgumentNullException(nameof(pattern));

			if (match == null)
				throw new ArgumentNullException(nameof(match));


			var cssClassName = HtmlEngineHelper.CreateCssClassName(definition.Name, pattern.Name);
			return string.Format(CLASS_SPAN_FORMAT, cssClassName, match.Value);
		}

		protected override string ProcessMarkupPatternMatch(Definition definition, MarkupPattern pattern, Match match)
		{
			if (definition == null)
				throw new ArgumentNullException(nameof(definition));

			if (pattern == null)
				throw new ArgumentNullException(nameof(pattern));

			if (match == null)
				throw new ArgumentNullException(nameof(match));

			var result = new StringBuilder();
			var cssClassName = HtmlEngineHelper.CreateCssClassName(definition.Name, pattern.Name + "Bracket");

			result.AppendFormat
			(
				CLASS_SPAN_FORMAT,
				cssClassName,
				match.Groups["openTag"]
					 .Value
			);

			result.Append
			(
				match.Groups["ws1"]
					 .Value
			);

			cssClassName = HtmlEngineHelper.CreateCssClassName(definition.Name, pattern.Name + "TagName");

			result.AppendFormat
			(
				CLASS_SPAN_FORMAT,
				cssClassName,
				match.Groups["tagName"]
					 .Value
			);

			if (pattern.HighlightAttributes)
			{
				var highlightedAttributes = ProcessMarkupPatternAttributeMatches(definition, pattern, match);
				result.Append(highlightedAttributes);
			}

			result.Append
			(
				match.Groups["ws5"]
					 .Value
			);

			cssClassName = HtmlEngineHelper.CreateCssClassName(definition.Name, pattern.Name + "Bracket");

			result.AppendFormat
			(
				CLASS_SPAN_FORMAT,
				cssClassName,
				match.Groups["closeTag"]
					 .Value
			);

			return result.ToString();
		}

		protected override string ProcessWordPatternMatch(Definition definition, WordPattern pattern, Match match)
		{

			var cssClassName = HtmlEngineHelper.CreateCssClassName(definition.Name, pattern.Name);
			return string.Format(CLASS_SPAN_FORMAT, cssClassName, match.Value);
		}

		private string ProcessMarkupPatternAttributeMatches(Definition definition, MarkupPattern pattern, Match match)
		{
			var result = new StringBuilder();

			for (var i = 0;
				i <
				match.Groups["attribute"]
					 .Captures.Count;
				i++)
			{
				result.Append
				(
					match.Groups["ws2"]
						 .Captures[i]
						 .Value
				);

				var cssClassName = HtmlEngineHelper.CreateCssClassName(definition.Name, pattern.Name + "AttributeName");

				result.AppendFormat
				(
					CLASS_SPAN_FORMAT,
					cssClassName,
					match.Groups["attribName"]
						 .Captures[i]
						 .Value
				);

				if (string.IsNullOrWhiteSpace
				(
					match.Groups["attribValue"]
						 .Captures[i]
						 .Value
				))
					continue;

				cssClassName = HtmlEngineHelper.CreateCssClassName(definition.Name, pattern.Name + "AttributeValue");

				result.AppendFormat
				(
					CLASS_SPAN_FORMAT,
					cssClassName,
					match.Groups["attribValue"]
						 .Captures[i]
						 .Value
				);
			}

			return result.ToString();
		}
	}

	internal static class HtmlEngineHelper
	{
		private static readonly Dictionary<string, string> Replacements = new Dictionary<string, string>
		{
			{ "modifier", "k" },
			{ "referencetype", "kt" },
			{ "operator", "p" },
			{ "keyword", "k" },
			{ "string", "s" },
		};
		public static string CreateCssClassName(string definition, string pattern)
		{
			var classname = pattern.ToLowerInvariant();
			if (Replacements.ContainsKey(classname))
				return Replacements[classname];

			return classname;
		}
	}
}
