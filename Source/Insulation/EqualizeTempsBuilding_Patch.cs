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
            lock (___beqRoomGroups)
            {
                rate = InsulateUtility.GetInsulationRate(b, rate);
                var num = 0;
                var num2 = 0f;
                if (twoWay)
                {
                    for (var i = 0; i < 2; i++)
                    {
                        var intVec = i != 0 ? b.Position - b.Rotation.FacingCell : b.Position + b.Rotation.FacingCell;
                        if (!intVec.InBounds(b.Map))
                        {
                            continue;
                        }

                        var roomGroup = intVec.GetRoomGroup(b.Map);
                        if (roomGroup == null)
                        {
                            continue;
                        }

                        num2 += roomGroup.Temperature;
                        ___beqRoomGroups[num] = roomGroup;
                        num++;
                    }
                }
                else
                {
                    for (var j = 0; j < 4; j++)
                    {
                        var intVec2 = b.Position + GenAdj.CardinalDirections[j];
                        if (!intVec2.InBounds(b.Map))
                        {
                            continue;
                        }

                        var roomGroup2 = intVec2.GetRoomGroup(b.Map);
                        if (roomGroup2 == null)
                        {
                            continue;
                        }

                        num2 += roomGroup2.Temperature;
                        ___beqRoomGroups[num] = roomGroup2;
                        num++;
                    }
                }

                if (num == 0)
                {
                    return false;
                }

                var num3 = num2 / num;
                var roomGroup3 = b.GetRoomGroup();
                if (roomGroup3 != null)
                {
                    roomGroup3.Temperature = num3;
                }

                if (num == 1)
                {
                    return false;
                }

                var num4 = 1f;
                for (var k = 0; k < num; k++)
                {
                    if (___beqRoomGroups[k].UsesOutdoorTemperature)
                    {
                        continue;
                    }

                    var temperature = ___beqRoomGroups[k].Temperature;
                    var num5 = (num3 - temperature) * rate;
                    var num6 = num5 / ___beqRoomGroups[k].CellCount;
                    var num7 = ___beqRoomGroups[k].Temperature + num6;
                    if (num5 > 0f && num7 > num3)
                    {
                        num7 = num3;
                    }
                    else if (num5 < 0f && num7 < num3)
                    {
                        num7 = num3;
                    }

                    var num8 = Mathf.Abs((num7 - temperature) * ___beqRoomGroups[k].CellCount / num5);
                    if (num8 < num4)
                    {
                        num4 = num8;
                    }
                }

                for (var l = 0; l < num; l++)
                {
                    if (___beqRoomGroups[l].UsesOutdoorTemperature)
                    {
                        continue;
                    }

                    var temperature2 = ___beqRoomGroups[l].Temperature;
                    var num9 = (num3 - temperature2) * rate * num4 / ___beqRoomGroups[l].CellCount;
                    ___beqRoomGroups[l].Temperature += num9;
                }

                for (var m = 0; m < ___beqRoomGroups.Length; m++)
                {
                    ___beqRoomGroups[m] = null;
                }

                return false;
            }
        }
    }
}