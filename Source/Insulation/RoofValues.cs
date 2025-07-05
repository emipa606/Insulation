using Verse;

namespace Insulation;

internal class RoofValues : DefModExtension
{
    private readonly float InsulationFactor = 1f;

    internal static float GetRVInsulationFactor(RoofDef rdef)
    {
        var RIFactor = 1f;
        if (rdef.HasModExtension<RoofValues>())
        {
            RIFactor = rdef.GetModExtension<RoofValues>().InsulationFactor;
        }

        return RIFactor;
    }
}