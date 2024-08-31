using CSShared.Debug;
using System;
using System.Collections.Generic;

namespace CSShared.Manager;

public static class ManagerPool {
    private static readonly Dictionary<Type, IManager> buffer = new();

    public static event Action<Type, IManager> OnManagerCreated;
    public static event Action<Type, IManager> OnManagerReset;
    public static event Action<Type, IManager> OnManagerReleased;

    public static bool HasManager<T>() => buffer.ContainsKey(typeof(T));
    public static IEnumerable<Type> GetAllManagerTypes() => buffer.Keys;
    public static IEnumerable<IManager> GetAllManagers() => buffer.Values;
    public static int GetManagerCount() => buffer.Count;

    public static T GetOrCreateManager<T>() where T : IManager, new() {
        if (buffer.TryGetValue(typeof(T), out IManager manager))
            return (T)manager;
        var newManager = new T();
        newManager.OnCreated();
        OnManagerCreated?.Invoke(typeof(T), newManager);
        buffer[typeof(T)] = newManager;
        LogManager.GetLogger().Info($"ManagerPool create manager: {typeof(T)}");
        return newManager;
    }

    public static T GetOrCreateManager<T, TParam>(TParam param) where T : IManager<TParam>, new() {
        if (buffer.TryGetValue(typeof(T), out IManager manager))
            return (T)manager;
        var newManager = new T();
        newManager.OnCreated(param);
        OnManagerCreated?.Invoke(typeof(T), newManager);
        buffer[typeof(T)] = newManager;
        LogManager.GetLogger().Info($"ManagerPool create manager: {typeof(T)}");
        return newManager;
    }

    public static T GetOrCreateManager<T, TParam1, TParam2>(TParam1 param1, TParam2 param2) where T : IManager<TParam1, TParam2>, new() {
        if (buffer.TryGetValue(typeof(T), out IManager manager))
            return (T)manager;
        var newManager = new T();
        newManager.OnCreated(param1, param2);
        OnManagerCreated?.Invoke(typeof(T), newManager);
        buffer[typeof(T)] = newManager;
        LogManager.GetLogger().Info($"ManagerPool create manager: {typeof(T)}");
        return newManager;
    }

    public static T GetOrCreateManager<T, TParam1, TParam2, TParam3>(TParam1 param1, TParam2 param2, TParam3 param3) where T : IManager<TParam1, TParam2, TParam3>, new() {
        if (buffer.TryGetValue(typeof(T), out IManager manager))
            return (T)manager;
        var newManager = new T();
        newManager.OnCreated(param1, param2, param3);
        OnManagerCreated?.Invoke(typeof(T), newManager);
        buffer[typeof(T)] = newManager;
        LogManager.GetLogger().Info($"ManagerPool create manager: {typeof(T)}");
        return newManager;
    }

    public static T GetOrCreateManager<T, TParam1, TParam2, TParam3, TParam4>(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4) where T : IManager<TParam1, TParam2, TParam3, TParam4>, new() {
        if (buffer.TryGetValue(typeof(T), out IManager manager))
            return (T)manager;
        var newManager = new T();
        newManager.OnCreated(param1, param2, param3, param4);
        OnManagerCreated?.Invoke(typeof(T), newManager);
        buffer[typeof(T)] = newManager;
        LogManager.GetLogger().Info($"ManagerPool create manager: {typeof(T)}");
        return newManager;
    }

    public static bool ResetManager<T>() where T : IManager {
        if (buffer.TryGetValue(typeof(T), out IManager manager)) {
            manager.Reset();
            OnManagerReset?.Invoke(typeof(T), manager);
            return true;
        }
        return false;
    }

    public static void ResetAllManagers() {
        foreach (var manager in buffer.Values) {
            manager.Reset();
        }
    }

    public static void DestroyManager<T>() where T : IManager {
        if (buffer.TryGetValue(typeof(T), out IManager manager)) {
            manager.OnReleased();
            OnManagerReleased?.Invoke(typeof(T), manager);
            buffer.Remove(typeof(T));
        }
    }

    public static void DestroyAllManager() {
        foreach (var manager in buffer.Values) {
            manager.OnReleased();
        }
        buffer.Clear();
    }

    public static T GetManagerOrNull<T>() where T : IManager {
        if (buffer.TryGetValue(typeof(T), out var manager))
            return (T)manager;
        return default;
    }

    public static void UpdateManager<T>() where T : IManager {
        if (buffer.TryGetValue(typeof(T), out var manager))
            manager.Update();
    }

    public static void UpdateManagers() {
        foreach (var manager in buffer.Values) {
            manager.Update();
        }
    }

    public static void UpdateManager<T>(Action<T> updateAction) where T : IManager {
        if (buffer.TryGetValue(typeof(T), out IManager manager)) {
            updateAction?.Invoke((T)manager);
        }
    }

    public static IEnumerable<T> FilterManagers<T>(Func<T, bool> predicate) where T : IManager {
        foreach (var manager in buffer.Values) {
            if (manager is T typedManager && predicate(typedManager)) {
                yield return typedManager;
            }
        }
    }

}