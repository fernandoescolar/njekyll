using System;
using System.IO;
using System.Linq;

namespace NJekyll
{
	public class FileWatcher : IDisposable
	{
		private Action<string> _action;
		private string[] _ignore;
		private FileSystemWatcher _watcher;

		public FileWatcher(string input, Action<string> action, string[] ignore)
		{
			_action = action;
			_ignore = ignore;

			_watcher = new FileSystemWatcher(input);
			_watcher.IncludeSubdirectories = true;
			_watcher.EnableRaisingEvents = true;
			_watcher.Changed += OnChanged;
			_watcher.Created += OnCreated;
			_watcher.Deleted += OnDeleted;
			_watcher.Renamed += OnRenamed;
		}

		public void Dispose()
		{
			_watcher.Dispose();
		}

		private void OnChanged(object sender, FileSystemEventArgs e)
		{
			OnSomethingHappens(e.FullPath);
		}

		private void OnCreated(object sender, FileSystemEventArgs e)
		{
			OnSomethingHappens(e.FullPath);
		}

		private void OnDeleted(object sender, FileSystemEventArgs e)
		{
			OnSomethingHappens(e.FullPath);
		}

		private void OnRenamed(object sender, RenamedEventArgs e)
		{
			OnSomethingHappens(e.FullPath);
		}

		private void OnSomethingHappens(string filepath)
		{
			if (_ignore.Any(x => filepath.StartsWith(x, StringComparison.InvariantCultureIgnoreCase)))
			{
				return;
			}

			_action?.Invoke(filepath);
		}
	}
}
