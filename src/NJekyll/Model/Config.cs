using System.IO;

namespace NJekyll.Model
{
	public class Config
	{
		public readonly string ConfigFile = "_config.yml";
		public readonly string HtmlExtension = ".html";
		public readonly string RazorExtension = ".cshtml";
		public readonly string MarkdownExtension = ".md";
		public readonly string SassExtension = ".scss";
		public readonly string CssExtension = ".css";
		public readonly string JavascriptExtension = ".js";
		public readonly string IndexFile = "index";
		public readonly string UrlSeparator = "/";
		public readonly string IndexHtml = "index.html";

		public string SitePath { get; set; }
		public string OutputPath { get; set; }

		public string LayoutsFolder { get; set; } = "_layouts";
		public string IncludesFolder { get; set; } = "_includes";
		public string PostsFolder { get; set; } = "_posts";
		public string SassFolder { get; set; } = "_sass";

		public string HeaderSeparator { get; set; } = "---";
		public string ExcerptSeparator { get; set; } = "<!--break-->";

		public string BaseLayout { get; set; } = "default.cshtml";
		public string TagsKey { get; set; } = "tags";
		public string CategoryKey { get; set; } = "categories";
		public string TitleKey { get; set; } = "title";
		public string LayoutKey { get; set; } = "layout";
		public string DateKey { get; set; } = "post_date";
		public string PaginateKey { get; set; } = "paginate";
		public string TimeToReadKey { get; set; } = "time_to_read";
		public string ExcerptKey { get; set; } = "excerpt";
		public string ContentKey { get; set; } = "content";
		public string UrlKey { get; set; } = "url";
		public string IdKey { get; set; } = "ID";

		public string PostsPath { get => Path.Combine(SitePath, PostsFolder); }
	}
}
