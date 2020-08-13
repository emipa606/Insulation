using System;
using System.Collections.Generic;
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
			float newrate = rate;
			ThingDef def = b.def;
			CompProperties_BuildInsulation CompsBI = (def != null) ? def.GetCompProperties<CompProperties_BuildInsulation>() : null;
			if (CompsBI != null)
			{
				float HeatTransferFactor = CompsBI.HeatTransferFactor;
				bool InsulationStuff = CompsBI.InsulationStuff;
				bool PowerNeed = CompsBI.PowerNeeded;
				bool FuelNeed = CompsBI.FuelNeeded;
				bool RepairNeed = CompsBI.RepairNeeded;
				float HitPointsNeed = CompsBI.HitPointsNeed;
				if (InsulateUtility.IsBuildingWorking(b, PowerNeed, FuelNeed, RepairNeed, HitPointsNeed))
				{
					float avgStuffCatFactor = 1f;
					if (InsulationStuff)
					{
						if (b.def.MadeFromStuff)
						{
							float TotalStuffCatFactor = 0f;
							int countStuffCatFactor = 0;
							ThingDef Bstuff = (b != null) ? b.Stuff : null;
							if (((Bstuff != null) ? Bstuff.stuffProps.categories : null) != null)
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
									avgStuffCatFactor = TotalStuffCatFactor / (float)countStuffCatFactor;
								}
							}
						}
						if (HeatTransferFactor > 0f)
						{
							if (avgStuffCatFactor > 0f)
							{
								newrate *= HeatTransferFactor / avgStuffCatFactor;
							}
							else
							{
								newrate *= HeatTransferFactor;
							}
						}
					}
					else if (HeatTransferFactor > 0f)
					{
						newrate *= HeatTransferFactor;
					}
				}
			}
			return newrate * 100f / (float)Controller.Settings.pctEffective;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000254C File Offset: 0x0000074C
		public static bool IsBuildingWorking(Building b, bool PowerNeed, bool FuelNeed, bool RepairNeed, float HitPointsNeed)
		{
			if (PowerNeed)
			{
				if (ThingCompUtility.TryGetComp<CompPowerTrader>(b) == null)
				{
					return false;
				}
				if (!ThingCompUtility.TryGetComp<CompPowerTrader>(b).PowerOn)
				{
					return false;
				}
			}
			if (FuelNeed)
			{
				if (ThingCompUtility.TryGetComp<CompRefuelable>(b) == null)
				{
					return false;
				}
				if (!ThingCompUtility.TryGetComp<CompRefuelable>(b).HasFuel)
				{
					return false;
				}
			}
			return (!RepairNeed || ThingCompUtility.TryGetComp<CompBreakdownable>(b) == null || !BreakdownableUtility.IsBrokenDown(b)) && (HitPointsNeed <= 0f || HitPointsNeed > 1f || !b.def.useHitPoints || b.def.BaseMaxHitPoints <= 0 || (float)b.HitPoints / (float)b.def.BaseMaxHitPoints >= HitPointsNeed);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000025F0 File Offset: 0x000007F0
		public static float GetAvgWallRate(RoomGroup rg)
		{
			float AvgWallRate = 1f;
			Map map = (rg != null) ? rg.Map : null;
			if (map != null && !rg.UsesOutdoorTemperature)
			{
				float TotalRate = 0f;
				int TotalCount = 0;
				List<Room> rooms = rg.Rooms;
				if (rooms.Count > 0)
				{
					foreach (Room room in rooms)
					{
						if (!room.IsDoorway)
						{
							List<IntVec3> bordercells = room.BorderCells.ToList<IntVec3>();
							if (bordercells.Count > 0)
							{
								foreach (IntVec3 cell in bordercells)
								{
									if (GenGrid.InBounds(cell, map))
									{
										if (GridsUtility.GetEdifice(cell, map) != null)
										{
											float cellEdRate = InsulateUtility.GetInsulationRate(GridsUtility.GetEdifice(cell, map), 1f);
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
						}
					}
				}
				AvgWallRate = TotalRate / (float)TotalCount;
			}
			return AvgWallRate;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002724 File Offset: 0x00000924
		public static float GetAvgThinRate(RoomGroup rg)
		{
			float AvgThinRate = 1f;
			if (rg.RoomCount > 0)
			{
				Map map = (rg != null) ? rg.Map : null;
				if (map != null && !rg.UsesOutdoorTemperature)
				{
					float totalThinFactor = 0f;
					int totalThinCount = 0;
					foreach (IntVec3 intVec in rg.Cells)
					{
						RoofDef roof = GridsUtility.GetRoof(intVec, map);
						if (roof != null && !roof.isThickRoof)
						{
							totalThinFactor += InsulateUtility.GetRoofInsulationFactor(roof, false);
							totalThinCount++;
						}
					}
					if (totalThinCount > 0)
					{
						AvgThinRate = totalThinFactor / (float)totalThinCount;
					}
				}
			}
			return AvgThinRate;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000027CC File Offset: 0x000009CC
		public static float GetAvgDeepRate(RoomGroup rg)
		{
			float AvgDeepRate = 1f;
			if (rg.RoomCount > 0)
			{
				Map map = (rg != null) ? rg.Map : null;
				if (map != null && !rg.UsesOutdoorTemperature)
				{
					float totalDeepFactor = 0f;
					int totalDeepCount = 0;
					foreach (IntVec3 intVec in rg.Cells)
					{
						RoofDef roof = GridsUtility.GetRoof(intVec, map);
						if (roof != null && roof.isThickRoof)
						{
							totalDeepFactor += InsulateUtility.GetRoofInsulationFactor(roof, true);
							totalDeepCount++;
						}
					}
					if (totalDeepCount > 0)
					{
						AvgDeepRate = totalDeepFactor / (float)totalDeepCount;
					}
				}
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
