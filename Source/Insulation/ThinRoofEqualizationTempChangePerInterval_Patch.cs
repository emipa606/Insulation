using System;
using HarmonyLib;
using Verse;

namespace Insulation
{
	// Token: 0x0200000C RID: 12
	[HarmonyPatch(typeof(RoomGroupTempTracker), "ThinRoofEqualizationTempChangePerInterval")]
	public class ThinRoofEqualizationTempChangePerInterval_Patch
	{
		// Token: 0x0600001A RID: 26 RVA: 0x00002984 File Offset: 0x00000B84
		[HarmonyPostfix]
		public static void PostFix(ref float __result, RoomGroup ___roomGroup)
		{
			float avgThinRate = InsulateUtility.GetAvgThinRate(___roomGroup);
			if (avgThinRate > 0f && avgThinRate <= 1f)
			{
				__result *= avgThinRate;
			}
		}
	}
}
