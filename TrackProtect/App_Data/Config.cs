using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace TrackProtect
{
	public class Config
	{
		Dictionary<string, string> _configuration = new Dictionary<string, string>();
		
		public string this[string key]
		{
			get 
			{
				if (!_configuration.ContainsKey(key.ToLower()))
				    return null;
				    
				return _configuration[key.ToLower()]; 
			}
		}
		
		public Config()
		{
		}
		
		public void Load(string configurationFile)
		{
			using (TextReader reader = new StreamReader(configurationFile))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					line = line.Trim();
					if (string.IsNullOrEmpty(line) || 
					    line.StartsWith("//") || 
					    line.StartsWith(";") || 
					    line.StartsWith("#"))
						continue;
					
					string[] parts = line.Split(new char[] {'='}, 2);
					string key = string.Empty, val = string.Empty;
					if (parts.Length > 0)
						key = parts[0].Trim().ToLower();
					if (parts.Length > 1)
						val = parts[1].Trim();
					if (!string.IsNullOrEmpty(key))
						_configuration[key] = val;
				}
			}
		}
	}
}

