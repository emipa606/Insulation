using System.Linq;
using RimWorld;
using Verse;

namespace Insulation
{
    // Token: 0x02000009 RID: 9
    public class InsulateUtility
    {
        // Token: 0x0600000E RID: 14 RVA: 0x0000237C File Offset: 0x0000057C
        public static float GetInsulationRate(Building b, float rate)
        {
            var newrate = rate;
            var def = b.def;
            var CompsBI = def?.GetCompProperties<CompProperties_BuildInsulation>();
            if (CompsBI == null)
            {
                return newrate * 100f / Controller.Settings.pctEffective;
            }

            var HeatTransferFactor = CompsBI.HeatTransferFactor;
            var InsulationStuff = CompsBI.InsulationStuff;
            var PowerNeed = CompsBI.PowerNeeded;
            var FuelNeed = CompsBI.FuelNeeded;
            var RepairNeed = CompsBI.RepairNeeded;
            var HitPointsNeed = CompsBI.HitPointsNeed;
            if (!IsBuildingWorking(b, PowerNeed, FuelNeed, RepairNeed, HitPointsNeed))
            {
                return newrate * 100f / Controller.Settings.pctEffective;
            }

            var avgStuffCatFactor = 1f;
            if (InsulationStuff)
            {
                if (b.def.MadeFromStuff)
                {
                    var TotalStuffCatFactor = 0f;
                    var countStuffCatFactor = 0;
                    var Bstuff = b.Stuff;
                    if (Bstuff?.stuffProps.categories != null)
                    {
                        if (Bstuff.stuffProps.categories.Contains(StuffCategoryDefOf.Woody))
                        {
                            TotalStuffCatFactor += 1f;
                            countStuffCatFactor++;
                        }

                        if (Bstuff.stuffProps.categories.Contains(StuffCategoryDefOf.Stony))
                        {
                            TotalStuffCatFactor += 1.5f;
                            countStuffCatFactor++;
                        }

                        if (Bstuff.stuffProps.categories.Contains(StuffCategoryDefOf.Metallic))
                        {
                            TotalStuffCatFactor += 0.75f;
                            countStuffCatFactor++;
                        }

                        if (Bstuff.stuffProps.categories.Contains(StuffCategoryDefOf.Fabric))
                        {
                            TotalStuffCatFactor += 2f;
                            countStuffCatFactor++;
                        }

                        if (Bstuff.stuffProps.categories.Contains(StuffCategoryDefOf.Leathery))
                        {
                            TotalStuffCatFactor += 1.75f;
                            countStuffCatFactor++;
                        }

                        if (countStuffCatFactor > 0)
                        {
                            avgStuffCatFactor = TotalStuffCatFactor / countStuffCatFactor;
                        }
                    }
                }

                if (!(HeatTransferFactor > 0f))
                {
                    return newrate * 100f / Controller.Settings.pctEffective;
                }

                if (avgStuffCatFactor > 0f)
                {
                    newrate *= HeatTransferFactor / avgStuffCatFactor;
                }
                else
                {
                    newrate *= HeatTransferFactor;
                }
            }
            else if (HeatTransferFactor > 0f)
            {
                newrate *= HeatTransferFactor;
            }

            return newrate * 100f / Controller.Settings.pctEffective;
        }

        // Token: 0x0600000F RID: 15 RVA: 0x0000254C File Offset: 0x0000074C
        public static bool IsBuildingWorking(Building b, bool PowerNeed, bool FuelNeed, bool RepairNeed,
            float HitPointsNeed)
        {
            if (PowerNeed)
            {
                if (b.TryGetComp<CompPowerTrader>() == null)
                {
                    return false;
                }

                if (!b.TryGetComp<CompPowerTrader>().PowerOn)
                {
                    return false;
                }
            }

            if (!FuelNeed)
            {
                return (!RepairNeed || b.TryGetComp<CompBreakdownable>() == null || !b.IsBrokenDown()) &&
                       (HitPointsNeed <= 0f || HitPointsNeed > 1f || !b.def.useHitPoints ||
                        b.def.BaseMaxHitPoints <= 0 ||
                        b.HitPoints / (float) b.def.BaseMaxHitPoints >= HitPointsNeed);
            }

            if (b.TryGetComp<CompRefuelable>() == null)
            {
                return false;
            }

            if (!b.TryGetComp<CompRefuelable>().HasFuel)
            {
                return false;
            }

            return (!RepairNeed || b.TryGetComp<CompBreakdownable>() == null || !b.IsBrokenDown()) &&
                   (HitPointsNeed <= 0f || HitPointsNeed > 1f || !b.def.useHitPoints || b.def.BaseMaxHitPoints <= 0 ||
                    b.HitPoints / (float) b.def.BaseMaxHitPoints >= HitPointsNeed);
        }

        // Token: 0x06000010 RID: 16 RVA: 0x000025F0 File Offset: 0x000007F0
        public static float GetAvgWallRate(RoomGroup rg)
        {
            var AvgWallRate = 1f;
            var map = rg?.Map;
            if (map == null || rg.UsesOutdoorTemperature)
            {
                return AvgWallRate;
            }

            var TotalRate = 0f;
            var TotalCount = 0;
            var rooms = rg.Rooms;
            if (rooms.Count > 0)
            {
                foreach (var room in rooms)
                {
                    if (room.IsDoorway)
                    {
                        continue;
                    }

                    var bordercells = room.BorderCells.ToList();
                    if (bordercells.Count <= 0)
                    {
                        continue;
                    }

                    foreach (var cell in bordercells)
                    {
                        if (!cell.InBounds(map))
                        {
                            continue;
                        }

                        if (cell.GetEdifice(map) != null)
                        {
                            var cellEdRate = GetInsulationRate(cell.GetEdifice(map), 1f);
                            TotalRate += cellEdRate;
                            TotalCount++;
                        }
                        else
                        {
                            TotalRate += 1f;
                            TotalCount++;
                        }
                    }
                }
            }

            AvgWallRate = TotalRate / TotalCount;

            return AvgWallRate;
        }

        // Token: 0x06000011 RID: 17 RVA: 0x00002724 File Offset: 0x00000924
        public static float GetAvgThinRate(RoomGroup rg)
        {
            var AvgThinRate = 1f;
            if (rg.RoomCount <= 0)
            {
                return AvgThinRate;
            }

            var map = rg.Map;
            if (map == null || rg.UsesOutdoorTemperature)
            {
                return AvgThinRate;
            }

            var totalThinFactor = 0f;
            var totalThinCount = 0;
            foreach (var intVec in rg.Cells)
            {
                var roof = intVec.GetRoof(map);
                if (roof == null || roof.isThickRoof)
                {
                    continue;
                }

                totalThinFactor += GetRoofInsulationFactor(roof, false);
                totalThinCount++;
            }

            if (totalThinCount > 0)
            {
                AvgThinRate = totalThinFactor / totalThinCount;
            }

            return AvgThinRate;
        }

        // Token: 0x06000012 RID: 18 RVA: 0x000027CC File Offset: 0x000009CC
        public static float GetAvgDeepRate(RoomGroup rg)
        {
            var AvgDeepRate = 1f;
            if (rg.RoomCount <= 0)
            {
                return AvgDeepRate;
            }

            var map = rg.Map;
            if (map == null || rg.UsesOutdoorTemperature)
            {
                return AvgDeepRate;
            }

            var totalDeepFactor = 0f;
            var totalDeepCount = 0;
            foreach (var intVec in rg.Cells)
            {
                var roof = intVec.GetRoof(map);
                if (roof == null || !roof.isThickRoof)
                {
                    continue;
                }

                totalDeepFactor += GetRoofInsulationFactor(roof, true);
                totalDeepCount++;
            }

            if (totalDeepCount > 0)
            {
                AvgDeepRate = totalDeepFactor / totalDeepCount;
            }

            return AvgDeepRate;
        }

        // Token: 0x06000013 RID: 19 RVA: 0x00002874 File Offset: 0x00000A74
        public static float GetRoofInsulationFactor(RoofDef roof, bool isThick)
        {
            return RoofValues.GetRVInsulationFactor(roof);
        }
    }
}