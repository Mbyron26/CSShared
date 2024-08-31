using ColossalFramework.Math;
using ColossalFramework.UI;
using CSShared.Common;
using CSShared.Localization;
using System;
using UnityEngine;

namespace CSShared.UI;

public class CustomUIColorPicker : CustomUIPanel {
    protected UITextureSprite hsbField;
    protected UITextureSprite hueField;
    protected UISlicedSprite hsbIndicator;
    protected CustomUISlider hueSlider;
    protected Color rgbColor = Color.white;
    protected Color hue = Color.white;
    protected static readonly Texture2D blankTexture = UIUtils.CreateTexture(16, 16, Color.white);
    protected UIByteValueField rValueField;
    protected UIByteValueField gValueField;
    protected UIByteValueField bValueField;

    public event Action<Color> EventRGBColorChanged;

    public bool Processing { get; private set; }
    public Color RGBColor {
        get => rgbColor;
        set => OnRGBColorChanged(value, UpdateRGBColor);
    }
    public bool IsMouseHovering => m_IsMouseHovering;

    protected virtual void UpdateRGBColor(Color color) {
        UpdateHue(color);
        UpdateHSBIndicator(color);
        UpdateHSBField();
        UpdateRGBField(color);
    }

    protected Color32 ColorFromField => new(rValueField.Value, gValueField.Value, bValueField.Value, 255);

    protected virtual void OnRGBColorChanged(Color color, Action<Color> callback = null, bool invokeAction = true) {
        rgbColor = color;
        callback?.Invoke(color);
        if (invokeAction) {
            EventRGBColorChanged?.Invoke(color);
        }
    }

    public CustomUIColorPicker() => InitComponents();

    protected virtual void InitComponents() {
        size = new Vector2(246, 246);
        atlas = CustomUIAtlas.CSSharedAtlas;
        bgSprite = CustomUIAtlas.RoundedRectangle2;
        bgNormalColor = bgDisabledColor = CustomUIColor.CPPrimaryBg;

        hsbField = AddUIComponent<UITextureSprite>();
        hsbField.material = new Material(Shader.Find("UI/ColorPicker HSB"));
        hsbField.texture = blankTexture;
        hsbField.size = new Vector2(200f, 200f);
        hsbField.canFocus = true;
        hsbField.eventMouseDown += IndicatorDown;
        hsbField.eventMouseMove += IndicatorMove;
        hsbField.relativePosition = new Vector2(10, 10);

        hsbIndicator = hsbField.AddUIComponent<UISlicedSprite>();
        hsbIndicator.size = new Vector2(16, 16);
        hsbIndicator.atlas = CustomUIAtlas.InGameAtlas;
        hsbIndicator.spriteName = "ColorPickerIndicator";

        hueField = AddUIComponent<UITextureSprite>();
        hueField.material = new Material(Shader.Find("UI/ColorPicker Hue"));
        hueField.texture = blankTexture;
        hueField.size = new Vector2(16f, 200f);
        hueField.relativePosition = new Vector2(220, 10);

        hueSlider = hueField.AddUIComponent<CustomUISlider>();
        hueSlider.size = new Vector2(16f, 200f);
        hueSlider.Orientation = UIOrientation.Vertical;
        hueSlider.relativePosition = Vector2.zero;
        hueSlider.MinValue = 0f;
        hueSlider.MaxValue = 1f;
        hueSlider.StepSize = 0.01f;
        hueSlider.RawValue = 0f;
        hueSlider.EventValueChanged += OnHueChanged;

        var thumbObject = hueSlider.AddUIComponent<UISprite>();
        thumbObject.atlas = CustomUIAtlas.CSSharedAtlas;
        thumbObject.spriteName = CustomUIAtlas.CheckBoxOffBg;
        thumbObject.color = new(220, 220, 220, 255);
        thumbObject.disabledColor = new Color32(110, 110, 110, 255);
        thumbObject.size = new Vector2(14, 14);
        thumbObject.relativePosition = Vector2.zero;
        hueSlider.ThumbObject = thumbObject;
        hueSlider.ThumbPadding = new RectOffset(0, 0, 7, 7);

        var fieldsPanel = AddUIComponent<CustomUIPanel>();
        fieldsPanel.AutoLayoutDirection = LayoutDirection.Horizontal;
        fieldsPanel.ItemGap = 8;
        fieldsPanel.AutoFitChildrenHorizontally = true;
        fieldsPanel.AutoFitChildrenVertically = true;
        fieldsPanel.AutoLayout = true;
        fieldsPanel.relativePosition = new Vector2(10, 220);
        AddLabel(fieldsPanel, "R");
        rValueField = AddField(fieldsPanel);
        AddLabel(fieldsPanel, "G");
        gValueField = AddField(fieldsPanel);
        AddLabel(fieldsPanel, "B");
        bValueField = AddField(fieldsPanel);
    }

    protected virtual CustomUILabel AddLabel(UIComponent parent, string text) {
        var label = parent.AddUIComponent<CustomUILabel>();
        label.AutoSize = false;
        label.size = new Vector2(8, 20);
        label.TextHorizontalAlignment = UIHorizontalAlignment.Center;
        label.TextVerticalAlignment = UIVerticalAlignment.Middle;
        label.TextScale = 0.8f;
        label.TextPadding = new RectOffset(0, 0, 4, 0);
        label.Text = text;
        return label;
    }

    protected virtual UIByteValueField AddField(UIComponent parent) {
        var field = parent.AddUIComponent<UIByteValueField>();
        field.SelectOnFocus = true;
        field.size = new(50, 20);
        field.TextScale = 0.8f;
        field.CanWheel = true;
        field.MinValue = 0;
        field.MaxValue = 255;
        field.UseValueLimit = true;
        field.builtinKeyNavigation = true;
        field.SetControlPanelStyle();
        field.Value = 0;
        field.TextPadding = new RectOffset(0, 0, 4, 0);
        field.CursorHeight = 12;
        field.CustomCursorHeight = true;
        field.EventValueChanged += OnValueChanged;
        field.WheelStep = 10;
        field.ShowTooltip = true;
        field.tooltip = ModLocalizationManager.Localize("ScrollWheel") + "\n" + ModLocalizationManager.Localize("AltToChangeAll");
        return field;
    }

    private void OnValueChanged(byte value) {
        if (Processing)
            return;
        Processing = true;
        if (KeyBinding.IsAltDown()) {
            rValueField.Value = value;
            gValueField.Value = value;
            bValueField.Value = value;
        }
        UpdateHSBIndicator(ColorFromField);
        UpdateHue(ColorFromField);
        UpdateHSBField();
        OnRGBColorChanged(ColorFromField, null, false);
        Processing = false;
    }

    private void OnHueChanged(float value) {
        hue = new HSBColor(value, 1f, 1f, 1f).ToColor();
        UpdateHSBField();
        UpdateSelectedColor();
        UpdateRGBField(RGBColor);
    }

    private void IndicatorDown(UIComponent comp, UIMouseEventParameter p) => UpdateHSBIndicator(p);
    private void IndicatorMove(UIComponent comp, UIMouseEventParameter p) {
        if (p.buttons == UIMouseButton.Left)
            UpdateHSBIndicator(p);
    }

    private void UpdateHSBIndicator(UIMouseEventParameter p) {
        if (hsbField.GetHitPosition(p.ray, out Vector2 a)) {
            hsbIndicator.relativePosition = a - hsbIndicator.size * 0.5f;
            UpdateSelectedColor();
        }
    }

    private void UpdateSelectedColor() {
        var vector = new Vector2(hsbIndicator.relativePosition.x, hsbIndicator.relativePosition.y) + hsbIndicator.size * 0.5f;
        var rgbColor = GetColor(vector.x, vector.y, hsbField.width, hsbField.height, hue);
        OnRGBColorChanged(rgbColor);
        UpdateRGBField(RGBColor);
    }

    private void UpdateRGBField(Color32 color) {
        if (Processing)
            return;
        Processing = true;
        rValueField.Value = color.r;
        gValueField.Value = color.g;
        bValueField.Value = color.b;
        Processing = false;
    }

    private void UpdateHue(Color color) {
        var hsbColor = HSBColor.FromColor(color);
        hue = new HSBColor(hsbColor.h, 1f, 1f, 1f);
        hueSlider.RawValue = HSBColor.FromColor(hue).h;
    }

    private Color GetColor(float x, float y, float width, float height, Color hue) {
        float num = x / width;
        float num2 = y / height;
        num = Mathf.Clamp01(num);
        num2 = Mathf.Clamp01(num2);
        Color result = Color.Lerp(Color.white, hue, num) * (1f - num2);
        result.a = 1f;
        return result;
    }

    private void UpdateHSBField() {
        if (hsbField.renderMaterial is not null) {
            hsbField.renderMaterial.color = hue.gamma;
        }
    }

    private void UpdateHSBIndicator(Color color) {
        var hsbColor = HSBColor.FromColor(color);
        var size = new Vector2(hsbColor.s * hsbField.width, (1f - hsbColor.b) * hsbField.height);
        hsbIndicator.relativePosition = size - hsbIndicator.size * 0.5f;
    }

}

public enum ColorPickerPosition {
    RightBelow,
    LeftBelow,
    RightAbove,
    LeftAbove
}