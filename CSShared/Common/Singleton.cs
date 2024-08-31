using CSShared.Debug;

namespace CSShared.Common;

public abstract class SingletonClass<T> where T : class, new() {
    private static T instance;

    public static T Instance {
        get {
            if (instance is null) {
                instance = new T();
                LogManager.GetLogger().Info($"Creating singleton of type {typeof(T).Name}");
            }
            return instance;
        }
    }
    public static bool Exists => instance is not null;

    public static void Destroy() {
        instance = null;
        LogManager.GetLogger().Info($"Destroyed singleton of type {typeof(T).Name}");
    }
}

public abstract class SingletonMod<T> : SingletonItem<T> { }

public abstract class SingletonItem<T> {
    public static T Instance { get; set; }
}