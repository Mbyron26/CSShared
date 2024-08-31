using System;
using System.Diagnostics;

namespace CSShared.Tools;

public class PerformanceCounter : IDisposable {
    private readonly Stopwatch stopwatch = new();
    private readonly Action<TimeSpan> resultCallback;
    private readonly Action<PerformanceCounter> counterCallback;

    public TimeSpan Result => stopwatch.Elapsed;
    public string ReportSeconds => string.Format("{0:F3}s", Result.TotalSeconds);
    public string ReportMilliseconds => string.Format("{0:F3}ms", Result.TotalMilliseconds);


    public PerformanceCounter() => stopwatch.Start();
    public PerformanceCounter(Action<TimeSpan> callback) : this() => resultCallback = callback;
    public PerformanceCounter(Action<PerformanceCounter> callback) : this() => counterCallback = callback;


    public static PerformanceCounter Start(Action<TimeSpan> callback) => new(callback);
    public static PerformanceCounter Start(Action<PerformanceCounter> callback) => new(callback);

    public void Report(Action<TimeSpan> callback) => callback(Result);


    public void Dispose() {
        stopwatch.Stop();
        resultCallback?.Invoke(Result);
        counterCallback?.Invoke(this);
    }
}