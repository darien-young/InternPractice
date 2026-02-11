using System;
using System.IO;
using System.Collections.Generic;


namespace InternConsoleApp
{

	public class LogEntry
	{
		public string TimeStamp { get; set; } = "";
		public string Action { get; set; } = "";
		public string Name { get; set; } = "";
		public string Age { get; set; } = "";
		public string Category { get; set; } = "";
	}

	// data container for appending to CSV
	public class EventRecord
	{
		public DateTime TimeStamp { get; set; } = DateTime.Now;
		public string Action { get; set; } = "";
		public string Name { get; set; } = "";
		public string Age { get; set; } = "";
		public string Category { get; set; } = "";

		public string ToCsv()
		{
			string Escape(string s) =>
				s.Contains(",") || s.Contains("\"")
				? $"\"{s.Replace("\"", "\"\"")}\""
				: s;

			return $"{TimeStamp:yyyy-MM-dd HH:mm:ss}," +
				   $"{Escape(Action)}," +
				   $"{Escape(Name)}," +
				   $"{Escape(Age)}," +
				   $"{Escape(Category)}";
		}
	}
}



