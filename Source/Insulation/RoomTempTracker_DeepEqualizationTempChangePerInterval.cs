using HarmonyLib;
using Verse;

namespace Insulation;

[HarmonyPatch(typeof(RoomTempTracker), "DeepEqualizationTempChangePerInterval")]
public class RoomTempTracker_DeepEqualizationTempChangePerInterval
{
    public static void Postfix(ref float __result, Room ___room)
    {
        var avgDeepRate = InsulateUtility.GetAvgDeepRate(___room);
        if (avgDeepRate is > 0f and <= 1f)
        {
            __result *= avgDeepRate;
        }
    }
}