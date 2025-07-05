using HarmonyLib;
using Verse;

namespace Insulation;

[HarmonyPatch(typeof(RoomTempTracker), "WallEqualizationTempChangePerInterval")]
public class RoomTempTracker_WallEqualizationTempChangePerInterval
{
    public static void Postfix(ref float __result, Room ___room)
    {
        var avgWallRate = InsulateUtility.GetAvgWallRate(___room);
        if (avgWallRate is > 0f and <= 1f)
        {
            __result *= avgWallRate;
        }
    }
}