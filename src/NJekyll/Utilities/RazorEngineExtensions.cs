using RazorEngineCore;
using System.Text;
using NJekyll.Model;

namespace NJekyll.Utilities
{
	public static class RazorEngineExtensions
    {
        public static IRazorEngineCompiledTemplate<RazorCompilerTemplateBase> Compile(this RazorEngine engine, FileWithMetadata file)
        {
            if (!string.IsNullOrWhiteSpace(file.Layout))
            {
                var content = new StringBuilder();
                content.AppendLine("@{");
                content.Append(" Layout = ");
                content.Append('"');
                content.Append(file.Layout);
                content.Append('"');
                content.AppendLine(";");
                content.AppendLine("}");
                content.Append(file.Content);
                return engine.Compile<RazorCompilerTemplateBase>(content.ToString());
            }

            return engine.Compile<RazorCompilerTemplateBase>(file.Content);
        }
    }
}
