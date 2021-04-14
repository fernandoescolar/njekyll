using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using NJekyll.Core;
using NJekyll.Core.FileSavers;
using NJekyll.Core.Initializers;
using NJekyll.Core.Postprocessors;
using NJekyll.Core.Preprocessors;
using NJekyll.Core.Processors;

namespace NJekyll
{
	class Program
    {
        static int Main(string[] args)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var build = new Command("build", "Build your Jekyll static site.")
            {
                new Option<string>(new[] { "--input", "-i" }, () => System.IO.Directory.GetCurrentDirectory(), "The input directory (default: current directory)."),
                new Option<string>(new[] { "--output", "-o" }, "The output directory (default: _output)."),
            };

            var run = new Command("run", "Build and Run your Jekyll static site.")
            {
                new Option<string>(new[] { "--input", "-i" }, () => System.IO.Directory.GetCurrentDirectory(), "The input directory (default: current directory)."),
                new Option<string>(new[] { "--output", "-o" }, "The output directory (default: _output)."),
            };

            build.Handler = CommandHandler.Create<string, string, IConsole>(Build);
            run.Handler = CommandHandler.Create<string, string, IConsole>(Run);

            var cmd = new RootCommand
            {
                build,
                run
            };

            return cmd.Invoke(args);
        }

        static void Run(string input, string output, IConsole console) 
        {
            Build(input, output, console);

            console.Out.Write("Running web server\n - http://localhost:5000\n - https://localhost:5001\n");

            using var fileWatcher = new FileWatcher(input, s => Build(input, output, console), new[] { output ?? System.IO.Path.Combine(input, "_output") });
            WebServer.Run();
        }

        static void Build(string input, string output, IConsole console)
        {
            console.Out.Write("Building...");

            var pipeline = new PipelineBuilder(c => { c.SitePath = input; c.OutputPath = output ?? System.IO.Path.Combine(input, "_output"); })
                    .Initialize(c => c.With<Configure>()
                                      .Then<GetFiles>()
                                      .Then<GetIncludes>()
                                      .Then<GetLayouts>())
                    .Preprocess(c => c.With<Variables>()
                                      .Then<Date>()
                                      .And<Category>()
                                      .And<Layout>()
                                      .And<Tags>()
                                      .Then<Sass>()
                                      .Then<Markdown>()
                                      .Then<Content>()
                                      .Then<Excerpt>()
                                      .And<Paginate>()
                                      .And<TimeToRead>()
                                      .And<PostLocalPath>()
                                      .And<PageLocalPath>()
                                      .And<Url>())
                    .Process(c => c.With<SitePages>()
                                   .And<SitePosts>()
                                   .And<SiteTags>()
                                   .And<SiteCategories>()
                                   .Then<RelatedPosts>()
                                   .Using<TemplateProcessor>()
                                   .Using<PaginatedTemplateProcessor>())
                    .Postprocess(c => c.With<Html>()
                                       .And<Javascript>()
                                       .And<Css>())
                    .FileSavers(c => c.Using<NonStaticFiles>()
                                      .And<StaticFiles>())
                    .Build();
            var sw = new Stopwatch();
            sw.Start();
            pipeline.Execute();
            sw.Stop();

            console.Out.Write($" {sw.Elapsed}\n");
        }
    }
}
