using ColossalFramework.UI;
using System;
using UnityEngine;

namespace CSShared.UI;

public class CustomUIColorField : CustomUIColorFieldBase<CustomUIColorPicker> { }

public abstract class CustomUIColorFieldBase<T> : CustomUIButtonBase where T : CustomUIColorPicker {
    private Color selectedColor;
    protected ColorPickerPosition pickerPosition = ColorPickerPosition.RightBelow;
    private T popup;
    private Color undoColor;

    public event Action<Color> EventOnSelectedColorChanged;
    public event Action<T> EventPopupOpen;
    public event Action<T> EventPopupClose;

    public virtual Color SelectedColor {
        get => selectedColor;
        set {
            if (!value.Equals(selectedColor)) {
                selectedColor = value;
                Invalidate();
            }
        }
    }
    public ColorPickerPosition PickerPosition {
        get => pickerPosition;
        set {
            if (value != pickerPosition) {
                ClosePopup();
                pickerPosition = value;
                Invalidate();
            }
        }
    }

    public CustomUIColorFieldBase() {
        fgSpriteMode = ForegroundSpriteMode.Scale;
        fgScaleFactor = 0.8f;
        RenderFg = true;
    }

    private void CheckForPopupClose() {
        if (popup is null || !Input.GetMouseButtonDown(0)) {
            return;
        }
        Camera camera = GetCamera();
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (popup.Raycast(ray) || IsHovering) {
            return;
        }
        ClosePopup();
    }

    public override void OnDisable() {
        base.OnDisable();
        ClosePopup();
    }

    public override void OnDestroy() {
        base.OnDestroy();
        ClosePopup();
    }

    public override void Update() {
        base.Update();
        CheckForPopupClose();
    }

    public override void LateUpdate() {
        base.LateUpdate();
        if (!Application.isPlaying) {
            return;
        }
    }

    protected override void OnClick(UIMouseEventParameter p) {
        base.OnClick(p);
        if (popup is not null) {
            ClosePopup();
        } else {
            OpenPopup();
        }
    }

    private bool OpenPopup() {
        undoColor = selectedColor;
        if (popup != null) {
            return false;
        }
        UIComponent uicomponent = GetRootContainer();
        popup = uicomponent.AddUIComponent<T>();
        popup.Focus();
        popup.RGBColor = selectedColor;
        popup.EventRGBColorChanged += OnSelectedColorChanged;
        popup.eventKeyDown += OnPopupKeyDown;
        Vector3 position = CalculatePopupPosition();
        popup.transform.position = position;
        popup.transform.rotation = transform.rotation;
        EventPopupOpen?.Invoke(popup);
        return true;
    }

    private void OnPopupKeyDown(UIComponent comp, UIKeyEventParameter p) {
        if (builtinKeyNavigation) {
            if (p.keycode == KeyCode.Space || p.keycode == KeyCode.Return) {
                ClosePopup();
                p.Use();
                return;
            }
            if (p.keycode == KeyCode.Escape) {
                selectedColor = undoColor;
                popup.RGBColor = selectedColor;
                ClosePopup();
                p.Use();
            }
        }
    }

    private void ClosePopup() {
        if (popup is null) {
            return;
        }
        popup.EventRGBColorChanged -= OnSelectedColorChanged;
        EventPopupClose?.Invoke(popup);
        Destroy(popup.gameObject);
        popup = null;
    }

    protected virtual void OnSelectedColorChanged(Color color) {
        SelectedColor = color;
        EventOnSelectedColorChanged?.Invoke(color);
    }

    private Vector3 CalculatePopupPosition() {
        float num = PixelsToUnits();
        Vector3 a = pivot.TransformToUpperLeft(size, arbitraryPivotOffset);
        Vector3 a2 = transform.position + a * num;
        Vector3 scaledDirection = GetScaledDirection(Vector3.down);
        Vector3 scaledDirection2 = GetScaledDirection(Vector3.right);
        Vector3 vector = Vector3.zero;
        if (PickerPosition == ColorPickerPosition.RightAbove) {
            vector = a2 - scaledDirection * popup.size.y * num;
        } else if (PickerPosition == ColorPickerPosition.LeftAbove) {
            vector = a2 - scaledDirection * popup.size.y * num - scaledDirection2 * (popup.size.x - size.x) * num;
        } else if (PickerPosition == ColorPickerPosition.RightBelow) {
            vector = a2 + scaledDirection * size.y * num;
        } else if (PickerPosition == ColorPickerPosition.LeftBelow) {
            vector = a2 + scaledDirection * size.y * num - scaledDirection2 * (popup.size.x - size.x) * num;
        }
        Vector3 a3 = popup.transform.parent.position / num + popup.parent.pivot.TransformToUpperLeft(size, arbitraryPivotOffset);
        Vector3 vector2 = a3 + scaledDirection * popup.parent.size.y + scaledDirection2 * popup.parent.size.x;
        Vector3 a4 = vector / num;
        Vector3 vector3 = a4 + scaledDirection * popup.size.y + scaledDirection2 * popup.size.x;
        if (a4.x < a3.x) {
            vector.x = a2.x;
        } else if (vector3.x > vector2.x) {
            vector.x = a2.x - (popup.size.x - size.x) * num;
        }
        if (a4.y > a3.y) {
            vector.y = a2.y - size.y * num;
        } else if (vector3.y < vector2.y) {
            vector.y = a2.y + popup.size.y * num;
        }
        return vector;
    }

    protected override UITextureAtlas.SpriteInfo GetFgSprite() {
        if (fgAtlas is null) {
            return null;
        }
        return isEnabled ? fgAtlas[OnFgNormalSprite] : fgAtlas[OnFgDisabledSprite];
    }
    protected override Color32 GetFgActiveColor() => SelectedColor;

}
