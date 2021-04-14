using NJekyll.Model;

namespace NJekyll.Utilities
{
	public interface IFileFactory
	{
		FileWithMetadata AddMetadata(File file);
		File Create(string path);
	}
}