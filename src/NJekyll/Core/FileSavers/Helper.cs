namespace NJekyll.Core.FileSavers
{
	public static class Helper
	{
		public static void EnsureDirectoryExists(string file)
		{
			var dir = System.IO.Path.GetDirectoryName(file);
			System.IO.Directory.CreateDirectory(dir);
		}
	}
}
