using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Reflection;
using System.Runtime;
using UnityEngine;
using Verse;

namespace Merthsoft.ZoomButtons;

[StaticConstructorOnStartup]
public class ZoomButtons : Mod
{
    public static ZoomButtonsSettings Settings;
    public static float ZoomButtonSize => Settings.ButtonsInPlaySettings ? 24f : 24 * Settings.ButtonsScale;

    private static readonly FieldInfo DesiredAltitudeField = AccessTools.Field(typeof(WorldCameraDriver), "desiredAltitude");
    private static Texture2D ZoomInTexture;
    private static Texture2D ZoomOutTexture;
    
    public ZoomButtons(ModContentPack content) : base(content)
    {
        Settings = GetSettings<ZoomButtonsSettings>();

        var harmony = new Harmony("Merthsoft.ZoomButtons");
        harmony.Patch(
            original: AccessTools.Method(typeof(PlaySettings), nameof(PlaySettings.DoPlaySettingsGlobalControls)),
            postfix: new HarmonyMethod(typeof(ZoomButtons), nameof(DrawZoomButtonsInPlaySettings))
        );

        harmony.Patch(
            original: AccessTools.Method(typeof(GlobalControlsUtility), nameof(GlobalControlsUtility.DoTimespeedControls)),
            postfix: new HarmonyMethod(typeof(ZoomButtons), nameof(DrawZoomButtonsInTimeControls))
        );

        LongEventHandler.ExecuteWhenFinished(() =>
        {
            ZoomInTexture = ContentFinder<Texture2D>.Get("UI/Icons/ZoomIn");
            ZoomOutTexture = ContentFinder<Texture2D>.Get("UI/Icons/ZoomOut");
        });
    }

    public override string SettingsCategory() => "Merthsoft.ZoomButtons.ZoomButtons".Translate();

    public override void DoSettingsWindowContents(Rect inRect)
    {
        var list = new Listing_Standard();
        list.Begin(inRect);

        list.Label("Merthsoft.ZoomButtons.ZoomStep".Translate(Settings.ZoomStep.ToString("0.0")));
        Settings.ZoomStep = list.Slider(Settings.ZoomStep, 0.5f, 25f);

        list.CheckboxLabeled("Merthsoft.ZoomButtons.ButtonsInPlaySettings".Translate(), ref Settings.ButtonsInPlaySettings);

        if (!Settings.ButtonsInPlaySettings)
        {
            list.Label("Merthsoft.ZoomButtons.ButtonsScale".Translate(Settings.ButtonsScale.ToString("0.0")));
            Settings.ButtonsScale = list.Slider(Settings.ButtonsScale, 1f, 6f);
            Settings.ButtonsScale = Mathf.Round(Settings.ButtonsScale * 2f) / 2f;
        }

        list.End();
    }


    public static void DrawZoomButtonsInPlaySettings(WidgetRow row)
    {
        if (!Settings.ButtonsInPlaySettings)
            return;

        if (row.ButtonIcon(ZoomOutTexture, tooltip: "Merthsoft.ZoomButtons.ZoomOut".Translate()))
            Zoom(Settings.ZoomStep);

        if (row.ButtonIcon(ZoomInTexture, tooltip: "Merthsoft.ZoomButtons.ZoomIn".Translate()))
            Zoom(-Settings.ZoomStep);
    }

    public static void DrawZoomButtonsInTimeControls(float leftX, float width, ref float curBaseY)
    {
        if (Settings.ButtonsInPlaySettings)
            return;

        Log.Message($"leftX: {leftX} width: {width} curBaseY: {curBaseY}");

        var buttonSize = ZoomButtonSize;
        var spacing = 4f;
        var fitsOneRow = (buttonSize * 2 + spacing) <= width;
        var rows = fitsOneRow ? 1 : 2;
        var groupHeight = rows * buttonSize + (rows - 1) * spacing;

        curBaseY -= groupHeight + spacing;
        var rect = new Rect(leftX, curBaseY, width, groupHeight);
        Widgets.BeginGroup(rect);

        var x = rect.width - (fitsOneRow ? (buttonSize * 2 + spacing) : buttonSize);
        var y = 0f;

        var buttonRect = new Rect(x, y, buttonSize, buttonSize);

        if (Widgets.ButtonImage(buttonRect, ZoomInTexture, false, "Merthsoft.ZoomButtons.ZoomIn".Translate()))
            Zoom(-Settings.ZoomStep);

        if (fitsOneRow)
            buttonRect.x += buttonSize + spacing;
        else
            buttonRect.y += buttonSize + spacing;

        if (Widgets.ButtonImage(buttonRect, ZoomOutTexture, false, "Merthsoft.ZoomButtons.ZoomOut".Translate()))
            Zoom(Settings.ZoomStep);

        Widgets.EndGroup();
    }


    private static void Zoom(float amount)
    {
        if (WorldRendererUtility.DrawingMap)
        {
            var camera = Find.CameraDriver;
            var clampedValue = camera.config.sizeRange.ClampToRange(camera.RootSize + amount);
            camera.SetRootSize(clampedValue);
        }
        else
        {
            var driver = Find.WorldCameraDriver;
            var clampedValue = Mathf.Clamp(driver.altitude + 2f*amount, WorldCameraDriver.MinAltitude, 1100f);
            DesiredAltitudeField.SetValue(driver, clampedValue);
        }
    }
}

