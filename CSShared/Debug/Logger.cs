using CSShared.Tools;
using System;
using System.IO;
using System.Threading;
using UnityEngine;

namespace CSShared.Debug;

public partial class Logger : ILog {
    private static readonly object fileLock = new();
    public static string DebugFilePath { get; } = Path.Combine(Application.dataPath, AssemblyTools.CurrentAssemblyName + ".log");
    public bool InternalLogging { get; set; }
    public bool ExternalLoggingFileCreated { get; private set; }
    public string Name { get; set; }

    public Logger(string name, bool internalLogging = false) {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));
        Name = name;
        InternalLogging = internalLogging;
    }

    public void Debug(object message) => Log(LogType.Debug, message);
    public void Debug(Exception exception, object message) => Log(LogType.Debug, $"\n{message}");
    public void DebugFormat(string format, object p) => Log(LogType.Debug, string.Format(format, p));
    public void DebugFormat(string format, object p1, object p2) => Log(LogType.Debug, string.Format(format, p1, p2));
    public void DebugFormat(string format, params object[] p) => Log(LogType.Debug, string.Format(format, p));
    public void Info(object message) => Log(LogType.Info, message);
    public void Info(Exception exception, object message) => Log(LogType.Info, $"\n{message}");
    public void InfoFormat(string format, object p) => Log(LogType.Info, string.Format(format, p));
    public void InfoFormat(string format, object p1, object p2) => Log(LogType.Info, string.Format(format, p1, p2));
    public void InfoFormat(string format, params object[] p) => Log(LogType.Info, string.Format(format, p));
    public void Error(object message) => Log(LogType.Error, message);
    public void Error(Exception exception, object message) => Log(LogType.Error, $"\n{message}");
    public void ErrorFormat(string format, object p) => Log(LogType.Error, string.Format(format, p));
    public void ErrorFormat(string format, object p1, object p2) => Log(LogType.Error, string.Format(format, p1, p2));
    public void ErrorFormat(string format, params object[] p) => Log(LogType.Error, string.Format(format, p));
    public void Warn(object message) => Log(LogType.Warn, message);
    public void Warn(Exception exception, object message) => Log(LogType.Warn, $"\n{message}");
    public void WarnFormat(string format, object p) => Log(LogType.Warn, string.Format(format, p));
    public void WarnFormat(string format, object p1, object p2) => Log(LogType.Warn, string.Format(format, p1, p2));
    public void WarnFormat(string format, params object[] p) => Log(LogType.Warn, string.Format(format, p));
    public void Patch(object message) => Log(LogType.Patch, message);
    public void Patch(Exception exception, object message) => Log(LogType.Patch, $"\n{message}");
    private string GetPrefixLog(LogType logType) => '[' + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ']' + $" [{logType.ToString().ToUpper()}] ";

    private void Log(LogType logType, object message) {
        if (InternalLogging) {
            UnityEngine.Debug.logger.Log($"[{AssemblyTools.CurrentAssemblyName}] {GetPrefixLog(logType)} {message}");
        }
        else {
            EnsureExternalFile();
            Monitor.Enter(fileLock);
            try {
                using FileStream debugFile = new(DebugFilePath, FileMode.Append);
                using StreamWriter sw = new(debugFile);
                sw.WriteLine($"{GetPrefixLog(logType)} {message}");
            }
            finally {
                Monitor.Exit(fileLock);
            }
        }
    }
    private void EnsureExternalFile() {
        if (!ExternalLoggingFileCreated) {
            using FileStream debugFile = new(DebugFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            using StreamWriter sw = new(debugFile);
            sw.WriteLine($"{GetPrefixLog(LogType.Info)} Version: {AssemblyTools.CurrentAssemblyVersion}");
            ExternalLoggingFileCreated = true;
        }
    }
}