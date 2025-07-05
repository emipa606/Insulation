using HarmonyLib;
using Verse;

namespace Insulation;

[HarmonyPatch(typeof(GenTemperature), nameof(GenTemperature.EqualizeTemperaturesThroughBuilding))]
public class GenTemperature_EqualizeTemperaturesThroughBuilding
{
    public static void Prefix(Building b, ref float rate)
    {
        rate = InsulateUtility.GetInsulationRate(b, rate);
    }
}