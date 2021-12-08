using System;
using Verse;

namespace Insulation;

public class CompBuildInsulate : ThingComp
{
    public CompProperties_BuildInsulation Props => (CompProperties_BuildInsulation)props;

    public override string CompInspectStringExtra()
    {
        return
            $"{"Insulation.Name".Translate()}: {Math.Round((1 - InsulateUtility.GetInsulationRate((Building)parent, 1)) * 100)}%";
    }
}