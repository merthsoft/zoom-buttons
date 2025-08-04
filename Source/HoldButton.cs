using System;
using UnityEngine;
using Verse;

namespace Merthsoft.ZoomButtons;

public static class HoldButton
{
    public static readonly Color HoverGray = new(0.85f, 0.85f, 0.85f);
    public static readonly Color PressedOverlay = new(0f, 0f, 0f, 0.3f);

    private static bool isHeld;
    private static float nextFireTime;
    private static bool initialDelayPassed;

    public static bool DoHoldButton(Rect rectangle, Texture2D texture, string tooltip = null, float initialDelay = 0.4f, float repeatRate = 0.1f)
    {
        var fired = false;
        var mouseOver = Mouse.IsOver(rectangle);
        var oldColor = GUI.color;

        if (mouseOver)
            GUI.color = HoverGray;

        GUI.DrawTexture(rectangle, texture);

        if (!isHeld)
            TooltipHandler.TipRegion(rectangle, tooltip);

        if (isHeld && Input.GetMouseButton(0) && mouseOver)
        {
            GUI.color = PressedOverlay;
            GUI.DrawTexture(rectangle, BaseContent.WhiteTex);
        }

        GUI.color = oldColor;

        if (mouseOver && Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            isHeld = true;
            initialDelayPassed = false;
            nextFireTime = Time.realtimeSinceStartup + initialDelay;
            fired = true;
            Event.current.Use();
        }

        if (!isHeld)
            return fired;

        if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            isHeld = false;
            Event.current.Use();
            return fired;
        }

        if (isHeld && Event.current.type == EventType.Repaint && Input.GetMouseButton(0) && mouseOver)
        {
            if (!initialDelayPassed && Time.realtimeSinceStartup >= nextFireTime)
            {
                initialDelayPassed = true;
                nextFireTime = Time.realtimeSinceStartup + repeatRate;
                fired = true;
            }
            else if (initialDelayPassed && Time.realtimeSinceStartup >= nextFireTime)
            {
                nextFireTime = Time.realtimeSinceStartup + repeatRate;
                fired = true;
            }
        }

        return fired;
    }
}
