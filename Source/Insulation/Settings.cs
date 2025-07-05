using UnityEngine;
using Verse;

namespace Insulation;

public class Settings : ModSettings
{
    public int pctEffective = 100;

    public void DoWindowContents(Rect canvas)
    {
        var listingStandard = new Listing_Standard
        {
            ColumnWidth = canvas.width
        };
        listingStandard.Begin(canvas);
        listingStandard.Gap();
        listingStandard.Label("Insulation.pctEffective".Translate() + "  " + pctEffective);
        pctEffective = checked((int)listingStandard.Slider(pctEffective, 50f, 200f));
        listingStandard.Gap();
        if (Controller.CurrentVersion != null)
        {
            listingStandard.Gap();
            GUI.contentColor = Color.gray;
            listingStandard.Label("Insulation.Modversion".Translate(Controller.CurrentVersion));
            GUI.contentColor = Color.white;
        }

        listingStandard.End();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref pctEffective, "pctEffective", 100);
    }
}