using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TrackProtect
{
	public class CsvReader
	{
		public string _filename = string.Empty;
		public CsvReader ()
		{
		}
		
		public CsvLines Load(string filename)
		{
			CsvLines res = new CsvLines();
			_filename = filename;
			using (TextReader reader = new StreamReader(filename, true))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					CsvLine data = new CsvLine();
					data.Fields = ProcessLine(line);
					res.Add(data);
				}
			}
			return res;
		}
		
		private string[] ProcessLine(string line)
		{
			List<string> res = new List<string>();
			StringBuilder sb = new StringBuilder();
			char lastQuote = '"';
			bool inQuote = false;
			bool escaped = false;
			foreach (char ch in line)
			{
				switch (ch)
				{
				case '\\':
					if (escaped)
					{
						sb.Append(ch);
						escaped = false;
					}
					else
					{
						escaped = true;
					}
					break;
					
				case '"':
				case '\'':
					sb.Append(ch);
					if (!inQuote)
					{
						inQuote = true;
						lastQuote = ch;
					}
					else
					{
						if (lastQuote == ch)
						{
							if (!escaped)
								inQuote = false;
						}
					}
					escaped = false;
					break;
					
				case ',':
					if (!inQuote)
					{
						string tmp = sb.ToString();
						tmp = tmp.Trim();
						tmp = tmp.Trim ('"', '\'');
						res.Add(tmp);
						sb = new StringBuilder();
					}

					escaped = false;
					break;
					
				default:
					sb.Append(ch);
					escaped = false;
					break;
				}
			}
			if (sb.Length > 0)
			{
				string tmp = sb.ToString();
				tmp = tmp.Trim();
				tmp = tmp.Trim('"', '\'');
				res.Add(tmp);
			}
			return res.ToArray();
		}
	}
	
	public class CsvLine
	{
		public string[] Fields { get; set; }
	}
	
	public class CsvLines : List<CsvLine>
	{
	}
}

