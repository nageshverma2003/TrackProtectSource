using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace TrackProtect.Logging
{
    #region LogLevel enum

    /// <summary>
    /// the possible log-levels
    /// </summary>
    [Flags]
    public enum LogLevel : uint
    {
        // Individual levels
        /// <summary>
        /// indicates a Fatal error, the application will terminate
        /// </summary>
        Fatal = 0x00000001,
        /// <summary>
        /// indicates an error condition, the application will continue
        /// </summary>
        Error = 0x00000002,
        /// <summary>
        /// indicates a warning condition, no malfunction is expected
        /// </summary>
        Warning = 0x00000004,
        /// <summary>
        /// indicates a condition that requires attention but is not erroneous
        /// </summary>
        Attention = 0x00000008,
        /// <summary>
        /// indicates a condition that desires attention but is not erroneous
        /// </summary>
        Info = 0x00000010,
        /// <summary>
        /// indicates a hint for improved operation
        /// </summary>
        Hint = 0x00000020,
        /// <summary>
        /// indicates tracing information
        /// </summary>
        Trace = 0x00000040,
        /// <summary>
        /// indicates debug information
        /// </summary>
        Debug = 0x00000080,
        /// <summary>
        /// indicates begin of logging
        /// </summary>
        Begin = 0x00000100,
        /// <summary>
        /// indicates end of logging
        /// </summary>
        End = 0x00000200,
        /// <summary>
        /// indicates a software checkpoint
        /// </summary>
        Checkpoint = 0x00000400,
        /// <summary>
        /// indicates a user defined log-entry
        /// </summary>
        User = 0x00000800,
        // Masks to enable sets of levels
        /// <summary>
        /// mask that allows all error log-levels
        /// </summary>
        All_Errors = 0x0000000F,
        /// <summary>
        /// mask that allows all information log-levels
        /// </summary>
        All_Info = 0x000000F0,
        /// <summary>
        /// mask that allows all system log-levels (error and information)
        /// </summary>
        System = 0x000007FF,
        /// <summary>
        /// mask that allows all log-levels
        /// </summary>
        All = 0xFFFFFFFF
    }

    #endregion

    #region LogEventArgs

    /// <summary>
    /// Implements the arguments for the OnAddLogEntry event.
    /// The argument is a string that will be logged through the subscribers
    /// of the OnAddLogEntry event.
    /// </summary>
    public class LogEventArgs : EventArgs
    {
        private readonly string _text;

        /// <summary>
        /// the text stored in this class 
        /// </summary>
        public string Text
        {
            get { return _text; }
        }

        /// <summary>
        /// constructor for this class
        /// </summary>
        /// <param name="text">the text to be logged</param>
        public LogEventArgs(string text)
        {
            _text = text;
        }
    }

    #endregion

    /// <summary>
    /// delegate for functions to handle the OnAddLogEntry event
    /// </summary>
    public delegate void AddLogEntry(object sender, LogEventArgs e);

    /// <summary>
    /// Log implements the AddLogEntry delegate and the OnAddLogEntry event.
    /// The Log class is implemented as a singleton. Whenever a class has to log
    /// information the Write function can be called specifying a log-level and
    /// the text to record. The subscribers to the Log class will then be
    /// notified of the specified log-entry to deal with it as they please.
    /// This class is implemented as a singleton.
    /// </summary>
    public class Log
    {
        /// <summary>
        /// event that is triggered when a new log-entry is to be written
        /// </summary>
        public event AddLogEntry OnAddLogEntry;
        /// <summary>
        /// reference to the instance of the log
        /// </summary>
        private static Log _instance = null;
        /// <summary>
        /// mask of acceptable log-levels
        /// </summary>
        private LogLevel _logLevel = 0;

        /// <summary>
        /// get the instance of the log object to act on 
        /// </summary>
        /// <value>a reference to the existing instance or null</value>
        public static Log Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Log();

                return _instance;
            }
        }

        /// <summary>
        /// get or set the current level for the log object
        /// </summary>
        /// <value>the log-level</value>
        public LogLevel LogLevel
        {
            get { return _logLevel; }
            set { _logLevel = value; }
        }
		
		public bool IsAttached
		{
			get { return (OnAddLogEntry != null); }
		}
		
        /// <summary>
        /// constructor for this class
        /// </summary>
        private Log()
        {
            _logLevel = LogLevel.All;
            Start();
        }

        /// <summary>
        /// destructor for this class
        /// </summary>
        ~Log()
        {
            Stop();
        }

        /// <summary>
        /// write the arguments in textual form if the specified log-level matches the set log-level
        /// </summary>
        /// <param name="level">the level to match for the data to be logged</param>
        /// <param name="format">the format of the text to log</param>
        /// <param name="args">the arguments to process before logging</param>
        public void Write(LogLevel level, string format, params object[] args)
        {
            // Begin and and are always logged
            if (level != LogLevel.Begin && level != LogLevel.End)
            {
                if ((level & _logLevel) == 0)
                    return;
            }

            char spec = GetLevelSpecifier(level);

            string fmt = spec + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " -- " + format;
            while (fmt.IndexOf("\r\r\n") > -1)
                fmt = fmt.Replace("\r\r\n", "\n");
            while (fmt.IndexOf("\r\n") > -1)
                fmt = fmt.Replace("\r\n", "\n");
            while (fmt.IndexOf("\r") > -1)
                fmt = fmt.Replace("\r", "\n");
            string text = string.Format(fmt, args);
            text = text.Replace("\n", "\r\n*                            ");
            if (OnAddLogEntry != null)
                OnAddLogEntry(this, new LogEventArgs(text));
        }

        /// <summary>
        /// write the arguments in textual form if the specified log-level
        /// matches the set log-level complete with a stack-trace of the calling
        /// process or thread.
        /// </summary>
        /// <param name="level">the level to match for the data to be logged</param>
        /// <param name="stackTrace">the stack-trace of the process or thread issuing this log event</param>
        /// <param name="format">the format of the text to log</param>
        /// <param name="args">the arguments to process before logging</param>
        public void Write(LogLevel level, StackTrace stackTrace, string format, params object[] args)
        {
            // Begin and and are always logged
            if (level != LogLevel.Begin && level != LogLevel.End)
            {
                if ((level & _logLevel) == 0)
                    return;
            }

            char spec = GetLevelSpecifier(level);

            string fmt = spec + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " -- " + format;
            while (fmt.IndexOf("\r\r\n") > -1)
                fmt = fmt.Replace("\r\r\n", "\n");
            while (fmt.IndexOf("\r\n") > -1)
                fmt = fmt.Replace("\r\n", "\n");
            while (fmt.IndexOf("\r") > -1)
                fmt = fmt.Replace("\r", "\n");
            string text = string.Format(fmt, args);

            string trace = string.Empty;
            if (stackTrace != null)
            {
                trace = stackTrace.ToString();
                trace = trace.Replace("\r\n", "\n");
                trace = trace.Replace("\r", "\n");
            }

            if (trace != string.Empty)
            {
                text += "\nStack-trace:\n============\n";
                text += trace;
            }

            text = text.Replace("\n", "\r\n*                            ");
            try
            {
                if (OnAddLogEntry != null)
                    OnAddLogEntry(this, new LogEventArgs(text));
            }
            catch (Exception)
            {
                // Occasional exception on disposed object
                // Ignore it
            }
        }

        /// <summary>
        /// Write the arguments in textual form if the specified log-level
        /// matches the set log-level complete with all data relevant for an
        /// exception, including nested (inner) exceptions.
        /// </summary>
        /// <param name="level">the level to match for the data to be logged</param>
        /// <param name="exception">the exception to log</param>
        public void Write(LogLevel level, Exception exception)
        {
            string format = " Exception occurred.\nMessage: {0}\nSource: {1}\nTarget Site: {2}\n{3}";
            Write(level, format, exception.Message, exception.Source, exception.TargetSite, exception.StackTrace);
            if (exception.InnerException != null)
                Write(level, exception.InnerException);
        }

        /// <summary>
        /// writes the arguments in textual form if the specified log-level
        /// matches the set log-level complete with all data relevent for an
        /// exception, including nested (inner) exceptions. The exception text
        /// will be preceded by the specified formatted string.
        /// </summary>
        /// <param name="level">the level to match for the data to be logged</param>
        /// <param name="exception">the exception to log</param>
        /// <param name="format">the formatted prefix string for the logged text</param>
        /// <param name="args">the parameters to use to complete the formatted prefix string</param>
        public void Write(LogLevel level, Exception exception, string format, params object[] args)
        {
            if (level != LogLevel.Begin && level != LogLevel.End)
            {
                if ((level & _logLevel) == 0)
                    return;
            }

            char spec = GetLevelSpecifier(level);

            string fmt = spec + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " -- " + format;
            while (fmt.IndexOf("\r\r\n") > -1)
                fmt = fmt.Replace("\r\r\n", "\n");
            while (fmt.IndexOf("\r\n") > -1)
                fmt = fmt.Replace("\r\n", "\n");
            while (fmt.IndexOf("\r") > -1)
                fmt = fmt.Replace("\r", "\n");
            string text = string.Format(fmt, args) + "\nException occurred.\nMessage: {0}\nSource: {1}\nTarget Site: {2}\n{3}";
            text = string.Format(text, exception.Message, exception.Source, exception.TargetSite, exception.StackTrace);
            text = text.Replace("\n", "\r\n*                            ");
            if (OnAddLogEntry != null)
                OnAddLogEntry(this, new LogEventArgs(text));
            if (exception.InnerException != null)
                Write(_logLevel, exception.InnerException, format, args);
        }

        /// <summary>
        /// log a starting line in the log-file
        /// </summary>
        public void Start()
        {
			FileVersionInfo fvi = null;
            Assembly asm = Assembly.GetEntryAssembly();
			if (asm != null)
            	fvi = FileVersionInfo.GetVersionInfo(asm.Location);

            Write(LogLevel.Begin, "LOGGING STARTED ---------------------------------------------------------------");
			if (fvi != null)
            	Write(LogLevel.Begin, "Application: {0}, Build: {1}", fvi.OriginalFilename, fvi.FileVersion);
        }

        /// <summary>
        /// log a stop line in the log-file
        /// </summary>
        public void Stop()
        {
            Write(LogLevel.End, "LOGGING ENDED -----------------------------------------------------------------");
        }

        /// <summary>
        /// convert the log-level value to a character representation to be used in the log-file
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private char GetLevelSpecifier(LogLevel level)
        {
            char retVal = '?';

            switch (level)
            {
                case LogLevel.Fatal: retVal = 'F'; break;
                case LogLevel.Error: retVal = 'E'; break;
                case LogLevel.Warning: retVal = 'W'; break;
                case LogLevel.Attention: retVal = 'A'; break;
                case LogLevel.Info: retVal = 'I'; break;
                case LogLevel.Hint: retVal = 'H'; break;
                case LogLevel.Trace: retVal = 'T'; break;
                case LogLevel.Debug: retVal = 'D'; break;
                case LogLevel.Begin: retVal = '<'; break;
                case LogLevel.End: retVal = '>'; break;
                case LogLevel.Checkpoint: retVal = '!'; break;
                case LogLevel.User: retVal = '='; break;
            }

            return retVal;
        }
    }
}
