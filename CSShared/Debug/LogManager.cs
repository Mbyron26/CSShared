using CSShared.Tools;
using System.Collections.Generic;

namespace CSShared.Debug;

public static class LogManager {
    private static Dictionary<string, ILog> Loggers { get; set; } = new();

    public static ILog GetLogger(string name, bool internalLogging = false) {
        if (string.IsNullOrEmpty(name)) {
            name = "Temp";
        }
        if (Loggers.TryGetValue(name, out var log)) {
            return log;
        }
        var logger = new Logger(name, internalLogging);
        Loggers.Add(name, logger);
        return logger;
    }

    public static ILog GetLogger() => GetLogger(AssemblyTools.CurrentAssemblyName);
}