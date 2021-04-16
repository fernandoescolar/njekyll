using Markdig;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.ClearScript.V8;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

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
			using (var sw = new StringWriter())
			{
				var html = new HtmlRenderer(sw);
				html.ObjectRenderers.AddIfNotAlready<RawInlineRenderer>();
				Markdown.Convert(content, html, _pipeline);
				sw.Flush();
				return sw.ToString();

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
		private readonly CodeBlockRenderer _underlyingRenderer;

		public SyntaxHighlightingCodeBlockRenderer(CodeBlockRenderer underlyingRenderer = null)
		{
			_underlyingRenderer = underlyingRenderer ?? new CodeBlockRenderer();
		}

		private static readonly object EngineLocker = new object();
		private static readonly Lazy<V8ScriptEngine> Engine = new Lazy<V8ScriptEngine>(() =>
		{
			var path = Path.GetDirectoryName(typeof(SyntaxHighlightingCodeBlockRenderer).Assembly.Location);
			var runtime = new V8Runtime();
			var engine = runtime.CreateScriptEngine();
			var script = runtime.Compile(File.ReadAllText(Path.Combine(path, "prism.js")));
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

			var code = GetCode(obj);
			code = code.Trim();

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
				catch
				{
                    // do not format
				}
			}


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
}
