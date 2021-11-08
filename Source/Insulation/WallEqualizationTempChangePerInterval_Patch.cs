using HarmonyLib;
using Verse;

namespace Insulation
{
    // Token: 0x02000002 RID: 2
    [HarmonyPatch(typeof(RoomTempTracker), "WallEqualizationTempChangePerInterval")]
    public class WallEqualizationTempChangePerInterval_Patch
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
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
}