namespace CSShared.Manager;

public interface IManager {
    void OnCreated();
    void Update();
    void Reset();
    void OnReleased();
}

public interface IManager<T> : IManager {
    void OnCreated(T t);
}

public interface IManager<T1, T2> : IManager {
    void OnCreated(T1 t1, T2 t2);
}

public interface IManager<T1, T2, T3> : IManager {
    void OnCreated(T1 t1, T2 t2, T3 t3);
}

public interface IManager<T1, T2, T3, T4> : IManager {
    void OnCreated(T1 t1, T2 t2, T3 t3, T4 t4);
}