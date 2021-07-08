using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using System;

namespace Bitstamp.DataPuller
{
    public class Log
    {
        static Logger _instance;
        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = CreateLogger();
                return _instance;
            }
        }
        static Logger CreateLogger()
        {
            LogManager.ThrowExceptions = true;
            LoggingConfiguration config = new LoggingConfiguration();
            FileTarget appData = new FileTarget("app_data")
            {
                Layout = new SimpleLayout(@"${longdate} - ${level:uppercase=true}(${callsite:className=true:includeSourcePath=false:methodName=true}): ${message}${onexception:${newline}EXCEPTION\: ${exception:format=ToString}}"),
                FileName = new SimpleLayout(@"${specialfolder:folder=LocalApplicationData}\PauliusUrmonas\BitstampDataPuller\Logs\log.txt"),
                ArchiveFileName = new SimpleLayout(@"${specialfolder:folder=LocalApplicationData}\PauliusUrmonas\BitstampDataPuller\Logs\log_${shortdate}.{##}.txt"),
                KeepFileOpen = false,
                ArchiveNumbering = ArchiveNumberingMode.Sequence,
                ArchiveEvery = FileArchivePeriod.Month,
                MaxArchiveFiles = 72
            };
            EventLogTarget eventlog = new EventLogTarget("eventlog")
            {
                Source = new SimpleLayout("Bitstamp.DataPuller"),
                Layout = new SimpleLayout("${message}${newline}${exception:format=ToString}")
            };
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Error, eventlog));
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, appData));
            LogManager.Configuration = config;
            return LogManager.GetCurrentClassLogger();
        }
    }
}
