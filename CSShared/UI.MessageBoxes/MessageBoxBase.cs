using ColossalFramework;
using ColossalFramework.UI;
using CSShared.Localization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CSShared.UI.MessageBoxes;

public static class MessageBox {
    public static T Show<T>() where T : MessageBoxBase {
        var uiObject = new GameObject();
        uiObject.transform.parent = UIView.GetAView().transform;
        var messageBox = uiObject.AddComponent<T>();

        UIView.PushModal(messageBox);
        messageBox.Show(true);
        messageBox.Focus();

        if (UIView.GetAView().panelsLibraryModalEffect is UIComponent modalEffect) {
            modalEffect.FitTo(null);
            if (!modalEffect.isVisible || modalEffect.opacity != 1f) {
                modalEffect.Show(false);
                ValueAnimator.Cancel("ModalEffect67419");
                ValueAnimator.Animate("ModalEffect67419", val => modalEffect.opacity = val, new AnimatedFloat(0f, 1f, 0.7f, EasingType.CubicEaseOut));
            }
        }

        return messageBox;
    }
    public static void Hide(MessageBoxBase messageBox) {
        if (messageBox == null || UIView.GetModalComponent() != messageBox)
            return;

        UIView.PopModal();

        if (UIView.GetAView().panelsLibraryModalEffect is UIComponent modalEffect) {
            if (!UIView.HasModalInput()) {
                ValueAnimator.Cancel("ModalEffect67419");
                ValueAnimator.Animate("ModalEffect67419", val => modalEffect.opacity = val, new AnimatedFloat(1f, 0f, 0.7f, EasingType.CubicEaseOut), () => modalEffect.Hide());
            }
            else
                modalEffect.zOrder = UIView.GetModalComponent().zOrder - 1;
        }

        messageBox.Hide();
        UnityEngine.Object.Destroy(messageBox.gameObject);
        UnityEngine.Object.Destroy(messageBox);
    }
}

public abstract class MessageBoxBase : CustomUIPanel {
    protected const int defaultHeight = 200;
    protected const float buttonHeight = 34f;
    protected AutoSizeUIScrollablePanel contentPanel;
    protected CustomUIPanel buttonPanel;
    protected UIDragHandle dragBar;
    protected CustomUILabel title;

    public string TitleText { set => title.Text = value; }
    protected CustomUIScrollablePanel MainPanel => contentPanel.MainPanel;
    protected List<CustomUIButton> Buttons { get; set; } = new();
    private float MaxScrollableContentHeight => GetUIView().GetScreenResolution().y - 600f;

    public MessageBoxBase() {
        isVisible = true;
        canFocus = true;
        isInteractive = true;
        size = new Vector2(MessageBoxParm.Width, defaultHeight);
        atlas = CustomUIAtlas.CSSharedAtlas;
        bgSprite = CustomUIAtlas.CustomBackground;
        InitComponents();
        Resize();
        contentPanel.eventSizeChanged += (component, size) => Resize();
    }

    protected string Localize(string value) => ModLocalizationManager.Localize(value);
    protected void Close() => MessageBox.Hide(this);

    protected virtual void Resize() {
        height = MessageBoxParm.DragBarHeight + contentPanel.height + MessageBoxParm.ButtonPanelHeight;
        contentPanel.relativePosition = new Vector2(0f, MessageBoxParm.DragBarHeight);
        buttonPanel.relativePosition = new Vector2(0f, MessageBoxParm.DragBarHeight + contentPanel.height);
    }

    public CustomUILabel AddLabelInMainPanel(string text) {
        var label = MainPanel.AddUIComponent<CustomUILabel>();
        label.AutoSize = false;
        label.AutoHeight = true;
        label.width = MessageBoxParm.ComponentWidth;
        label.TextHorizontalAlignment = UIHorizontalAlignment.Center;
        label.WordWrap = true;
        label.Text = text;
        return label;
    }

    protected void InitComponents() {
        title = AddUIComponent<CustomUILabel>();
        title.AutoSize = false;
        title.size = new Vector2(MessageBoxParm.Width, MessageBoxParm.DragBarHeight);
        title.TextHorizontalAlignment = UIHorizontalAlignment.Center;
        title.TextVerticalAlignment = UIVerticalAlignment.Middle;
        title.TextScale = 1.3f;
        title.TextPadding = new RectOffset(0, 0, 16, 0);
        title.Font = CustomUIFontHelper.SemiBold;
        title.relativePosition = Vector2.zero;

        dragBar = AddUIComponent<UIDragHandle>();
        dragBar.size = new Vector2(MessageBoxParm.Width, MessageBoxParm.DragBarHeight);
        dragBar.relativePosition = Vector2.zero;

        contentPanel = AddUIComponent<AutoSizeUIScrollablePanel>();
        contentPanel.Size = new Vector2(MessageBoxParm.Width, 200);
        contentPanel.MaxSize = new Vector2(MessageBoxParm.Width, MaxScrollableContentHeight);
        contentPanel.MainPanel.autoLayoutPadding = new RectOffset(20, 20, 4, 10);
        contentPanel.MainPanel.verticalScrollbar.thumbObject.color = CustomUIColor.CPPrimaryBg;

        buttonPanel = AddUIComponent<CustomUIPanel>();
        buttonPanel.size = new Vector2(MessageBoxParm.Width, MessageBoxParm.ButtonPanelHeight);
    }

    protected CustomUIButton AddButtons(uint number, uint total, string text, Action callback) {
        var spacing = (total - 1) * MessageBoxParm.Padding;
        var buttonWidth = (MessageBoxParm.Width - 2 * MessageBoxParm.Padding - spacing) / total;
        var button = CustomUIButton.Add(buttonPanel, text, buttonWidth, buttonHeight, callback, 1f);
        button.OnBgSprites.SetColors(CustomUIColor.CPPrimaryBg, CustomUIColor.CPButtonHovered, CustomUIColor.CPButtonPressed, CustomUIColor.CPPrimaryBg, CustomUIColor.CPButtonDisabled);
        ArrangePosition(button, number, buttonWidth);
        return button;
    }

    protected CustomUIButton AddButton(string text, Action onButtonClicked) {
        var button = CustomUIButton.Add(buttonPanel, text, 10, buttonHeight, onButtonClicked, 1f);
        button.OnBgSprites.SetColors(CustomUIColor.CPPrimaryBg, CustomUIColor.CPButtonHovered, CustomUIColor.CPButtonPressed, CustomUIColor.CPPrimaryBg, CustomUIColor.CPButtonDisabled);
        Buttons.Add(button);
        ArrangeButtons();
        return button;
    }

    private void ArrangeButtons() {
        var count = Buttons.Count;
        var buttonWidth = (MessageBoxParm.ComponentWidth - (count - 1) * 10) / count;
        float offsetX = MessageBoxParm.Padding;
        for (int i = 0; i < count; i++) {
            Buttons[i].width = buttonWidth;
            Buttons[i].relativePosition = new Vector2(offsetX, (buttonPanel.height - buttonHeight) / 2);
            offsetX += buttonWidth + 10;
        }
    }

    private CustomUIButton ArrangePosition(CustomUIButton button, uint number, float buttonWidth) {
        button.name = "Button" + number.ToString();
        var posX = MessageBoxParm.Padding + (number - 1) * (buttonWidth + MessageBoxParm.Padding);
        float posY = (MessageBoxParm.ButtonPanelHeight - button.height) / 2;
        button.relativePosition = new Vector2(posX, posY);
        return button;
    }

    private Vector2 SizeBefore { get; set; } = new Vector2();
    protected override void OnSizeChanged() {
        base.OnSizeChanged();
        var resolution = GetUIView().GetScreenResolution();
        var delta = (size - SizeBefore) / 2;
        SizeBefore = size;
        var newPosition = Vector2.Max(Vector2.Min((Vector2)relativePosition - delta, resolution - size), Vector2.zero);
        relativePosition = newPosition;
    }
    protected override void OnKeyDown(UIKeyEventParameter p) {
        if (!p.used) {
            if (p.keycode == KeyCode.Escape) {
                p.Use();
                Close();
            }
        }
    }
}

public record struct MessageBoxParm {
    public const float Width = 600;
    public const int Padding = 20;
    public const float ComponentWidth = 560;
    public const float DragBarHeight = 80;
    public const float ButtonPanelHeight = 74;
}