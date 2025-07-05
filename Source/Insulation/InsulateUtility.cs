using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Insulation;

public class InsulateUtility
{
    private static Dictionary<Room, float> CachedRoomWallRates;
    private static Dictionary<Room, float> CachedRoomThinRates;
    private static Dictionary<Room, float> CachedRoomDeepRates;

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

    private static bool IsBuildingWorking(Building b, bool PowerNeed, bool FuelNeed, bool RepairNeed,
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
                    b.HitPoints / (float)b.def.BaseMaxHitPoints >= HitPointsNeed);
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
                b.HitPoints / (float)b.def.BaseMaxHitPoints >= HitPointsNeed);
    }

    public static float GetAvgWallRate(Room roomToCheck)
    {
        CachedRoomWallRates ??= new Dictionary<Room, float>();

        if (CachedRoomWallRates.ContainsKey(roomToCheck) && GenTicks.TicksAbs % GenTicks.TickRareInterval != 0)
        {
            return CachedRoomWallRates[roomToCheck];
        }

        var AvgWallRate = 1f;
        var map = roomToCheck.Map;
        if (map == null || roomToCheck.UsesOutdoorTemperature)
        {
            CachedRoomWallRates[roomToCheck] = AvgWallRate;
            return AvgWallRate;
        }

        if (roomToCheck.IsDoorway)
        {
            CachedRoomWallRates[roomToCheck] = AvgWallRate;
            return AvgWallRate;
        }

        var bordercells = roomToCheck.BorderCells.ToList();
        if (bordercells.Count <= 0)
        {
            CachedRoomWallRates[roomToCheck] = AvgWallRate;
            return AvgWallRate;
        }

        var TotalRate = 0f;
        var TotalCount = 0;
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

        AvgWallRate = TotalRate / TotalCount;

        CachedRoomWallRates[roomToCheck] = AvgWallRate;
        return AvgWallRate;
    }

    public static float GetAvgThinRate(Room roomToCheck)
    {
        CachedRoomThinRates ??= new Dictionary<Room, float>();

        if (CachedRoomThinRates.ContainsKey(roomToCheck) && GenTicks.TicksAbs % GenTicks.TickRareInterval != 0)
        {
            return CachedRoomThinRates[roomToCheck];
        }

        var AvgThinRate = 1f;

        var map = roomToCheck.Map;
        if (map == null || roomToCheck.UsesOutdoorTemperature)
        {
            CachedRoomThinRates[roomToCheck] = AvgThinRate;
            return AvgThinRate;
        }

        var totalThinFactor = 0f;
        var totalThinCount = 0;
        foreach (var intVec in roomToCheck.Cells)
        {
            var roof = intVec.GetRoof(map);
            if (roof == null || roof.isThickRoof)
            {
                continue;
            }

            totalThinFactor += GetRoofInsulationFactor(roof);
            totalThinCount++;
        }

        if (totalThinCount > 0)
        {
            AvgThinRate = totalThinFactor / totalThinCount;
        }

        CachedRoomThinRates[roomToCheck] = AvgThinRate;
        return AvgThinRate;
    }

    public static float GetAvgDeepRate(Room roomToCheck)
    {
        CachedRoomDeepRates ??= new Dictionary<Room, float>();

        if (CachedRoomDeepRates.ContainsKey(roomToCheck) && GenTicks.TicksAbs % GenTicks.TickRareInterval != 0)
        {
            return CachedRoomDeepRates[roomToCheck];
        }

        var AvgDeepRate = 1f;

        var map = roomToCheck.Map;
        if (map == null || roomToCheck.UsesOutdoorTemperature)
        {
            CachedRoomDeepRates[roomToCheck] = AvgDeepRate;
            return AvgDeepRate;
        }

        var totalDeepFactor = 0f;
        var totalDeepCount = 0;
        foreach (var intVec in roomToCheck.Cells)
        {
            var roof = intVec.GetRoof(map);
            if (roof is not { isThickRoof: true })
            {
                continue;
            }

            totalDeepFactor += GetRoofInsulationFactor(roof);
            totalDeepCount++;
        }

        if (totalDeepCount > 0)
        {
            AvgDeepRate = totalDeepFactor / totalDeepCount;
        }

        CachedRoomDeepRates[roomToCheck] = AvgDeepRate;
        return AvgDeepRate;
    }

    private static float GetRoofInsulationFactor(RoofDef roof)
    {
        return RoofValues.GetRVInsulationFactor(roof);
    }
}