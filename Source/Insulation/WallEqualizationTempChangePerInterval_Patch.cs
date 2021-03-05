using HarmonyLib;
using Verse;

namespace Insulation
{
    // Token: 0x02000002 RID: 2
    [HarmonyPatch(typeof(RoomGroupTempTracker), "WallEqualizationTempChangePerInterval")]
    public class WallEqualizationTempChangePerInterval_Patch
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        [HarmonyPostfix]
        public static void PostFix(ref float __result, RoomGroup ___roomGroup)
        {
            var avgWallRate = InsulateUtility.GetAvgWallRate(___roomGroup);
            if (avgWallRate > 0f && avgWallRate <= 1f)
            {
                __result *= avgWallRate;
            }
        }
    }
}