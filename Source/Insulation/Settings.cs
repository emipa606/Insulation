using UnityEngine;
using Verse;

namespace Insulation;

public class Settings : ModSettings
{
    public int pctEffective = 100;

    public void DoWindowContents(Rect canvas)
    {
        var listing_Standard = new Listing_Standard();
        listing_Standard.ColumnWidth = canvas.width;
        listing_Standard.Begin(canvas);
        listing_Standard.Gap();
        listing_Standard.Label("Insulation.pctEffective".Translate() + "  " + pctEffective);
        pctEffective = checked((int)listing_Standard.Slider(pctEffective, 50f, 200f));
        listing_Standard.Gap();
        listing_Standard.End();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref pctEffective, "pctEffective", 100);
    }
}