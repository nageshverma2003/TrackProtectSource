using System;
using System.Diagnostics;
using System.Web;
using System.Web.UI;
using TrackProtect.Logging;

namespace TrackProtect
{
	public class Logger
	{
		Log _log = Log.Instance;
		LogWriter _logw = null;
		
		static Logger _logger = null;
		public static Logger Instance
		{
			get
			{
				if (_logger == null)
					_logger = new Logger();
				
				return _logger;
			}
		}
		
		private Logger()
		{
			if (!_log.IsAttached || _logw == null)
			{
				Config cfg = new Config();
				cfg.Load(HttpContext.Current.Server.MapPath("~/Config/trackprotect.config"));
				string logfile = cfg["logfile"];
				if (logfile.StartsWith("~"))
					logfile = HttpContext.Current.Server.MapPath(logfile);
		
				//Page page = HttpContext.Current.Handler as Page;
				//page.Response.Write (string.Format ("logfile: {0}<br/>", logfile));

				_logw = new LogWriter(_log, logfile);
				_log.Start();
			}
		}
		
		public void Write(LogLevel level, string format, params object[] args)
		{
			_log.Write (level, format, args);
		}
		
		public void Write(LogLevel level, Exception exception)
		{
			_log.Write (level, exception);
		}
		
		public void Write(LogLevel level, Exception exception, string format, params object[] args)
		{
			_log.Write (level, exception, format, args);
		}
		
		public void Write(LogLevel level, StackTrace stackTrace, string format, params object[] args)
		{
			_log.Write (level, stackTrace, format, args);
		}
	}

	public class Trace
	{
		string _text = string.Empty;
		
		public Trace(string text)
		{
			_text = text;
			Logger.Instance.Write(LogLevel.Trace, "{0} ENTER", _text);
		}
		
		~Trace()
		{
			Logger.Instance.Write(LogLevel.Trace, "{0} LEAVE", _text);
		}
	}
}

