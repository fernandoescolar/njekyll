using RazorEngineCore;
using System;

namespace NJekyll.Utilities
{
	public class RazorCompilerTemplateBase : RazorEngineTemplateBase
    {
        public string Layout { get; set; }

        public dynamic Site { get; set; }

        public Func<string, object, string> IncludeCallback { get; set; }

        public Func<string> RenderBodyCallback { get; set; }

        public string Include(string key, object model = null)
        {
            return this.IncludeCallback(key, model);
        }

        public string RenderBody()
        {
            return this.RenderBodyCallback();
        }

        public string RelativeUrl(string url)
        {
            var siteUrl = FixEndPath(Site.Url ?? "");
            var baseUrl = FixPath(Site.BaseUrl ?? "/");
            if (!string.IsNullOrEmpty(siteUrl))
            {
                if (!string.IsNullOrEmpty(baseUrl))
                {
                    siteUrl = $"{siteUrl}/{baseUrl}";
                }

                if (url.StartsWith(siteUrl))
                {
                    url = url.Substring(siteUrl.Length);
                }
            }
            
            if (!string.IsNullOrEmpty(baseUrl))
            {
                url = $"/{baseUrl}/{FixStartPath(url)}";
            }
            
            return url;
        }

        private static string FixPath(string urlPath)
        {
            return FixStartPath(FixEndPath(urlPath));
        }

        private static string FixStartPath(string urlPath)
        {
            if (urlPath.StartsWith('/'))
            {
                urlPath = urlPath.Substring(1);
            }

            return urlPath;
        }

        private static string FixEndPath(string urlPath)
        {
            if (urlPath.EndsWith('/'))
            {
                urlPath = urlPath.Substring(0, urlPath.Length - 1);
            }

            return urlPath;
        }
    }
}
