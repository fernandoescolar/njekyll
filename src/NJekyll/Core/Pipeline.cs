using System.Collections.Generic;
using System.Linq;

namespace NJekyll.Core
{
	public class Pipeline
	{
		private readonly IEnumerable<IProcessor> _initializers;
		private readonly IEnumerable<IFileProcessor> _preprocessors;
		private readonly IEnumerable<IProcessor> _processors;
		private readonly IEnumerable<IFileProcessor> _postprocessors;
		private readonly IEnumerable<IProcessor> _savers;

		public Pipeline(IEnumerable<IProcessor> initializers, IEnumerable<IFileProcessor> preprocessors, IEnumerable<IProcessor> processors, IEnumerable<IFileProcessor> postprocessors, IEnumerable<IProcessor> savers)
		{
			_initializers = initializers;
			_preprocessors = preprocessors;
			_processors = processors;
			_postprocessors = postprocessors;
			_savers = savers;
		}

		public void Execute()
		{
			var context = new PipelineContext();
			_initializers.ToList().ForEach(x => ExecuteProcessor(x, context));
			_preprocessors.ToList().ForEach(x => ExecuteFileProcessor(x, context));
			_processors.ToList().ForEach(x => ExecuteProcessor(x, context));
			_postprocessors.ToList().ForEach(x => ExecuteFileProcessor(x, context));
			_savers.ToList().ForEach(x => ExecuteProcessor(x, context));
		}

		private void ExecuteProcessor(IProcessor processor, PipelineContext context)
		{
			processor.Process(context);
		}

		private void ExecuteFileProcessor(IFileProcessor fileProcessor, PipelineContext context)
		{
			context.Layouts?.AsParallel().ForAll(item =>
			{
				fileProcessor.Process(item.Value, context);
			});
			context.NonStaticFiles?.AsParallel().ForAll(item =>
			{
				fileProcessor.Process(item, context);
			});
		}
	}
}
