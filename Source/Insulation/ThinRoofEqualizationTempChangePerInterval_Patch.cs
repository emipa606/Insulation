using HarmonyLib;
using Verse;

namespace Insulation
{
    // Token: 0x0200000C RID: 12
    [HarmonyPatch(typeof(RoomTempTracker), "ThinRoofEqualizationTempChangePerInterval")]
    public class ThinRoofEqualizationTempChangePerInterval_Patch
    {
        // Token: 0x0600001A RID: 26 RVA: 0x00002984 File Offset: 0x00000B84
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
}