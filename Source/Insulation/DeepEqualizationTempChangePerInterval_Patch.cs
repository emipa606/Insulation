using HarmonyLib;
using Verse;

namespace Insulation;

[HarmonyPatch(typeof(RoomTempTracker), "DeepEqualizationTempChangePerInterval")]
public class DeepEqualizationTempChangePerInterval_Patch
{
    [HarmonyPostfix]
    public static void PostFix(ref float __result, Room ___room)
    {
        var avgDeepRate = InsulateUtility.GetAvgDeepRate(___room);
        if (avgDeepRate is > 0f and <= 1f)
        {
            __result *= avgDeepRate;
        }
    }
}