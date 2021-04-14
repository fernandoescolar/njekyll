using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;

namespace NJekyll
{
	public class WebServer
	{
		public static void Run()
		{
			CreateHostBuilder().Build().Run();
		}

		public static IHostBuilder CreateHostBuilder() =>
			Host.CreateDefaultBuilder()
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				})
			    .ConfigureLogging((context, logBuilder) =>
				{
					logBuilder.ClearProviders();
				});

		class Startup
		{
			public void ConfigureServices(IServiceCollection services)
			{
				services.AddResponseCompression();
			}

			public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
			{
				app.UseResponseCompression();

				var op = new StaticFileOptions
				{
					FileProvider = new PhysicalFileProvider(@"C:\projects\fernandoescolar.github.io\_output"),
				};

				app.Use(async (context, next) =>
				{
					if (string.IsNullOrEmpty(Path.GetExtension(context.Request.Path)))
					{
						if (!context.Request.Path.ToString().EndsWith("/"))
							context.Request.Path += "/";

						context.Request.Path += "index.html";
					}

					await next();

					if (context.Response.StatusCode == 404)
					{
						context.Request.Path = "/404.html";
						await next();
					}
				});

				app.UseStaticFiles(op);
			}
		}
	}
}
