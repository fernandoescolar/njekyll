using Microsoft.Extensions.DependencyInjection;
using RazorEngineCore;
using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using NJekyll.Model;
using NJekyll.Utilities;

namespace NJekyll.Core
{
	public class PipelineBuilder
	{
		private readonly IServiceCollection _services = new ServiceCollection();
		private readonly List<Type> _initializers = new List<Type>();
		private readonly List<Type> _preprocessors = new List<Type>();
		private readonly List<Type> _processors = new List<Type>();
		private readonly List<Type> _postprocessors = new List<Type>();
		private readonly List<Type> _savers = new List<Type>();
		private readonly Lazy<IServiceProvider> _provider;

		public PipelineBuilder() : this(null)
		{ 
		}

		public PipelineBuilder(Action<Config> configure)
		{
			_provider = new Lazy<IServiceProvider>(() => _services.BuildServiceProvider());
			_services.AddScoped(s => 
			{
				var config = ActivatorUtilities.CreateInstance<Config>(s);
				configure?.Invoke(config);
				return config;
			});
			_services.AddScoped<IFileFactory, FileFactory>();
			_services.AddScoped<IFileFinder, FileFinder>();
			_services.AddScoped<IMarkdownRenderer, MarkdownRenderer>();
			_services.AddScoped<IYamlDeserializer, YamlDeserializer>();
			_services.AddScoped(s => ActivatorUtilities.CreateInstance<Serializer>(s));


			//_services.AddScoped<ITemplateCompilerFactory, RazorCompilerFactory>();
			//_services.AddScoped(s => ActivatorUtilities.CreateInstance<RazorEngine>(s));
			_services.AddScoped<ITemplateCompilerFactory, LiquidCompilerFactory>();
		}

		public PipelineBuilder Initialize(Action<PipelineBuilderAggregator<IProcessor>> configure)
		{
			var aggregator = new PipelineBuilderAggregator<IProcessor>(_services, _initializers);
			configure?.Invoke(aggregator);
			return this;
		}

		public PipelineBuilder Preprocess(Action<PipelineBuilderAggregator<IFileProcessor>> configure)
		{
			var aggregator = new PipelineBuilderAggregator<IFileProcessor>(_services, _preprocessors);
			configure?.Invoke(aggregator);
			return this;
		}

		public PipelineBuilder Process(Action<PipelineBuilderAggregator<IProcessor>> configure)
		{
			var aggregator = new PipelineBuilderAggregator<IProcessor>(_services, _processors);
			configure?.Invoke(aggregator);
			return this;
		}

		public PipelineBuilder Postprocess(Action<PipelineBuilderAggregator<IFileProcessor>> configure)
		{
			var aggregator = new PipelineBuilderAggregator<IFileProcessor>(_services, _postprocessors);
			configure?.Invoke(aggregator);
			return this;
		}

		public PipelineBuilder FileSavers(Action<PipelineBuilderAggregator<IProcessor>> configure)
		{
			var aggregator = new PipelineBuilderAggregator<IProcessor>(_services, _savers);
			configure?.Invoke(aggregator);
			return this;
		}

		public Pipeline Build()
		{
			var initializers = _initializers.Select(t => (IProcessor)_provider.Value.GetService(t)).ToList();
			var preprocessors = _preprocessors.Select(t => (IFileProcessor)_provider.Value.GetService(t)).ToList();
			var processors = _processors.Select(t => (IProcessor)_provider.Value.GetService(t)).ToList();
			var postprocessors = _postprocessors.Select(t => (IFileProcessor)_provider.Value.GetService(t)).ToList();
			var savers = _savers.Select(t => (IProcessor)_provider.Value.GetService(t)).ToList();

			return ActivatorUtilities.CreateInstance<Pipeline>(_provider.Value, initializers, preprocessors, processors, postprocessors, savers);
		}

		public class PipelineBuilderAggregator<T>
		{
			private readonly IServiceCollection _services;
			private readonly List<Type> _list;
			
			public PipelineBuilderAggregator(IServiceCollection services, List<Type> list)
			{
				_services = services;
				_list = list;
			}

			public PipelineBuilderAggregator<T> With<S>() where S : class, T
			{
				_services.AddScoped<S>();
				_list.Add(typeof(S));
				return this;
			}

			public PipelineBuilderAggregator<T> Then<S>() where S : class, T
			{
				return With<S>();
			}

			public PipelineBuilderAggregator<T> And<S>() where S : class, T
			{
				return With<S>();
			}

			public PipelineBuilderAggregator<T> Using<S>() where S : class, T
			{
				return With<S>();
			}
		}
	}
}
