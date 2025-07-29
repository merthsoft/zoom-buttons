using HarmonyLib;
using UnityEngine;
using Verse;

namespace Merthsoft.ZoomButtons;

[StaticConstructorOnStartup]
public class ZoomButtons : Mod
{
    private static Texture2D ZoomInTexture;
    private static Texture2D ZoomOutTexture;
    
    public ZoomButtons(ModContentPack content) : base(content)
    {
        var harmony = new Harmony("Merthsoft.ZoomButtons");
        harmony.Patch(
            AccessTools.Method(typeof(RimWorld.TimeControls), "DoTimeControlsGUI"),
            postfix: new HarmonyMethod(typeof(ZoomButtons), nameof(DrawZoomButtons))
        );

        LongEventHandler.ExecuteWhenFinished(() =>
        {
            ZoomInTexture = ContentFinder<Texture2D>.Get("UI/Icons/ZoomIn");
            ZoomOutTexture = ContentFinder<Texture2D>.Get("UI/Icons/ZoomOut");
        });
    }

    private static void DrawZoomButtons(Rect timerRect)
    {
        var buttonSize = 24f;
        var padding = 4f;

        var zoomInRect = new Rect(timerRect.x, timerRect.yMax + padding, buttonSize, buttonSize);
        if (Widgets.ButtonImage(zoomInRect, ZoomInTexture, doMouseoverSound: false))
            ZoomIn();

        var zoomOutRect = new Rect(zoomInRect.xMax + padding, zoomInRect.y, buttonSize, buttonSize);
        if (Widgets.ButtonImage(zoomOutRect, ZoomOutTexture, doMouseoverSound: false))
            ZoomOut();

        TooltipHandler.TipRegion(zoomInRect, "Merthsoft.ZoomButtons.ZoomIn".Translate());
        TooltipHandler.TipRegion(zoomOutRect, "Merthsoft.ZoomButtons.ZoomOut".Translate());
    }

    private static void ZoomIn()
    {
        CameraDriver camera = Find.CameraDriver;
        camera.SetRootSize(camera.config.sizeRange.ClampToRange(camera.RootSize - 1f));
    }

    private static void ZoomOut()
    {
        CameraDriver camera = Find.CameraDriver;
        camera.SetRootSize(camera.config.sizeRange.ClampToRange(camera.RootSize + 1f));
    }
}
