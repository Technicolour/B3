﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace IndiaTango.Models
{
    public static class EventLogger
    {
        private const string Info = "INFO";
        private const string Warning = "WARNING";
        private const string Error = "ERROR";
        private static StreamWriter _writer;
        private static readonly object mutex = new object();

        public static string LogFilePath
        {
            get { return Path.Combine(Common.AppDataPath,"Logs","log.txt") ; }
        }

        private static void WriteLogToFile(string log)
        {
            if(!Directory.Exists(LogFilePath))
                Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath));

            lock (mutex)
            {
                _writer = File.AppendText(LogFilePath);
                _writer.WriteLine(log);
                _writer.Close();
            }
        }

        private static string LogBase(string logType, string threadName, string eventDetails)
        {
            if (String.IsNullOrWhiteSpace(threadName))
                throw new ArgumentNullException("Thread name cannot be null or empty");

            if (String.IsNullOrWhiteSpace(eventDetails))
                throw new ArgumentNullException("Parameter 'eventDetails' cannot be null or empty");
            
            string logString = DateTime.Now.ToString("dd/MM/yyyy HH:MM:ss") + " " + logType.PadRight(8).Substring(0, 8) + " " + threadName.PadRight(20).Substring(0, 20) + " " + eventDetails;
            
            WriteLogToFile(logString);

            return logString;
        }

        public static string LogInfo(string threadName, string eventDetails)
        {
            return LogBase(Info, threadName, eventDetails);
        }

        public static string LogWarning(string threadName, string eventDetails)
        {
            return LogBase(Warning, threadName, eventDetails);
        }

        public static string LogError(string threadName, string eventDetails)
        {
            return LogBase(Error, threadName, eventDetails);
        }
    }
}
