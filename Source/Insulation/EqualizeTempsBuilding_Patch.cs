using HarmonyLib;
using Verse;

namespace Insulation;

[HarmonyPatch(typeof(GenTemperature), nameof(GenTemperature.EqualizeTemperaturesThroughBuilding))]
public class EqualizeTempsBuilding_Patch
{
    [HarmonyPrefix]
    public static void PreFix(Building b, ref float rate)
    {
        rate = InsulateUtility.GetInsulationRate(b, rate);
    }
}