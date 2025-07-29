using Verse;

namespace Merthsoft.ZoomButtons;

public class ZoomButtonsSettings : ModSettings
{
    public float ZoomStep = 5f;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref ZoomStep, "ZoomStep", 5f);
    }
}
