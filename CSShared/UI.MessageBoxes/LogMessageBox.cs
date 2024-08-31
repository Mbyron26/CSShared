using ColossalFramework.UI;
using CSShared.Common;
using CSShared.Localization;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CSShared.UI.MessageBoxes;

public class LogMessageBox : MessageBoxBase {
    public LogMessageBox() {
        AddButtons(1, 1, Localize("MessageBox_OK"), Close);
    }

    public void Initialize<Mod>(bool maximizeFirst = true) where Mod : IMod {
        TitleText = $"{SingletonMod<Mod>.Instance.ModName} {Localize("ChangeLog")}";
        if (SingletonMod<Mod>.Instance.Changelog.Count == 0) {
            MainPanel.AddUIComponent<CustomUIPanel>().size = new Vector2(MessageBoxParm.ComponentWidth, 30);
            return;
        }
        var first = default(VersionPanel);
        foreach (var list in SingletonMod<Mod>.Instance.Changelog) {
            var versionPanel = MainPanel.AddUIComponent<VersionPanel>();
            versionPanel.VersionChangeLog = list;
            if (first == null) first = versionPanel;
        }
        if (maximizeFirst)
            first.IsMinimize = false;
    }

    public class VersionPanel : CustomUIPanel {
        private const string space = "      ";
        private UILabel versionInfo;
        private CustomUIPanel logContainer;
        private readonly List<LogPanel> logPanels = new();
        private ChangelogInfo versionChangeLog;

        public ChangelogInfo VersionChangeLog {
            get => versionChangeLog;
            set {
                if (!value.Equals(versionChangeLog)) {
                    versionChangeLog = value;
                    OnVersionChangeLogChanged(value);
                }
            }
        }
        public bool IsMinimize {
            get => !logContainer.isVisible;
            set {
                logContainer.isVisible = !value;
                if (versionInfo is not null) {
                    versionInfo.suffix = space + (value ? "▲" : "▼");
                }
            }
        }

        public VersionPanel() {
            width = MessageBoxParm.ComponentWidth;
            autoLayout = true;
            padding = new(10, 10, 10, 10);
            AutoFitChildrenVertically = true;
            itemGap = 4;
            atlas = CustomUIAtlas.CSSharedAtlas;
            bgSprite = CustomUIAtlas.RoundedRectangle3;
            bgNormalColor = CustomUIColor.CPPrimaryBg;
            CreateVersionInfo();
            CreateLogContainer();
        }

        private void OnVersionChangeLogChanged(ChangelogInfo value) {
            versionInfo.text = value.ModVersion.ToString() + space + value.Date.ToString("yyyy/MM/dd");
            FillLogs(value);
        }
        private void FillLogs(ChangelogInfo value) {
            if (logPanels.Any()) {
                foreach (var item in logPanels) {
                    if (item is not null)
                        logContainer.RemoveUIComponent(item);
                }
            }
            for (int i = 0; i < value.Contents.Count; i++) {
                var panel = logContainer.AddUIComponent<LogPanel>();
                if (value.Contents.Count - 1 != i) {
                    panel.RenderFg = true;
                }
                panel.Log = value.Contents[i];
                logPanels.Add(panel);

            }
            IsMinimize = true;
        }
        private void CreateLogContainer() {
            if (logContainer is not null)
                return;
            logContainer = AddUIComponent<CustomUIPanel>();
            logContainer.ItemGap = 4;
            logContainer.AutoLayout = true;
            logContainer.width = MessageBoxParm.ComponentWidth - 20;
            logContainer.AutoFitChildrenVertically = true;
        }
        private void CreateVersionInfo() {
            if (versionInfo is not null) {
                return;
            }
            versionInfo = AddUIComponent<UILabel>();
            versionInfo.autoSize = false;
            versionInfo.size = new(MessageBoxParm.ComponentWidth - 20, 30);
            versionInfo.padding = new(0, 0, 5, 0);
            versionInfo.textScale = 1.125f;
            versionInfo.eventClicked += (c, v) => IsMinimize = !IsMinimize;
        }
    }

    public class LogPanel : CustomUIPanel {
        private CustomUILabel tagLabel;
        private CustomUILabel textLabel;
        private ChangelogContent log;

        public ChangelogContent Log {
            get => log;
            set {
                if (!value.Equals(log)) {
                    log = value;
                    OnLogChanged(value);
                }
            }
        }

        public LogPanel() {
            width = MessageBoxParm.ComponentWidth - 20;
            fgSize = new(width, 20);
            atlas = CustomUIAtlas.CSSharedAtlas;
            fgSprite = CustomUIAtlas.LineBottom;
            padding = new(0, 0, 4, 4);
            fgSpriteMode = ForegroundSpriteMode.Custom;
            verticalAlignment = UIVerticalAlignment.Bottom;
            autoLayoutDirection = LayoutDirection.Horizontal;
            autoFitChildrenVertically = true;
            fgNormalColor = CustomUIColor.CPPrimaryFg;
            itemGap = 6;
            autoLayout = true;
        }

        private void OnLogChanged(ChangelogContent value) {
            if (tagLabel is null) {
                tagLabel = AddUIComponent<CustomUILabel>();
                tagLabel.SetStyle(CustomUIAtlas.CSSharedAtlas, CustomUIAtlas.RoundedRectangle2);
                tagLabel.AutoSize = false;
                tagLabel.size = new Vector2(100, 20);
                tagLabel.TextPadding = new(0, 0, 4, 0);
                tagLabel.TextScale = 0.8f;
                tagLabel.TextHorizontalAlignment = UIHorizontalAlignment.Center;
                tagLabel.TextVerticalAlignment = UIVerticalAlignment.Middle;
                textLabel = AddUIComponent<CustomUILabel>();
                textLabel.width = MessageBoxParm.ComponentWidth - 20 - 100 - 4;
                textLabel.WordWrap = true;
                textLabel.TextScale = 0.8f;
                textLabel.TextPadding = new RectOffset(0, 0, 4, 4);
                textLabel.AutoHeight = true;
                textLabel.TextHorizontalAlignment = UIHorizontalAlignment.Left;
            }
            tagLabel.BgNormalColor = GetColor(value.Flag);
            tagLabel.Text = GetLocalized(value.Flag);
            textLabel.Text = value.Content;
            Invalidate();
        }
        private string GetLocalized(ChangelogFlag value) => value switch {
            ChangelogFlag.Added => Localize("LogMessageBox_Added"),
            ChangelogFlag.Removed => Localize("LogMessageBox_Removed"),
            ChangelogFlag.Updated => Localize("LogMessageBox_Updated"),
            ChangelogFlag.Fixed => Localize("LogMessageBox_Fixed"),
            ChangelogFlag.Optimized => Localize("LogMessageBox_Optimized"),
            ChangelogFlag.Translation => Localize("LogMessageBox_Translation"),
            ChangelogFlag.Attention => Localize("LogMessageBox_Attention"),
            _ => string.Empty
        };

        private string Localize(string value) => ModLocalizationManager.Localize(value);

        private Color32 GetColor(ChangelogFlag value) => value switch {
            ChangelogFlag.Added => new Color32(38, 158, 62, 255),//Green
            ChangelogFlag.Removed => new Color32(146, 50, 128, 255),//Pink
            ChangelogFlag.Updated => new Color32(58, 86, 190, 255),//Blue
            ChangelogFlag.Fixed => new Color32(216, 136, 30, 255),//Orange
            ChangelogFlag.Optimized => new Color32(113, 30, 160, 255),//Purple
            ChangelogFlag.Translation => new Color32(8, 150, 150, 255),//Cyan
            ChangelogFlag.Attention => new Color32(146, 4, 10, 255),//Red
            _ => new Color32(160, 160, 160, 255)//Gray
        };

    }
}



