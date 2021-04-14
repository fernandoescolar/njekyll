using System;
using System.Collections.Generic;

namespace NJekyll.Model
{
	public class FileWithMetadata : File
	{
		public Dictionary<string, object> Variables { get; set; }
		public string LocalPath { get; set; }
		public bool IsPost { get; set; }
		public string Layout { get; set; }
		public int? Paginate { get; set; }
		public DateTime Date { get; set; }
		public string Category { get; set; }
		public IEnumerable<string> Tags { get; set; }
	}
}
