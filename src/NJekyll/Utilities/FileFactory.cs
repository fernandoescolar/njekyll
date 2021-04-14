using System;
using System.Text;
using NJekyll.Model;

namespace NJekyll.Utilities
{
	public class FileFactory : IFileFactory
	{
		private const int BufferSize = 32 * 1024;

		private readonly Config _config;

		public FileFactory(Config config)
		{
			_config = config;
		}

		public File Create(string path)
		{
			var stream = System.IO.File.OpenRead(path);
			using (var reader = new System.IO.StreamReader(stream, Encoding.UTF8, false, BufferSize, true))
			{
				var line = reader.ReadLine();
				if (line.Equals(_config.HeaderSeparator, StringComparison.Ordinal))
				{
					var header = new StringBuilder();
					while (!(line = reader.ReadLine()).Equals(_config.HeaderSeparator, StringComparison.Ordinal))
					{
						header.AppendLine(line);
					}

					return new File { Path = path, Header = header.ToString(), Content = reader.ReadToEnd() };
				}
				else
				{
					return new File { Path = path };
				}
			}
		}

		public FileWithMetadata AddMetadata(File file)
		{
			var isPost = System.IO.Path.GetDirectoryName(file.Path).Equals(_config.PostsPath, StringComparison.InvariantCultureIgnoreCase);
			var localPath = isPost
				? file.Path.Substring(_config.SitePath.Length + 1 + _config.PostsFolder.Length + 1)
				: file.Path.Substring(_config.SitePath.Length + 1);

			if (file.Content == null)
			{
				file.Content = System.IO.File.ReadAllText(file.Path);
			}

			return new FileWithMetadata
			{
				Path = file.Path,
				Header = file.Header,
				Content = file.Content,
				LocalPath = localPath,
				IsPost = isPost
			};
		}
	}
}
