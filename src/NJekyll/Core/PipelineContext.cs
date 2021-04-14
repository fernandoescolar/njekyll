using System.Collections.Generic;
using System.Dynamic;
using NJekyll.Model;

namespace NJekyll.Core
{
	public class PipelineContext
	{
		public Dictionary<string, object> Site { get; set; }

		public Dictionary<string, FileWithMetadata> Layouts { get; set; }

		public Dictionary<string, FileWithMetadata> Includes { get; set; }

		public IEnumerable<File> StaticFiles { get; set; }

		public IEnumerable<FileWithMetadata> NonStaticFiles { get; set; }
	}
}
