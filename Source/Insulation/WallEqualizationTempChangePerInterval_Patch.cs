using HarmonyLib;
using Verse;

namespace Insulation;

[HarmonyPatch(typeof(RoomTempTracker), "WallEqualizationTempChangePerInterval")]
public class WallEqualizationTempChangePerInterval_Patch
{
    [HarmonyPostfix]
    public static void PostFix(ref float __result, Room ___room)
    {
        var avgWallRate = InsulateUtility.GetAvgWallRate(___room);
        if (avgWallRate is > 0f and <= 1f)
        {
            __result *= avgWallRate;
        }
    }
}