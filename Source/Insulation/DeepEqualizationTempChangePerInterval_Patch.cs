using System;
using HarmonyLib;
using Verse;

namespace Insulation
{
	// Token: 0x02000006 RID: 6
	[HarmonyPatch(typeof(RoomGroupTempTracker), "DeepEqualizationTempChangePerInterval")]
	public class DeepEqualizationTempChangePerInterval_Patch
	{
		// Token: 0x06000009 RID: 9 RVA: 0x000020EC File Offset: 0x000002EC
		[HarmonyPostfix]
		public static void PostFix(ref float __result, RoomGroup ___roomGroup)
		{
			float avgDeepRate = InsulateUtility.GetAvgDeepRate(___roomGroup);
			if (avgDeepRate > 0f && avgDeepRate <= 1f)
			{
				__result *= avgDeepRate;
			}
		}
	}
}
