using Verse;

namespace Insulation;

public class CompProperties_BuildInsulation : CompProperties
{
    public readonly float HeatTransferFactor = 1f;
    public bool FuelNeeded;

    public float HitPointsNeed;

    public bool InsulationStuff;

    public bool PowerNeeded;

    public bool RepairNeeded;

    public CompProperties_BuildInsulation()
    {
        compClass = typeof(CompBuildInsulate);
    }
}