namespace CSShared.Common;

public struct ChangelogContent {
    public readonly ChangelogFlag Flag = ChangelogFlag.None;
    public readonly string Content = string.Empty;

    public ChangelogContent(ChangelogFlag logFlag, string content) {
        Flag = logFlag;
        Content = content;
    }
}