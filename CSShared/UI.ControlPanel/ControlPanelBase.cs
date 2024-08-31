using ColossalFramework.UI;
using CSShared.Common;
using CSShared.Localization;
using CSShared.Tools;
using UnityEngine;

namespace CSShared.UI.ControlPanel;

public abstract class ControlPanelBase<TypeMod, TypePanel> : CustomUIPanel where TypeMod : IMod where TypePanel : CustomUIPanel {
    protected UIDragHandle dragBar;
    protected CustomUILabel title;
    protected CustomUIButton closeButton;

    public virtual float PanelWidth { get; protected set; } = 440;
    public virtual float PanelHeight { get; protected set; } = 600;
    public virtual float ElementOffset { get; protected set; } = 10;
    public virtual float CaptionHeight { get; protected set; } = 40;
    public static Vector2 PanelPosition { get; set; }
    public static Vector2 ButtonSize { get; } = new(28, 28);
    public virtual float PropertyPanelWidth => PanelWidth - 2 * 16;

    public override void Start() {
        base.Start();
        name = AssemblyTools.CurrentAssemblyName + "ControlPanel";
        size = new Vector2(PanelWidth, PanelHeight);
        atlas = CustomUIAtlas.CSSharedAtlas;
        bgSprite = CustomUIAtlas.CustomBackground;
        //isVisible = true;
        //canFocus = true;
        //isInteractive = true;
        InitComponents();
        SetPosition();
        eventPositionChanged += (c, v) => PanelPosition = relativePosition;
    }

    protected string Localize(string key) => ModLocalizationManager.Localize(key);

    protected virtual void InitComponents() {
        AddButtons();
        AddDragBar(closeButton.relativePosition.x);
    }
    protected virtual void SetPosition() {
        if (PanelPosition == Vector2.zero) {
            Vector2 vector = GetUIView().GetScreenResolution();
            var x = vector.x - PanelWidth - 200;
            PanelPosition = relativePosition = new Vector3(x, 80);
        }
        else {
            relativePosition = PanelPosition;
        }
    }
    protected virtual void AddButtons() {
        closeButton = AddUIComponent<CustomUIButton>();
        closeButton.BgAtlas = CustomUIAtlas.CSSharedAtlas;
        closeButton.size = ButtonSize;
        closeButton.OnBgSprites.SetSprites(CustomUIAtlas.CloseButton);
        closeButton.OnBgSprites.SetColors(CustomUIColor.White, CustomUIColor.OffWhite, new Color32(180, 180, 180, 255), CustomUIColor.White, CustomUIColor.White);
        closeButton.relativePosition = new Vector2(width - ElementOffset - 28f, 6f);
        closeButton.eventClicked += (c, p) => ControlPanelManager<TypeMod, TypePanel>.Close();
    }
    protected virtual void AddDragBar(float width) {
        title = AddUIComponent<CustomUILabel>();
        title.AutoSize = false;
        title.size = new Vector2(width, CaptionHeight);
        title.relativePosition = Vector2.zero;
        title.TextHorizontalAlignment = UIHorizontalAlignment.Center;
        title.TextVerticalAlignment = UIVerticalAlignment.Middle;
        title.Text = SingletonMod<TypeMod>.Instance.ModName;

        dragBar = AddUIComponent<UIDragHandle>();
        dragBar.width = width;
        dragBar.height = CaptionHeight;
        dragBar.relativePosition = Vector2.zero;
    }

}