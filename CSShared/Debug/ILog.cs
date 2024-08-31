using System;

namespace CSShared.Debug;

public partial interface ILog {
    string Name { get; set; }
    bool InternalLogging { get; set; }

    void Debug(object message);
    void Debug(Exception exception, object message);
    void DebugFormat(string format, object p1);
    void DebugFormat(string format, object p1, object p2);
    void DebugFormat(string format, params object[] p);

    void Info(object message);
    void Info(Exception exception, object message);
    void InfoFormat(string format, object p);
    void InfoFormat(string format, object p1, object p2);
    void InfoFormat(string format, params object[] p);

    void Warn(object message);
    void Warn(Exception exception, object message);
    void WarnFormat(string format, object p);
    void WarnFormat(string format, object p1, object p2);
    void WarnFormat(string format, params object[] p);

    void Error(object message);
    void Error(Exception exception, object message);
    void ErrorFormat(string format, object p);
    void ErrorFormat(string format, object p1, object p2);
    void ErrorFormat(string format, params object[] p);

    void Patch(object message);
    void Patch(Exception exception, object message);
}