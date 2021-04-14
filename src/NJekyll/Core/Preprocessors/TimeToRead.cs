using System;
using System.Text.RegularExpressions;
using NJekyll.Model;

namespace NJekyll.Core.Preprocessors
{
	public class TimeToRead : IFileProcessor
	{
		private readonly Config _config;

		public TimeToRead(Config config)
		{
			_config = config;
		}

		public void Process(File file, PipelineContext context)
		{
			if (file is not FileWithMetadata m) return;
			if (m.Variables == null) return;

			m.Variables.Add(_config.TimeToReadKey, CalculateTimeToRead(file.Content));
		}

		private static int CalculateTimeToRead(string text)
		{
			var words = Regex.Matches(text, @"\w+").Count;
			var time = words / 180.0;
			if (time < 1)
				time = 1;

			return (int)Math.Round(time);
		}
	}
}
