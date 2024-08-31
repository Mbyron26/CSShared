using CSShared.ToolButton;

namespace CSShared.Common;

public partial class SingletonConfig<T> where T : SingletonConfig<T>, new() {
    public ToolButtonPresent ToolButtonPresent { get; set; } = ToolButtonPresent.UUI;
    public float ToolButtonPositionX { get; set; }
    public float ToolButtonPositionY { get; set; }
}