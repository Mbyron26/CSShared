using CSShared.Common;
using CSShared.Debug;
using CSShared.Extension;
using CSShared.Localization;
using CSShared.Manager;
using CSShared.UI.MessageBoxes;
using System;
using UnityEngine;

namespace CSShared.UI.OptionPanel;

public partial class OptionPanelBase<TypeMod, TypeConfig, TypeOptionPanel> : CustomUIPanel where TypeMod : IMod where TypeConfig : SingletonConfig<TypeConfig>, new() where TypeOptionPanel : CustomUIPanel {
    public static readonly Vector2 Size = new(764, 773);
    public static readonly float MainPadding = 16;
    public static readonly float MainWidth = Size.x - 2 * MainPadding;
    public static readonly Vector2 TabSize = new(MainWidth, 30);
    public static readonly Vector2 ContainerSize = new(MainWidth, Size.y - 2 * MainPadding - 30 - 10);
    protected CustomUITabContainer tabContainer;
    protected Action ResetCallback;

    protected CustomUIScrollablePanel GeneralContainer { get; private set; }
    protected CustomUIScrollablePanel HotkeyContainer { get; private set; }
    protected CustomUIScrollablePanel AdvancedContainer { get; private set; }

    public OptionPanelBase() {
        size = Size;
        atlas = CustomUIAtlas.CSSharedAtlas;
        bgSprite = CustomUIAtlas.CustomBackground;
        bgNormalColor = new Color32(130, 130, 130, 255);
        tabContainer = AddUIComponent<CustomUITabContainer>();
        tabContainer.size = TabSize;
        tabContainer.Gap = 3;
        tabContainer.Atlas = CustomUIAtlas.CSSharedAtlas;
        tabContainer.BgSprite = CustomUIAtlas.RoundedRectangle2;
        tabContainer.BgNormalColor = CustomUIColor.OPPrimaryBg;
        tabContainer.relativePosition = new(MainPadding, MainPadding);
        tabContainer.EventTabAdded += (_) => {
            _.SetDefaultOptionPanelStyle();
            _.TextPadding = new RectOffset(0, 0, 2, 0);
        };
        tabContainer.EventContainerAdded += (_) => {
            _.size = ContainerSize;
            _.autoLayoutPadding = new RectOffset(0, 0, 4, 20);
            var scrollbar0 = UIScrollbarHelper.AddScrollbar(this, _, new Vector2(8, ContainerSize.y));
            scrollbar0.thumbObject.color = CustomUIColor.OPPrimaryBg;
            scrollbar0.relativePosition = new Vector2(Size.x - 8, MainPadding + 40);
            _.relativePosition = new Vector2(MainPadding, MainPadding + 30 + 10);
        };

        AddGeneralContainer();
        AddExtraContainer();
        AddAdvancedContainer();
    }

    private void AddGeneralContainer() {
        GeneralContainer = AddTab("OptionPanel_General");
        OptionPanelHelper.AddGroup(GeneralContainer, Localize("OptionPanel_ModInfo"));
        var flag = SingletonMod<TypeMod>.Instance.VersionType switch {
            BuildVersion.BetaDebug => "BETA DEBUG",
            BuildVersion.BetaRelease => "BETA",
            BuildVersion.StableDebug => "STABLE DEBUG",
            _ => "STABLE",
        };
        var panel0 = OptionPanelHelper.AddLabel($"{SingletonMod<TypeMod>.Instance.ModName}", $"{SingletonMod<TypeMod>.Instance.ModVersion.GetString()} {flag}");
        var label0 = panel0.Child as CustomUILabel;
        label0.BgNormalColor = (SingletonMod<TypeMod>.Instance.VersionType == BuildVersion.StableRelease) ? new Color32(76, 148, 10, 255) : ((SingletonMod<TypeMod>.Instance.VersionType == BuildVersion.BetaRelease) ? new Color32(188, 120, 6, 255) : new Color32(6, 132, 138, 255));
        label0.TextPadding = new(4, 4, 4, 2);
        label0.TextAtlas = label0.BgAtlas = CustomUIAtlas.CSSharedAtlas;
        label0.BgSprite = CustomUIAtlas.RoundedRectangle2;
        label0.width += 8;
        panel0.StartLayout();
        AddExtraModInfoProperty();
        OptionPanelHelper.AddDropDown(Localize("Language"), null, GetSupportLocales(), LocaleIndex, 310, 30, (_) => OnLanguageSelectedIndexChanged(_)
        );
        OptionPanelHelper.Reset();
        FillGeneralContainer();
    }

    protected virtual void AddExtraContainer() => FillHotkeyContainer();

    protected virtual void FillGeneralContainer() { }
    protected virtual void FillHotkeyContainer() => HotkeyContainer = AddTab("OptionPanel_Hotkeys");
    protected virtual void FillAdvancedContainer() { }
    protected virtual void AddExtraModInfoProperty() { }
    protected virtual void OnModLocaleChanged() { }
    private void AddAdvancedContainer() {
        AdvancedContainer = AddTab("OptionPanel_Advanced");
        OptionPanelHelper.AddGroup(AdvancedContainer, Localize("OptionPanel_Advanced"));
        OptionPanelHelper.AddButton(Localize("ChangeLog_Major"), null, Localize("ChangeLog"), 250, 30, ShowLog);
        OptionPanelHelper.AddButton(Localize("CompatibilityCheck_Major"), Localize("CompatibilityCheck_Minor"), Localize("Check"), 250, 30, ShowCompatibility);
        OptionPanelHelper.AddButton(Localize("ResetModMajor"), Localize("ResetModMinor"), Localize("Reset"), 250, 30, ResetSettings);
        OptionPanelHelper.Reset();
    }
    protected CustomUIScrollablePanel AddTab(string text) => tabContainer.AddContainer(Localize(text), this);

    private int LocaleIndex => (SingletonItem<TypeConfig>.Instance.LocaleId == ModLocalizationManager.USE_GAME_LANGUAGE) ? 0 : ManagerPool.GetOrCreateManager<ModLocalizationManager>().LocaleIdList.FindIndex(x => x == SingletonConfig<TypeConfig>.Instance.LocaleId);

    private void OnLanguageSelectedIndexChanged(int value) {
        ManagerPool.GetOrCreateManager<ModLocalizationManager>().OnLanguagesOptionChanged(value, _ => SingletonConfig<TypeConfig>.Instance.LocaleId = _);
        OptionPanelManager<TypeMod, TypeOptionPanel>.LocaleChanged();
        OnModLocaleChanged();
    }


    protected string[] GetSupportLocales() => ManagerPool.GetOrCreateManager<ModLocalizationManager>().GetLanguagesOption();


    ResetModWarningMessageBox messageBox;
    ResetModMessageBox messageBox1;

    protected void ResetSettings() {
        try {
            messageBox = MessageBox.Show<ResetModWarningMessageBox>();
            messageBox.Init<TypeMod>(First);
        }
        catch (Exception e) {
            LogManager.GetLogger().Error(e, $"Reset settings failed:");
            MessageBox.Show<ResetModMessageBox>().Init<TypeMod>(false);
        }

        void First() {
            LogManager.GetLogger().Info($"Resetting mod config");
            SingletonConfig<TypeConfig>.Instance = null;
            SingletonConfig<TypeConfig>.Instance = new();
            ManagerPool.GetOrCreateManager<ModLocalizationManager>().OnResetSettings(SingletonConfig<TypeConfig>.Instance);
            OptionPanelManager<TypeMod, TypeOptionPanel>.LocaleChanged();
            ResetCallback?.Invoke();
            LogManager.GetLogger().Info($"Reset mod config succeeded.");
            MessageBox.Hide(messageBox);
            messageBox1 = MessageBox.Show<ResetModMessageBox>();
            messageBox1.Init<TypeMod>(true);
        }
    }
    protected string Localize(string value) => ModLocalizationManager.Localize(value);

    private static void ShowLog() => MessageBox.Show<LogMessageBox>().Initialize<TypeMod>(false);
    private static void ShowCompatibility() => ManagerPool.GetOrCreateManager<CompatibilityManager>().ShowMessageBox();

}