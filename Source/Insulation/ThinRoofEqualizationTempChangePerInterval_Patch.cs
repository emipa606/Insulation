using HarmonyLib;
using Verse;

namespace Insulation;

[HarmonyPatch(typeof(RoomTempTracker), "ThinRoofEqualizationTempChangePerInterval")]
public class ThinRoofEqualizationTempChangePerInterval_Patch
{
    [HarmonyPostfix]
    public static void PostFix(ref float __result, Room ___room)
    {
        var avgThinRate = InsulateUtility.GetAvgThinRate(___room);
        if (avgThinRate is > 0f and <= 1f)
        {
            __result *= avgThinRate;
        }
    }
}