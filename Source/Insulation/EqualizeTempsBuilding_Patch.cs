using System;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace Insulation
{
	// Token: 0x02000007 RID: 7
	[HarmonyPatch(typeof(GenTemperature), "EqualizeTemperaturesThroughBuilding")]
	public class EqualizeTempsBuilding_Patch
	{
		// Token: 0x0600000B RID: 11 RVA: 0x00002120 File Offset: 0x00000320
		[HarmonyPrefix]
		public static bool PreFix(ref RoomGroup[] ___beqRoomGroups, Building b, ref float rate, bool twoWay)
		{
			rate = InsulateUtility.GetInsulationRate(b, rate);
			int num = 0;
			float num2 = 0f;
			if (twoWay)
			{
				for (int i = 0; i < 2; i++)
				{
					IntVec3 intVec = (i != 0) ? (b.Position - b.Rotation.FacingCell) : (b.Position + b.Rotation.FacingCell);
					if (GenGrid.InBounds(intVec, b.Map))
					{
						RoomGroup roomGroup = GridsUtility.GetRoomGroup(intVec, b.Map);
						if (roomGroup != null)
						{
							num2 += roomGroup.Temperature;
							___beqRoomGroups[num] = roomGroup;
							num++;
						}
					}
				}
			}
			else
			{
				for (int j = 0; j < 4; j++)
				{
					IntVec3 intVec2 = b.Position + GenAdj.CardinalDirections[j];
					if (GenGrid.InBounds(intVec2, b.Map))
					{
						RoomGroup roomGroup2 = GridsUtility.GetRoomGroup(intVec2, b.Map);
						if (roomGroup2 != null)
						{
							num2 += roomGroup2.Temperature;
							___beqRoomGroups[num] = roomGroup2;
							num++;
						}
					}
				}
			}
			if (num == 0)
			{
				return false;
			}
			float num3 = num2 / (float)num;
			RoomGroup roomGroup3 = RegionAndRoomQuery.GetRoomGroup(b);
			if (roomGroup3 != null)
			{
				roomGroup3.Temperature = num3;
			}
			if (num == 1)
			{
				return false;
			}
			float num4 = 1f;
			for (int k = 0; k < num; k++)
			{
				if (!___beqRoomGroups[k].UsesOutdoorTemperature)
				{
					float temperature = ___beqRoomGroups[k].Temperature;
					float num5 = (num3 - temperature) * rate;
					float num6 = num5 / (float)___beqRoomGroups[k].CellCount;
					float num7 = ___beqRoomGroups[k].Temperature + num6;
					if (num5 > 0f && num7 > num3)
					{
						num7 = num3;
					}
					else if (num5 < 0f && num7 < num3)
					{
						num7 = num3;
					}
					float num8 = Mathf.Abs((num7 - temperature) * (float)___beqRoomGroups[k].CellCount / num5);
					if (num8 < num4)
					{
						num4 = num8;
					}
				}
			}
			for (int l = 0; l < num; l++)
			{
				if (!___beqRoomGroups[l].UsesOutdoorTemperature)
				{
					float temperature2 = ___beqRoomGroups[l].Temperature;
					float num9 = (num3 - temperature2) * rate * num4 / (float)___beqRoomGroups[l].CellCount;
					___beqRoomGroups[l].Temperature += num9;
				}
			}
			for (int m = 0; m < ___beqRoomGroups.Length; m++)
			{
				___beqRoomGroups[m] = null;
			}
			return false;
		}
	}
}
