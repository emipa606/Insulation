using HarmonyLib;
using UnityEngine;
using Verse;

namespace Insulation;

[HarmonyPatch(typeof(GenTemperature), "EqualizeTemperaturesThroughBuilding")]
public class EqualizeTempsBuilding_Patch
{
    [HarmonyPrefix]
    public static bool PreFix(ref Room[] ___beqRooms, Building b, ref float rate, bool twoWay)
    {
        lock (___beqRooms)
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

                    var room = intVec.GetRoom(b.Map);
                    if (room == null)
                    {
                        continue;
                    }

                    num2 += room.Temperature;
                    ___beqRooms[num] = room;
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

                    var room2 = intVec2.GetRoom(b.Map);
                    if (room2 == null)
                    {
                        continue;
                    }

                    num2 += room2.Temperature;
                    ___beqRooms[num] = room2;
                    num++;
                }
            }

            if (num == 0)
            {
                return false;
            }

            var num3 = num2 / num;
            var room3 = b.GetRoom();
            if (room3 != null)
            {
                room3.Temperature = num3;
            }

            if (num == 1)
            {
                return false;
            }

            var num4 = 1f;
            for (var k = 0; k < num; k++)
            {
                if (___beqRooms[k].UsesOutdoorTemperature)
                {
                    continue;
                }

                var temperature = ___beqRooms[k].Temperature;
                var num5 = (num3 - temperature) * rate;
                var num6 = num5 / ___beqRooms[k].CellCount;
                var num7 = ___beqRooms[k].Temperature + num6;
                switch (num5)
                {
                    case > 0f when num7 > num3:
                    case < 0f when num7 < num3:
                        num7 = num3;
                        break;
                }

                var num8 = Mathf.Abs((num7 - temperature) * ___beqRooms[k].CellCount / num5);
                if (num8 < num4)
                {
                    num4 = num8;
                }
            }

            for (var l = 0; l < num; l++)
            {
                if (___beqRooms[l].UsesOutdoorTemperature)
                {
                    continue;
                }

                var temperature2 = ___beqRooms[l].Temperature;
                var num9 = (num3 - temperature2) * rate * num4 / ___beqRooms[l].CellCount;
                ___beqRooms[l].Temperature += num9;
            }

            for (var m = 0; m < ___beqRooms.Length; m++)
            {
                ___beqRooms[m] = null;
            }

            return false;
        }
    }
}