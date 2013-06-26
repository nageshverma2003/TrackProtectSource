using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;

namespace TrackProtect.Logging
{
    /// <summary>
    /// LogWriter writes logging information to a file. LogWriter is a
    /// subscriber to the instance of the Log class with the express purpose
    /// of writing log information to the file.
    /// </summary>
    /// <example> The following example shows how to attach a logWriter instance to a log.
    /// <code>
    ///		ProSyntic.Logging.Log log = ProSyntic.Logging.Log.Instance;
    ///		ProSyntic.Logging.LogWriter logWriter = new ProSyntic.Logging.LogWriter(log, "mylog.%D.log");
    ///		
    ///		// Now start the log and write the first entry
    ///		log.Start();
    ///		log.Write(LogLevel.Trace, "Example log entry");
    /// </code>
    /// </example>
    /// <example> The following example shows how to implement an event handler for the OnAddLogEntry event
    /// <code>		
    /// private void WriteEntry(object sender, LogEventArgs e)
    /// {
    ///		string filename = _filename;
    /// 
    /// 	try
    ///		{
    /// 		StreamWriter stream = new StreamWriter(filename, true);
    /// 		if ( stream != null )
    ///			{
    /// 			stream.WriteLine(e.Text);
    /// 			stream.Close();
    ///			}
    ///		}
    /// 	catch ( Exception ex )
    ///		{
    /// 		string report = string.Format("[LogWriter] Exception. Source: {0}. Message: {1}. Site: {2}. Stacktrace: {3}", ex.Source, ex.Message, ex.TargetSite, ex.StackTrace);
    /// 
    /// 		EventLog eventLog = new EventLog("Application");
    /// 		if ( eventLog != null )
    ///			{
    /// 			eventLog.Source = _eventLogName;
    /// 			eventLog.WriteEntry(report, EventLogEntryType.Error);
    ///			}
    /// 
    ///		}
    /// }
    /// </code>
    /// </example>
    public class LogWriter
    {
        // The event-log name will be used when logging to a file is not
        // possible, in that case this occurrance and the event are logged
        // to the Windows event log under the name "LOGGER";
        const string _eventLogName = "Logger";

        Log _log;
        string _filename;


        /// <summary>
        /// write the specified log event to a log-file
        /// </summary>
        /// <param name="sender">the source of the log-event</param>
        /// <param name="e">the log-event to log</param>
        private void WriteEntry(object sender, LogEventArgs e)
        {
            DateTime now = DateTime.Now;
			//Page page = HttpContext.Current.Handler as Page;

            try
            {
                bool success = false;
                for (int retries = 0; retries < 5; retries++)
                {
                    string filename = _filename;
                    filename = filename.Replace("%D", now.ToString("yyyyMMdd"));
                    filename = filename.Replace("%T", now.ToString("HHmmss"));
                    filename = filename.Replace("%M", Environment.MachineName);
                    filename = filename.Replace("%N", Environment.UserName);

					//page.Response.Write (string.Format("filename: {0}<br/>", filename));
                    try
                    {
                        lock (this)
                        {
							//page.Response.Write(string.Format ("{0}<br/>",e.Text));
                            using (StreamWriter stream = new StreamWriter(filename, true, Encoding.Unicode))
                            {
                                if (stream != null)
                                {
                                    stream.WriteLine(e.Text);
                                    stream.Close();
                                }
                            }
							using (Database db = new MySqlDatabase())
							{
								db.AddLogEntry(e.Text);
							}
                        }
                        success = true;
                        break;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // For some reason the access to the log-file has been denied.
                        // Create a new Name and try it again.
                        string fnm = Path.GetFileNameWithoutExtension(_filename);
                        string ext = Path.GetExtension(_filename);
                        string idx = Path.GetExtension(fnm);
                        if (idx.StartsWith(".(") && idx.EndsWith(")"))
                        {
                            fnm = Path.GetFileNameWithoutExtension(fnm);
                            idx = idx.Substring(2);
                            idx = idx.Substring(0, idx.Length - 1);
                            // We should now have a numeric value, check that
                            int cnt = 0;
                            bool isNumber = true;	// Assume it is a number
                            // Check leading sign
                            if (idx[0] != '-'
                                && idx[0] != '+'
                                && !char.IsDigit(idx[0])
                                )
                                isNumber = false;
                            for (int i = 1; i < idx.Length && isNumber; i++)
                            {
                                if (!char.IsDigit(idx[i]))
                                    isNumber = false;
                            }
                            if (isNumber)
                                cnt = Convert.ToInt32(idx);
                            ++cnt;
                            idx = ".(" + cnt.ToString() + ")";
                        }
                        else
                        {
                            // No log-file recovery index exists yet, create one
                            // now. Start with 1, the original (unnumbered) file
                            // is then considered 0. Under normal circumstances
                            // renumbering should not be necessary.
                            idx = ".(1)";
                        }
                        fnm = Path.GetFileNameWithoutExtension(fnm) + idx;
                        _filename = fnm + ext;
                    }
                }
                if (!success)
                    throw new Exception("Access denied to log-files. File renumbering failed continuously.");
            }
            catch (Exception /*ex*/)
            {
                //ReportToEventLog(ex, 1, e.Text);
				//throw;
            }
        }

        /// <summary>
        /// constructor for this class
        /// </summary>
        /// <param name="log">the log-object to receive log information from</param>
        /// <param name="filename">the filename of the file to log to</param>
        /// <remarks>
        /// <para>
        /// The filename may contain on or more of the following sequences to add
        /// additional information to the filename:
        /// </para>
        /// <list type='bullet'>
        /// <item>%D - this sequence is replaced by the date the log-file is written to.</item>
        /// <item>%T - this sequence is replaced by the time (HH:mm:ss) the log-file is written to.</item>
        /// <item>%M - this sequence is replaced by the name of the machine causing the log-file to be changed.</item>
        /// <item>%N - this sequence is replaced by the name of the user executing the application that causes the log-file to be changed.</item>
        /// </list>
        /// <para>
        ///     NOTE: Adding a date or time sequence will cause a new log-file to be created whenever
        ///     the date or the time changes.
        /// </para>
        /// </remarks>
        public LogWriter(Log log, string filename)
        {
            _log = log;
            _filename = filename;

            /*
            if (!EventLog.SourceExists(_eventLogName))
                EventLog.CreateEventSource(_eventLogName, "Application");
            */
            _log.OnAddLogEntry += new AddLogEntry(WriteEntry);

            try
            {
                // Ensure the directory path for the log exists
                string dir = Path.GetDirectoryName(filename);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
            catch (Exception)
            {
                //ReportToEventLog(ex, ex.Data);
            }
        }

        private void ReportToEventLog(Exception ex, int errorId, string originalMessage)
        {
            /*
            using (EventLog eventLog = new EventLog("Application"))
            {
                string report = string.Format("[LogWriter] Exception. Source: {0}. Message: {1}. Site: {2}. Stacktrace: {3}", ex.Source, ex.Message, ex.TargetSite, ex.StackTrace);

                // Only append original if it is present
                if (originalMessage != null && originalMessage != string.Empty)
                    report += ("\r\nOriginal message: " + originalMessage);

                if (eventLog != null)
                {
                    eventLog.Source = _eventLogName;
                    eventLog.WriteEntry(report, EventLogEntryType.Error, errorId);
                    eventLog.Close();
                }
            }
             * */
        }
    }
}
