using HarmonyLib;
using UnityEngine;
using Verse;

namespace Merthsoft.ZoomButtons;

[StaticConstructorOnStartup]
public class ZoomButtons : Mod
{
    public static ZoomButtonsSettings Settings;

    private static Texture2D ZoomInTexture;
    private static Texture2D ZoomOutTexture;
    
    public ZoomButtons(ModContentPack content) : base(content)
    {
        Settings = GetSettings<ZoomButtonsSettings>();

        var harmony = new Harmony("Merthsoft.ZoomButtons");
        harmony.Patch(
            original: AccessTools.Method(typeof(RimWorld.PlaySettings), "DoMapControls"),
            postfix: new HarmonyMethod(typeof(ZoomButtons), nameof(DrawZoomButtons))
        );


        LongEventHandler.ExecuteWhenFinished(() =>
        {
            ZoomInTexture = ContentFinder<Texture2D>.Get("UI/Icons/ZoomIn");
            ZoomOutTexture = ContentFinder<Texture2D>.Get("UI/Icons/ZoomOut");
        });
    }

    public override string SettingsCategory() => "Zoom Buttons";

    public override void DoSettingsWindowContents(Rect inRect)
    {
        var list = new Listing_Standard();
        list.Begin(inRect);

        list.Label("Merthsoft.ZoomButtons.ZoomStep".Translate() + " " + Settings.ZoomStep.ToString("0.0"));
        Settings.ZoomStep = list.Slider(Settings.ZoomStep, 0.5f, 25f);

        list.End();
    }

    private static void DrawZoomButtons(WidgetRow row)
    {
        if (row.ButtonIcon(ZoomOutTexture, tooltip: "Merthsoft.ZoomButtons.ZoomOut".Translate()))
            Zoom(Settings.ZoomStep);

        if (row.ButtonIcon(ZoomInTexture, tooltip: "Merthsoft.ZoomButtons.ZoomIn".Translate()))
            Zoom(-Settings.ZoomStep);   
    }

    private static void Zoom(float amount)
    {
        var camera = Find.CameraDriver;
        camera.SetRootSize(camera.config.sizeRange.ClampToRange(camera.RootSize + amount));
    }
}
