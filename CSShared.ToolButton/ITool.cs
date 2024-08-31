using CSShared.Manager;

namespace CSShared.ToolButton;

public interface ITool : IManager {
    bool UUISupport { get; }
    void Enable();
    void Disable();
}