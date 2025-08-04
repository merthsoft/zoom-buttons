using System.Reflection;
using UnityEngine;
using Verse;

namespace Merthsoft.ZoomButtons;

internal static class WidgetRowExtensions
{
    private static readonly MethodInfo buttonIconRectMethod =
        typeof(WidgetRow).GetMethod("ButtonIconRect", BindingFlags.Instance | BindingFlags.Public);

    private static readonly MethodInfo incrementPositionMethod =
        typeof(WidgetRow).GetMethod("IncrementPosition", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly FieldInfo gapField =
        typeof(WidgetRow).GetField("gap", BindingFlags.Instance | BindingFlags.NonPublic);

    public static bool HoldButton(this WidgetRow row, Texture2D texture, string tooltip = null, float size = 24f, float initialDelay = 0.4f, float repeatRate = 0.1f)
    {
        if (buttonIconRectMethod == null)
            return row.ButtonIcon(texture, tooltip, doMouseoverSound: false, overrideSize: size);

        var rect = (Rect)buttonIconRectMethod.Invoke(row, [ size ]);
        var fired = Merthsoft.ZoomButtons.HoldButton.DoHoldButton(rect, texture, tooltip, initialDelay, repeatRate);

        var gap = (float)gapField.GetValue(row);
        incrementPositionMethod?.Invoke(row, [(size > 0f ? size : 24f) + gap]);

        return fired;
    }
}
