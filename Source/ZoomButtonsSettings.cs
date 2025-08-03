using Verse;

namespace Merthsoft.ZoomButtons;

public class ZoomButtonsSettings : ModSettings
{
    public float ZoomStep = 5f;
    public bool ButtonsInPlaySettings = false;
    public float ButtonsScale = 2f;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref ZoomStep, "ZoomStep", 5f);
        Scribe_Values.Look(ref ButtonsInPlaySettings, "ButtonsInPlaySettings", false);
        Scribe_Values.Look(ref ButtonsScale, "ButtonsScale", 2f);
    }
}
